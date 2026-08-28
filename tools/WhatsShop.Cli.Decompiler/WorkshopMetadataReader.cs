using System.Text.RegularExpressions;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace WhatsShop;

/// <summary>
/// Reads workshop metadata from a DLL via ICSharpCode.Decompiler (no Assembly.Load).
/// </summary>
internal static partial class WorkshopMetadataReader
{
    private const string IWorkshopFullName = "MakeKits.Workshop.IWorkshop";
    private const string IWorkshopDescriptorFullName = "MakeKits.Workshop.IWorkshopDescriptor";
    private const string WorkshopAttributeFullName = "MakeKits.Workshop.WorkshopAttribute";

    [GeneratedRegex(
        @"\b(?<name>Id|Name|Author|Description)\b\s*\{[^}]*\}\s*=\s*(?<value>null|""(?:\\.|[^""])*""|@""(?:""""|[^""])*"")",
        RegexOptions.CultureInvariant)]
    private static partial Regex PropertyInitializerRegex();

    [GeneratedRegex(
        @"\b(?<name>Id|Name|Author|Description)\b\s*=>\s*(?<value>""(?:\\.|[^""])*""|@""(?:""""|[^""])*"")\s*;",
        RegexOptions.CultureInvariant)]
    private static partial Regex ExpressionBodyStringRegex();

    [GeneratedRegex(
        @"\b(?<name>Name|Author|Description)\b\s*=>\s*(?<type>[A-Za-z_][\w.]*)\.(?<member>Id|Name|Author|Description)\s*;",
        RegexOptions.CultureInvariant)]
    private static partial Regex ExpressionBodyMemberRegex();

