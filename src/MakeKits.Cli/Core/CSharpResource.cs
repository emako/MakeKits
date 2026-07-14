using MakeKits.Cli.Helper;
using PureSharpCompress.Compressors.Deflate;

namespace MakeKits.Cli.Core;

internal static class CSharpResource
{
    public static void SetupConfig(string resourceDir, Configuration config)
    {
        if (!Directory.Exists(resourceDir))
            _ = Directory.CreateDirectory(resourceDir);

        string targetResource = Path.Combine(resourceDir, "Resource.zip");
        string targetPackage = Path.Combine(resourceDir, "Package.zip");

        if (Directory.Exists(config.ResourceDirectory))
        {
            if (File.Exists(targetResource))
            {
                File.Delete(targetResource);
            }
            ArchiveFileCompressHelper.CreateZip(targetResource, config.ResourceDirectory, CompressionLevel.BestCompression);
        }
        else
        {
            if (File.Exists(config.Resource))
            {
                File.Copy(config.Resource, targetResource, true);
            }
        }

        if (Directory.Exists(config.PackageDirectory))
        {
            if (File.Exists(targetPackage))
            {
                File.Delete(targetPackage);
            }
            ArchiveFileCompressHelper.CreateZip(targetPackage, config.PackageDirectory, CompressionLevel.BestCompression);
        }
        else
        {
            if (File.Exists(config.Package))
            {
                File.Copy(config.Package, targetPackage, true);
            }
        }
    }
}
