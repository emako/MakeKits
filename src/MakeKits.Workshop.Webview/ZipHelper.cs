using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace MakeKits.Workshop.Webview;

public static class ZipHelper
{
    /// <summary>
    /// Extract all files from ZIP memory stream to memory
    /// </summary>
    /// <param name="zipStream">The ZIP memory stream.</param>
    /// <returns>A dictionary where the key is the internal path in the ZIP (using '/' as the separator) and the value is the file content.</returns>
    public static Dictionary<string, byte[]> ExtractZipToMemory(Stream zipStream)
    {
        if (zipStream == null)
            throw new ArgumentNullException(nameof(zipStream));

        zipStream.Position = 0;

        var result = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);

        using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Read, true))
        {
            foreach (var entry in archive.Entries)
            {
                // Skip directory
                if (string.IsNullOrEmpty(entry.Name))
                    continue;

                using Stream entryStream = entry.Open();
                using MemoryStream ms = new();
                entryStream.CopyTo(ms);

                // Uniform path separator
                string path = entry.FullName.Replace('\\', '/');
                if (!path.StartsWith("/")) path = "/" + path;

                result[path] = ms.ToArray();
            }
        }

        return result;
    }

    /// <summary>
    /// Extracts a ZIP archive to the specified directory (compatible with .NET Framework 4.x).
    /// </summary>
    /// <param name="zipFile">Path to the ZIP file.</param>
    /// <param name="destinationDirectory">Destination directory.</param>
    /// <param name="overwrite">Whether to overwrite existing files.</param>
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
            // ZIP entries use '/' as the path separator
            var relativePath = entry.FullName.Replace('/', Path.DirectorySeparatorChar);

            var fullPath = Path.GetFullPath(
                Path.Combine(destinationDirectory, relativePath));

            // Prevent Zip Slip vulnerability
            var root = Path.GetFullPath(destinationDirectory)
                .TrimEnd(Path.DirectorySeparatorChar)
                + Path.DirectorySeparatorChar;

            if (!fullPath.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Zip contains invalid entry: " + entry.FullName);

            // Directory entry
            if (string.IsNullOrEmpty(entry.Name))
            {
                Directory.CreateDirectory(fullPath);
                continue;
            }

            // Create parent directory
            string? directoryName = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directoryName))
                Directory.CreateDirectory(directoryName);

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
