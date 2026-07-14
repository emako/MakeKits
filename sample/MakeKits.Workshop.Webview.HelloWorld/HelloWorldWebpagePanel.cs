using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;

namespace MakeKits.Workshop.Webview.HelloWorld;

public sealed class HelloWorldWebpagePanel : EmbeddedResourceWebpagePanel
{
    public override string UserDataFolder { get; set; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        @"MakeKits\Workshop\Webview2_Data\"
    );

    protected override void OnLoaded(object sender, RoutedEventArgs e)
    {
        _ = Task.Run(() =>
        {
            Assembly hiAssembly = GetType().Assembly;
            string? md5Source = null;

            if (EmbeddedResourceLoader.GetPackageMD5(hiAssembly) is Stream md5Stream)
            {
                using StreamReader reader = new(md5Stream,
                    Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: true,
                    bufferSize: 1024,
                    leaveOpen: true);

                md5Source = reader.ReadToEnd();
            }

            if (EmbeddedResourceLoader.GetPackage(hiAssembly) is Stream packageStream)
            {
                string? id = hiAssembly.GetCustomAttributes<WorkshopAttribute>()?.FirstOrDefault()?.Id;

                using (packageStream)
                {
                    string cachesDirectory = Path.Combine(Path.GetTempPath(), "MakeKits", "Caches");
                    string programDirectory = Path.Combine(cachesDirectory, id);
                    string packagePath = Path.Combine(programDirectory, "Package.zip");
                    string packageMd5Path = Path.Combine(programDirectory, "Package.md5");

                    if (File.Exists(packagePath)
                        && File.Exists(packageMd5Path)
                        && md5Source != null)
                    {
                        string md5Target = File.ReadAllText(packageMd5Path);

                        if (md5Target.Equals(md5Source, StringComparison.OrdinalIgnoreCase))
                        {
                            // The package is up-to-date, no need to extract again
                            return;
                        }
                    }

                    if (!Directory.Exists(Path.GetDirectoryName(packagePath)))
                        Directory.CreateDirectory(Path.GetDirectoryName(packagePath));

                    // Extract the embedded package to the program directory
                    using FileStream fileStream = new(packagePath, FileMode.Create, FileAccess.Write, FileShare.Delete);
                    packageStream?.CopyTo(fileStream);
                    fileStream.Close();

                    File.WriteAllText(packageMd5Path, md5Source, Encoding.UTF8);

                    // Extract the package contents to the program directory
                    ZipHelper.ExtractZipToDir(packagePath, programDirectory, true);
                }
            }
        });
    }
}
