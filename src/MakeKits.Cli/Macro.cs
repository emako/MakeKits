using System.Reflection;

namespace MakeKits.Cli;

internal static class Macro
{
    public const string KitsDir = "${KitsDir}";
    public const string Id = "${Id}";
    public const string AssemblyName = "${AssemblyName}";

    public static string GetKitsDir()
        => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    public static string GetFullPath(string? path)
    {
        string? newPath = path?.Replace(KitsDir, GetKitsDir());

        if (string.IsNullOrWhiteSpace(newPath))
        {
            return newPath!;
        }
        return Path.GetFullPath(path);
    }

    /// <summary>
    /// Solve Template path.
    /// </summary>
    /// <param name="value">Recommend for ${KitsDir}/template/default.7z</param>
    public static string SolveTemplate(this string value)
    {
        _ = value ?? throw new ArgumentNullException(nameof(value));

        if (value.Contains(KitsDir))
        {
            string kitsdir = GetKitsDir();
            return value.Replace(KitsDir, kitsdir);
        }

        return value;
    }

    /// <summary>
    /// Solve AssemblyName setup path.
    /// </summary>
    /// <param name="value">Recommend for ./${AppName}_v${Version}_win64.exe</param>
    public static string SolveAssemblyName(this string value, Configuration config)
    {
        _ = value ?? throw new ArgumentNullException(nameof(value));

        return
            value.Replace(Id, config.Id);
    }

    /// <summary>
    /// Solve Output setup path.
    /// </summary>
    /// <param name="value">Recommend for ./${AppName}_v${Version}_win64.exe</param>
    public static string SolveOutput(this string value, Configuration config)
    {
        _ = value ?? throw new ArgumentNullException(nameof(value));

        return
            value.Replace(AssemblyName, config.AssemblyName)
                 .Replace(Id, config.Id);
    }
}
