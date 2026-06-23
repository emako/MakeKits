using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MakeKits.Workshop.Webview;

public static class EmbeddedResourceLoader
{
    public static Dictionary<string, byte[]> LoadResources(Assembly assembly, string resourcePrefix)
    {
        Dictionary<string, byte[]> resources = [];

        foreach (string resourceName in assembly.GetManifestResourceNames())
        {
            if (!resourceName.StartsWith(resourcePrefix, StringComparison.Ordinal))
                continue;

            string relativePath = resourceName.Substring(resourcePrefix.Length);
            if (relativePath.Equals("resources", StringComparison.OrdinalIgnoreCase)) continue;

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null) continue;
            using MemoryStream memoryStream = new();
            stream.CopyTo(memoryStream);
            resources[$"/{relativePath.Replace('\\', '/')}"] = memoryStream.ToArray();
        }

        return resources;
    }

    public static byte[]? ReadResource(IReadOnlyDictionary<string, byte[]> resources, string key)
    {
        return resources.TryGetValue(key, out byte[]? value) ? value : null;
    }
}
