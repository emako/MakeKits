using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MakeKits.Workshop.Webview;

public static class EmbeddedResourceLoader
{
    public static Dictionary<string, byte[]> LoadResources(Assembly assembly)
    {
        foreach (string resourceName in assembly.GetManifestResourceNames())
        {
            if (resourceName.Equals("resources", StringComparison.OrdinalIgnoreCase)) continue;
            if (resourceName != "/Resource.zip") continue;

            using Stream? stream = assembly.GetManifestResourceStream(resourceName);

            if (stream == null) return [];
            return ZipHelper.ExtractZipToMemory(stream);
        }

        return [];
    }

    public static Stream? GetPackage(Assembly assembly)
    {
        foreach (string resourceName in assembly.GetManifestResourceNames())
        {
            if (resourceName.Equals("resources", StringComparison.OrdinalIgnoreCase))
                continue;

            if (resourceName == "/Package.zip")
                return assembly.GetManifestResourceStream(resourceName);
        }
        return null!;
    }

    public static Stream? GetPackageMD5(Assembly assembly)
    {
        foreach (string resourceName in assembly.GetManifestResourceNames())
        {
            if (resourceName.Equals("resources", StringComparison.OrdinalIgnoreCase))
                continue;

            if (resourceName == "/Package.md5")
                return assembly.GetManifestResourceStream(resourceName);
        }
        return null!;
    }
}
