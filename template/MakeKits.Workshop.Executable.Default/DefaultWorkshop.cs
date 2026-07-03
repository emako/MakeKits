using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms.Integration;

namespace MakeKits.Workshop.Executable.Default;

/// <inheritdoc/>
public sealed class DefaultWorkshop : ExecutableWorkshop
{
    /// <inheritdoc/>
    public override LaunchType LaunchType { get; set; } = LaunchType.Process;

    /// <inheritdoc/>
    public override string CachesDirectory { get; set; } = null!;

    /// <inheritdoc/>
    public override string ProgramDirectory { get; set; } = null!;

    /// <inheritdoc/>
    public override string ProgramPath { get; set; } = null!;

    /// <inheritdoc/>
    public override string PackagePath { get; set; } = null!;

    /// <inheritdoc/>
    public override string Alias { get; set; } = "Default";

    /// <inheritdoc/>
    public override string ExecName { get; set; } = "start.ps1";

    /// <inheritdoc/>
    public override IWorkshopContext Context { get; set; } = new DefaultWorkshopContext();

    /// <inheritdoc/>
    public override void Init()
    {
        CachesDirectory ??= Path.Combine(Path.GetTempPath(), "MakeKits", "Caches");
        ProgramDirectory ??= Path.Combine(CachesDirectory, Alias);
        ProgramPath ??= Path.Combine(ProgramDirectory, ExecName);
        PackagePath = Path.Combine(ProgramDirectory, "Package.zip");

        if (!Directory.Exists(ProgramDirectory)) Directory.CreateDirectory(ProgramDirectory);

        // Extract the embedded package to the program directory
        {
            using Stream? packageStream = EmbeddedResourceLoader.GetPackage(Assembly.GetExecutingAssembly());
            using FileStream fileStream = new(PackagePath, FileMode.Create, FileAccess.Write, FileShare.Delete);

            packageStream?.CopyTo(fileStream);
        }

        // Extract the package contents to the program directory
        {
            ZipHelper.ExtractZipToDir(PackagePath, ProgramDirectory, true);
        }
    }

    /// <inheritdoc/>
    protected override DisposablePanel CreatePanel(IWorkshopContext context)
    {
        if (!File.Exists(ProgramPath))
        {
            throw new FileNotFoundException("The executable file was not found.", ProgramPath);
        }

        try
        {
            string fileName = ProgramPath.EndsWith(".ps1") ? "powershell.exe" : ProgramPath;

            ProcessStartInfo processStartInfo = new()
            {
                FileName = fileName,
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = ProgramPath.EndsWith(".ps1") ? $"-ExecutionPolicy Bypass -File \"{ProgramPath}\"" : null,
            };
            using Process process = Process.Start(processStartInfo);
            string[] exePaths = [.. Directory.EnumerateFiles(ProgramDirectory, "*.exe", SearchOption.AllDirectories)];
            DefaultHostPanel panel = new();

            ProcessWindowPolling.PollProcessWindow(exePaths, windowHandle =>
            {
                Debug.WriteLine($"Found window handle: {windowHandle}");

                panel.Dispatcher.Invoke(() => panel.AttachExternalWindow(windowHandle));
            });

            return panel;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }

        return null!;
    }
}