    public static WorkshopMetaInfo? TryRead(string libraryPath)
    {
        if (string.IsNullOrWhiteSpace(libraryPath) || !File.Exists(libraryPath))
        {
            return null;
        }

        try
        {
            string absolutePath = Path.GetFullPath(libraryPath);
            using PEFile peFile = new(absolutePath);
            CSharpDecompiler decompiler = CreateDecompiler(absolutePath);
            ICompilation compilation = decompiler.TypeSystem;
            IModule module = compilation.MainModule;

            string? workshopId = TryReadWorkshopId(module);
            string? version = TryReadVersion(peFile);
            string fileName = Path.GetFileName(absolutePath);

            bool hasWorkshopType = HasWorkshopType(module);
            Dictionary<string, string?> configurationValues = TryReadConfigurationValues(decompiler, module);
            Dictionary<string, string?> descriptorValues = TryReadDescriptorValues(decompiler, module, configurationValues);

            if (!hasWorkshopType
                && string.IsNullOrWhiteSpace(workshopId)
                && configurationValues.Count == 0
                && descriptorValues.Count == 0)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(workshopId)
                && configurationValues.TryGetValue("Id", out string? configId)
                && !string.IsNullOrWhiteSpace(configId))
            {
                workshopId = configId;
            }

            string? name = FirstNonEmpty(
                GetValue(descriptorValues, "Name"),
                GetValue(configurationValues, "Name"));
            string? author = FirstNonEmpty(
                GetValue(descriptorValues, "Author"),
                GetValue(configurationValues, "Author"));
            string? description = FirstNonEmpty(
                GetValue(descriptorValues, "Description"),
                GetValue(configurationValues, "Description"));

            return new WorkshopMetaInfo
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
        }
        catch (Exception ex)
        {
            return new WorkshopMetaInfo
            {
                Error = ex.GetBaseException().Message,
            };
        }
    }

    private static CSharpDecompiler CreateDecompiler(string absolutePath)
    {
        string? directory = Path.GetDirectoryName(absolutePath);
        UniversalAssemblyResolver resolver = new(
            absolutePath,
            throwOnError: false,
            targetFramework: ".NETFramework,Version=v4.8");

        if (!string.IsNullOrWhiteSpace(directory))
        {
            resolver.AddSearchDirectory(directory!);
        }

        DecompilerSettings settings = new()
        {
            ThrowOnAssemblyResolveErrors = false,
        };

        return new CSharpDecompiler(absolutePath, resolver, settings);
    }

    private static string? TryReadVersion(PEFile peFile)
    {
        try
        {
            if (!peFile.Metadata.IsAssembly)
            {
                return null;
            }

            Version version = peFile.Metadata.GetAssemblyDefinition().Version;
            return $"v{version}";
        }
        catch
        {
            return null;
        }
    }

    private static string? TryReadWorkshopId(IModule module)
    {
        try
        {
            foreach (IAttribute attr in module.GetAssemblyAttributes())
            {
                if (!string.Equals(attr.AttributeType.FullName, WorkshopAttributeFullName, StringComparison.Ordinal))
                {
                    continue;
                }

                if (attr.FixedArguments.Length > 0
                    && attr.FixedArguments[0].Value is string ctorId
                    && !string.IsNullOrWhiteSpace(ctorId))
                {
                    return ctorId;
                }

                foreach (var named in attr.NamedArguments)
                {
                    if (string.Equals(named.Name, "Id", StringComparison.Ordinal)
                        && named.Value is string namedId
                        && !string.IsNullOrWhiteSpace(namedId))
                    {
                        return namedId;
                    }
                }
            }
        }
        catch
        {
            // ignore attribute decode failures
        }

        return null;
    }

    private static bool HasWorkshopType(IModule module)
    {
        foreach (ITypeDefinition type in module.TypeDefinitions)
        {
            if (type.Kind is not (TypeKind.Class or TypeKind.Struct))
            {
                continue;
            }

            if (type.IsAbstract || type.IsStatic)
            {
                continue;
            }

            if (ImplementsInterfaceByName(type, IWorkshopFullName))
            {
                return true;
            }
        }

        return false;
    }

    private static bool ImplementsInterfaceByName(ITypeDefinition type, string interfaceFullName)
    {
        HashSet<ITypeDefinition> seen = [];
        Stack<ITypeDefinition> stack = new();
        stack.Push(type);

        while (stack.Count > 0)
        {
            ITypeDefinition current = stack.Pop();
            if (!seen.Add(current))
            {
                continue;
            }

            foreach (IType baseType in current.DirectBaseTypes)
            {
                if (string.Equals(baseType.FullName, interfaceFullName, StringComparison.Ordinal))
                {
                    return true;
                }

                ITypeDefinition? baseDef = baseType.GetDefinition();
                if (baseDef is not null)
                {
                    stack.Push(baseDef);
                }
            }
        }

        return false;
    }

    private static Dictionary<string, string?> TryReadConfigurationValues(
        CSharpDecompiler decompiler,
        IModule module)
    {
        Dictionary<string, string?> values = [with(StringComparer.Ordinal)];

        foreach (ITypeDefinition type in module.TypeDefinitions)
        {
            if (!string.Equals(type.Name, "Configuration", StringComparison.Ordinal))
            {
                continue;
            }

            if (!type.IsStatic && type.Kind != TypeKind.Class)
            {
                continue;
            }

            try
            {
                string code = decompiler.DecompileTypeAsString(type.FullTypeName);
                MergeStringProperties(values, code);
            }
            catch
            {
                // ignore decompile failures for individual types
            }
        }

        return values;
    }

    private static Dictionary<string, string?> TryReadDescriptorValues(
        CSharpDecompiler decompiler,
        IModule module,
        IReadOnlyDictionary<string, string?> configurationValues)
    {
        Dictionary<string, string?> values = [with(StringComparer.Ordinal)];

        List<ITypeDefinition> descriptors = [.. module.TypeDefinitions
            .Where(type =>
                type.Kind == TypeKind.Class
                && !type.IsAbstract
                && !type.IsStatic
                && ImplementsInterfaceByName(type, IWorkshopDescriptorFullName))
            .OrderByDescending(GetInheritanceDepth)];

        foreach (ITypeDefinition descriptor in descriptors)
        {
            try
            {
                string code = decompiler.DecompileTypeAsString(descriptor.FullTypeName);
                Dictionary<string, string?> local = [with(StringComparer.Ordinal)];
                MergeStringProperties(local, code);
                MergeMemberReferences(local, code, configurationValues);

                // Prefer overrides that actually produced values; keep searching if empty.
                foreach (KeyValuePair<string, string?> pair in local)
                {
                    if (!values.ContainsKey(pair.Key) || string.IsNullOrWhiteSpace(values[pair.Key]))
                    {
                        values[pair.Key] = pair.Value;
                    }
                }

                if (!string.IsNullOrWhiteSpace(GetValue(values, "Name"))
                    || !string.IsNullOrWhiteSpace(GetValue(values, "Author"))
                    || !string.IsNullOrWhiteSpace(GetValue(values, "Description")))
                {
                    break;
                }
            }
            catch
            {
                // ignore decompile failures for individual types
            }
        }

        return values;
    }

    private static void MergeStringProperties(Dictionary<string, string?> values, string code)
    {
        foreach (Match match in PropertyInitializerRegex().Matches(code))
        {
            string name = match.Groups["name"].Value;
            string? decoded = DecodeCSharpStringOrNull(match.Groups["value"].Value);
            if (!values.ContainsKey(name) || string.IsNullOrWhiteSpace(values[name]))
            {
                values[name] = decoded;
            }
        }

        foreach (Match match in ExpressionBodyStringRegex().Matches(code))
        {
            string name = match.Groups["name"].Value;
            string? decoded = DecodeCSharpStringOrNull(match.Groups["value"].Value);
            if (!values.ContainsKey(name) || string.IsNullOrWhiteSpace(values[name]))
            {
                values[name] = decoded;
            }
        }
    }

    private static void MergeMemberReferences(
        Dictionary<string, string?> values,
        string code,
        IReadOnlyDictionary<string, string?> configurationValues)
    {
        foreach (Match match in ExpressionBodyMemberRegex().Matches(code))
        {
            string name = match.Groups["name"].Value;
            string member = match.Groups["member"].Value;
            if (values.ContainsKey(name) && !string.IsNullOrWhiteSpace(values[name]))
            {
                continue;
            }

            if (configurationValues.TryGetValue(member, out string? referenced)
                && !string.IsNullOrWhiteSpace(referenced))
            {
                values[name] = referenced;
            }
        }
    }

    private static string? DecodeCSharpStringOrNull(string raw)
    {
        if (string.Equals(raw, "null", StringComparison.Ordinal))
        {
            return null;
        }

        if (raw.Length >= 2 && raw[0] == '"' && raw[raw.Length - 1] == '"')
        {
            return UnescapeRegularString(raw.Substring(1, raw.Length - 2));
        }

        if (raw.Length >= 3 && raw.StartsWith("@\"", StringComparison.Ordinal) && raw.EndsWith("\"", StringComparison.Ordinal))
        {
            return raw.Substring(2, raw.Length - 3).Replace("\"\"", "\"");
        }

        return null;
    }

    private static string UnescapeRegularString(string value)
    {
        System.Text.StringBuilder result = new(value.Length);
        for (int i = 0; i < value.Length; i++)
        {
            char c = value[i];
            if (c != '\\' || i + 1 >= value.Length)
            {
                result.Append(c);
                continue;
            }

            char next = value[++i];
            switch (next)
            {
                case 'n':
                    result.Append('\n');
                    break;

                case 'r':
                    result.Append('\r');
                    break;

                case 't':
                    result.Append('\t');
                    break;

                case '0':
                    result.Append('\0');
                    break;

                case '\\':
                case '"':
                case '\'':
                    result.Append(next);
                    break;

                case 'u' when i + 4 < value.Length
                    && int.TryParse(
                        value.Substring(i + 1, 4),
                        System.Globalization.NumberStyles.HexNumber,
                        null,
                        out int code):
                    result.Append((char)code);
                    i += 4;
                    break;

                default:
                    result.Append(next);
                    break;
            }
        }

        return result.ToString();
    }

    private static int GetInheritanceDepth(ITypeDefinition type)
    {
        int depth = 0;
        for (IType? current = type; current is not null; current = current.DirectBaseTypes.FirstOrDefault())
        {
            depth++;
            if (depth > 64)
            {
                break;
            }
        }

        return depth;
    }

    private static string? GetValue(IReadOnlyDictionary<string, string?> values, string key)
        => values.TryGetValue(key, out string? value) ? value : null;

    private static string? FirstNonEmpty(params string?[] values)
        => values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
}
