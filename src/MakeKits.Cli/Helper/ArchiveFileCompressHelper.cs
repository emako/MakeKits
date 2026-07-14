using PureSharpCompress.Archives;
using PureSharpCompress.Archives.Zip;
using PureSharpCompress.Common;
using PureSharpCompress.Compressors.Deflate;
using PureSharpCompress.Writers;

namespace MakeKits.Cli.Helper;

internal static class ArchiveFileCompressHelper
{
    public static void CreateZip(string destinationFilePath, string sourceDirectory, CompressionLevel compressionLevel = CompressionLevel.Default, string searchPattern = "*.*", SearchOption searchOption = SearchOption.AllDirectories)
    {
        using ZipArchive archive = ZipArchive.Create();
        archive.DeflateCompressionLevel = compressionLevel;
        archive.AddAllFromDirectory(sourceDirectory, searchPattern, searchOption);
        archive.SaveTo(destinationFilePath, new WriterOptions(CompressionType.Deflate));
    }

    public static void CreateZip(Stream destinationStream, string sourceDirectory, CompressionLevel compressionLevel = CompressionLevel.Default, string searchPattern = "*.*", SearchOption searchOption = SearchOption.AllDirectories, bool leaveStreamOpen = false)
    {
        using ZipArchive archive = ZipArchive.Create();
        archive.DeflateCompressionLevel = compressionLevel;
        archive.AddAllFromDirectory(sourceDirectory, searchPattern, searchOption);
        archive.SaveTo(destinationStream, new WriterOptions(CompressionType.Deflate)
        {
            LeaveStreamOpen = leaveStreamOpen,
        });
    }
}
