using System;
using System.IO;
using System.IO.Compression;

namespace MakeKits.Workshop.Executable;

public static class ZipHelper
{
    /// <summary>
    /// 解压 ZIP 到指定目录（兼容 .NET Framework 4.x）。
    /// </summary>
    /// <param name="zipFile">zip 文件路径</param>
    /// <param name="destinationDirectory">目标目录</param>
    /// <param name="overwrite">是否覆盖已有文件</param>
    public static void ExtractZipToDir(string zipFile, string destinationDirectory, bool overwrite = true)
    {
        if (string.IsNullOrWhiteSpace(zipFile))
            throw new ArgumentNullException(nameof(zipFile));

        if (!File.Exists(zipFile))
            throw new FileNotFoundException(zipFile);

        Directory.CreateDirectory(destinationDirectory);

        using ZipArchive archive = ZipFile.OpenRead(zipFile);
        foreach (var entry in archive.Entries)
        {
            // Zip 内统一使用 '/'
            var relativePath = entry.FullName.Replace('/', Path.DirectorySeparatorChar);

            var fullPath = Path.GetFullPath(
                Path.Combine(destinationDirectory, relativePath));

            // 防止 Zip Slip 漏洞
            var root = Path.GetFullPath(destinationDirectory)
                .TrimEnd(Path.DirectorySeparatorChar)
                + Path.DirectorySeparatorChar;

            if (!fullPath.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Zip contains invalid entry: " + entry.FullName);

            // 目录
            if (string.IsNullOrEmpty(entry.Name))
            {
                Directory.CreateDirectory(fullPath);
                continue;
            }

            // 创建父目录
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            if (overwrite)
            {
                entry.ExtractToFile(fullPath, true);
            }
            else
            {
                entry.ExtractToFile(fullPath);
            }
        }
    }
}
