namespace MakeKits.Cli.Core;

internal static class CSharpResource
{
    public static void SetupConfig(string resourceDir, Configuration config)
    {
        if (!Directory.Exists(resourceDir))
            _ = Directory.CreateDirectory(resourceDir);

        if (File.Exists(config.Resource))
        {
            File.Copy(config.Resource, Path.Combine(resourceDir, "Resource.7z"), true);
        }

        if (File.Exists(config.Package))
        {
            File.Copy(config.Package, Path.Combine(resourceDir, "Package.zip"), true);
        }
    }
}
