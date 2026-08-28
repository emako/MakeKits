using System.IO.Compression;
using System.Reflection;

namespace WhatsShop;

/// <summary>
/// Purely reflective reading of the DLL workshop metadata (MakeKits type is not referenced at compile time).
/// At runtime, CLR parses the assembly to MakeKits.Workshop.Abstractions via local covenant stakes with the same name.
/// </summary>
internal static class WorkshopMetadataReader
{
    private const string AbstractionsAssemblyName = "MakeKits.Workshop.Abstractions";
    private const string IWorkshopFullName = "MakeKits.Workshop.IWorkshop";
    private const string WorkshopAttributeFullName = "MakeKits.Workshop.WorkshopAttribute";

    private static readonly object ResolveGate = new();
    private static bool s_resolveHooked;
    private static Assembly? s_abstractionsAssembly;

    [ThreadStatic]
    private static bool t_resolvingAbstractions;

    public static WorkshopMetaInfo? TryRead(string libraryPath)
    {
        if (string.IsNullOrWhiteSpace(libraryPath) || !File.Exists(libraryPath))
        {
            return null;
        }

        EnsureAbstractionsResolveHooked();

        try
        {
            AssemblyName assemblyName = AssemblyName.GetAssemblyName(libraryPath);
            Assembly assembly = Assembly.LoadFrom(libraryPath);

            string? workshopId = TryReadWorkshopId(assembly);
            string? version = assemblyName.Version is null ? null : $"v{assemblyName.Version}";
            string fileName = Path.GetFileName(libraryPath);

            Type? workshopType = null;
            string? typeLoadError = null;
            try
            {
                workshopType = GetWorkshopType(assembly);
            }
            catch (Exception ex)
            {
                typeLoadError = FormatException(ex);
            }

            if (workshopType is null && string.IsNullOrWhiteSpace(workshopId))
            {
                if (!string.IsNullOrWhiteSpace(typeLoadError))
                {
                    return new WorkshopMetaInfo
                    {
                        Version = version ?? string.Empty,
                        Error = typeLoadError!,
                    };
                }

                return null;
            }

            string? name = null;
            string? author = null;
            string? description = null;
            string? descriptorError = null;

            if (workshopType is not null)
            {
                try
                {
                    TryReadDescriptor(workshopType, out name, out author, out description);
                }
                catch (Exception ex)
                {
                    descriptorError = FormatException(ex);
                }
            }

            var info = new WorkshopMetaInfo
            {
                Id = workshopId ?? string.Empty,
                Name = string.IsNullOrWhiteSpace(name)
                    ? Path.GetFileNameWithoutExtension(fileName)
                    : name!,
                Author = author ?? string.Empty,
                Description = description ?? string.Empty,
                Version = version ?? string.Empty,
                Error = string.Empty,
            };

            if (string.IsNullOrWhiteSpace(info.Id)
                && workshopType is null
                && (!string.IsNullOrWhiteSpace(typeLoadError) || !string.IsNullOrWhiteSpace(descriptorError)))
            {
                info.Error = typeLoadError ?? descriptorError ?? string.Empty;
            }
            else if (!string.IsNullOrWhiteSpace(descriptorError)
                && string.IsNullOrWhiteSpace(name)
                && string.IsNullOrWhiteSpace(author)
                && string.IsNullOrWhiteSpace(description))
            {
                info.Error = descriptorError ?? string.Empty;
            }

            return info;
        }
        catch (Exception ex)
        {
            return new WorkshopMetaInfo
            {
                Error = FormatException(ex),
            };
        }
    }

