using System;
using System.IO;
using System.Reflection;

namespace MakeKits.Workshop.Executable;

public static class EmbeddedResourceLoader
{
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

    public static Stream? GetPackageMd5(Assembly assembly)
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