    private static void EnsureAbstractionsResolveHooked()
    {
        if (s_resolveHooked)
        {
            return;
        }

        lock (ResolveGate)
        {
            if (s_resolveHooked)
            {
                return;
            }

            s_abstractionsAssembly = ResolveAbstractionsAssembly();

            AppDomain.CurrentDomain.AssemblyResolve += static (_, args) =>
            {
                if (t_resolvingAbstractions)
                {
                    return null;
                }

                try
                {
                    string simpleName;
                    try
                    {
                        simpleName = new AssemblyName(args.Name).Name ?? string.Empty;
                    }
                    catch
                    {
                        return null;
                    }

                    if (!string.Equals(
                            simpleName,
                            AbstractionsAssemblyName,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }

                    t_resolvingAbstractions = true;
                    return ResolveAbstractionsAssembly();
                }
                catch
                {
                    return null;
                }
                finally
                {
                    t_resolvingAbstractions = false;
                }
            };

            s_resolveHooked = true;
        }
    }

    private static Assembly? ResolveAbstractionsAssembly()
    {
        if (s_abstractionsAssembly is not null)
        {
            return s_abstractionsAssembly;
        }

        Assembly? found = FindLoadedAbstractionsAssembly()
            ?? TryLoadAbstractionsFromDisk()
            ?? TryLoadAbstractionsFromCostura();

        if (found is not null)
        {
            s_abstractionsAssembly = found;
        }

        return found;
    }

    private static Assembly? FindLoadedAbstractionsAssembly()
    {
        foreach (Assembly loaded in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                if (string.Equals(
                        loaded.GetName().Name,
                        AbstractionsAssemblyName,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return loaded;
                }
            }
            catch
            {
                // ignore
            }
        }

        return null;
    }

    private static Assembly? TryLoadAbstractionsFromDisk()
    {
        try
        {
            string? baseDir = AppDomain.CurrentDomain.BaseDirectory;
            if (string.IsNullOrWhiteSpace(baseDir))
            {
                return null;
            }

            string beside = Path.Combine(baseDir!, AbstractionsAssemblyName + ".dll");
            if (!File.Exists(beside))
            {
                return null;
            }

            return Assembly.LoadFrom(beside);
        }
        catch
        {
            return null;
        }
    }

    private static Assembly? TryLoadAbstractionsFromCostura()
    {
        try
        {
            Assembly host = typeof(WorkshopMetadataReader).Assembly;
            string token = "costura." + AbstractionsAssemblyName.ToLowerInvariant() + ".dll";

            foreach (string resourceName in host.GetManifestResourceNames())
            {
                string lower = resourceName.ToLowerInvariant();
                if (!lower.StartsWith(token, StringComparison.Ordinal))
                {
                    continue;
                }

                using Stream? raw = host.GetManifestResourceStream(resourceName);
                if (raw is null)
                {
                    continue;
                }

                byte[] bytes;
                if (lower.EndsWith(".compressed", StringComparison.Ordinal))
                {
                    using DeflateStream deflate = new(raw, CompressionMode.Decompress);
                    using MemoryStream ms = new();
                    deflate.CopyTo(ms);
                    bytes = ms.ToArray();
                }
                else
                {
                    using MemoryStream ms = new();
                    raw.CopyTo(ms);
                    bytes = ms.ToArray();
                }

                if (bytes.Length == 0)
                {
                    continue;
                }

                return Assembly.Load(bytes);
            }
        }
        catch
        {
            // ignore
        }

        return null;
    }

    private static string? TryReadWorkshopId(Assembly assembly)
    {
        try
        {
            foreach (CustomAttributeData data in CustomAttributeData.GetCustomAttributes(assembly))
            {
                if (!IsTypeFullName(data.AttributeType, WorkshopAttributeFullName))
                {
                    continue;
                }

                if (data.ConstructorArguments.Count > 0
                    && data.ConstructorArguments[0].Value is string ctorId
                    && !string.IsNullOrWhiteSpace(ctorId))
                {
                    return ctorId;
                }

                foreach (CustomAttributeNamedArgument named in data.NamedArguments)
                {
                    if (string.Equals(named.MemberName, "Id", StringComparison.Ordinal)
                        && named.TypedValue.Value is string namedId
                        && !string.IsNullOrWhiteSpace(namedId))
                    {
                        return namedId;
                    }
                }
            }
        }
        catch
        {
            // Ignore if the attribute type cannot be parsed
        }

        return null;
    }

    private static Type? GetWorkshopType(Assembly assembly)
    {
        Type[] types = GetLoadableTypes(assembly);

        return types
            .Where(type =>
                type is not null
                && !type.IsInterface
                && !type.IsAbstract
                && ImplementsInterfaceByName(type, IWorkshopFullName))
            .OrderByDescending(GetInheritanceDepth)
            .FirstOrDefault();
    }

    private static Type[] GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            Type[] partial = [.. ex.Types.Where(t => t is not null).Cast<Type>()];
            if (partial.Length == 0)
            {
                throw;
            }

            return partial;
        }
    }

    private static bool ImplementsInterfaceByName(Type type, string interfaceFullName)
    {
        try
        {
            foreach (Type iface in GetInterfacesSafe(type))
            {
                if (IsTypeFullName(iface, interfaceFullName))
                {
                    return true;
                }
            }
        }
        catch
        {
            return false;
        }

        return false;
    }

    private static IEnumerable<Type> GetInterfacesSafe(Type type)
    {
        for (Type? current = type; current is not null; current = current.BaseType)
        {
            Type[] interfaces;
            try
            {
                interfaces = current.GetInterfaces();
            }
            catch
            {
                continue;
            }

            foreach (Type iface in interfaces)
            {
                yield return iface;
            }
        }
    }

    private static bool IsTypeFullName(Type type, string fullName)
    {
        try
        {
            return string.Equals(type.FullName, fullName, StringComparison.Ordinal);
        }
        catch
        {
            return false;
        }
    }

    private static void TryReadDescriptor(
        Type workshopType,
        out string? name,
        out string? author,
        out string? description)
    {
        name = null;
        author = null;
        description = null;

        object? instance = Activator.CreateInstance(workshopType);
        if (instance is null)
        {
            return;
        }

        PropertyInfo? descriptorProp = FindInstanceProperty(workshopType, "Descriptor");
        object? descriptor = descriptorProp?.GetValue(instance, null);
        if (descriptor is null)
        {
            return;
        }

        Type descriptorType = descriptor.GetType();
        name = GetStringProperty(descriptorType, descriptor, "Name");
        author = GetStringProperty(descriptorType, descriptor, "Author");
        description = GetStringProperty(descriptorType, descriptor, "Description");
    }

    private static PropertyInfo? FindInstanceProperty(Type type, string propertyName)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
        for (Type? current = type; current is not null; current = current.BaseType)
        {
            PropertyInfo? prop = current.GetProperty(propertyName, flags);
            if (prop is not null && prop.GetIndexParameters().Length == 0)
            {
                return prop;
            }
        }

        return null;
    }

    private static string? GetStringProperty(Type type, object instance, string propertyName)
    {
        PropertyInfo? prop = FindInstanceProperty(type, propertyName);
        return prop?.GetValue(instance, null) as string;
    }

    private static int GetInheritanceDepth(Type type)
    {
        int depth = 0;
        for (Type? t = type; t is not null; t = t.BaseType)
        {
            depth++;
        }

        return depth;
    }

    private static string FormatException(Exception ex)
    {
        if (ex is ReflectionTypeLoadException rtle)
        {
            string detail = string.Join(
                "; ",
                (rtle.LoaderExceptions ?? [])
                    .Where(e => e is not null)
                    .Select(e => e!.Message)
                    .Distinct());
            return string.IsNullOrWhiteSpace(detail)
                ? rtle.Message
                : $"{rtle.Message} ({detail})";
        }

        if (ex is TargetInvocationException { InnerException: not null } tie)
        {
            return FormatException(tie.InnerException!);
        }

        return ex.GetBaseException().Message;
    }
}
