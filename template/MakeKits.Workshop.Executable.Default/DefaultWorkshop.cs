using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace MakeKits.Workshop.Executable.Default;

/// <inheritdoc/>
public sealed class DefaultWorkshop : ExecutableWorkshop
{
    /// <inheritdoc/>
    public override LaunchType LaunchType
    {
        get
        {
            if (Enum.TryParse(Configuration.LaunchType, out LaunchType launchType))
            {
                return launchType;
            }
            return LaunchType.Process;
        }
    }

    /// <inheritdoc/>
    public override string CachesDirectory { get; set; } = null!;

    /// <inheritdoc/>
    public override string ProgramDirectory { get; set; } = null!;

    /// <inheritdoc/>
    public override string ProgramPath { get; set; } = null!;

    /// <inheritdoc/>
    public override string PackagePath { get; set; } = null!;

    /// <inheritdoc/>
    public override string PackageMd5Path { get; set; } = null!;

    /// <inheritdoc/>
    public override string Id => Configuration.Id;

    /// <inheritdoc/>
    public override string ExecName => Configuration.ExecName;

    /// <inheritdoc/>
    public override IWorkshopContext Context { get; set; } = new DefaultWorkshopContext();

    /// <inheritdoc/>
    public override void Init()
    {
        CachesDirectory ??= Path.Combine(Path.GetTempPath(), "MakeKits", "Caches");
        ProgramDirectory ??= Path.Combine(CachesDirectory, Id);
        ProgramPath ??= Path.Combine(ProgramDirectory, ExecName);
        PackagePath = Path.Combine(ProgramDirectory, "Package.zip");
        PackageMd5Path = Path.Combine(ProgramDirectory, "Package.md5");

        if (!Directory.Exists(ProgramDirectory)) Directory.CreateDirectory(ProgramDirectory);

        // Extract the embedded package to the program directory
        if (!File.Exists(PackageMd5Path))
        {
            using Stream? md5Stream = EmbeddedResourceLoader.GetPackageMd5(Assembly.GetExecutingAssembly());
            using FileStream fileStream = new(PackageMd5Path, FileMode.Create, FileAccess.Write, FileShare.Delete);

            md5Stream?.CopyTo(fileStream);
        }

        // Extract the embedded package to the program directory
        if (!File.Exists(PackagePath))
        {
            using Stream? packageStream = EmbeddedResourceLoader.GetPackage(Assembly.GetExecutingAssembly());
            using FileStream fileStream = new(PackagePath, FileMode.Create, FileAccess.Write, FileShare.Delete);

            packageStream?.CopyTo(fileStream);
            fileStream.Close();

            // Extract the package contents to the program directory
            ZipHelper.ExtractZipToDir(PackagePath, ProgramDirectory, true);
        }
        else
        {
            using Stream? md5Stream = EmbeddedResourceLoader.GetPackageMd5(Assembly.GetExecutingAssembly());
            using StreamReader reader = new(md5Stream,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true,
                bufferSize: 1024,
                leaveOpen: true);

            if (File.ReadAllText(PackageMd5Path) != reader.ReadToEnd())
            {
                using Stream? packageStream = EmbeddedResourceLoader.GetPackage(Assembly.GetExecutingAssembly());
                using FileStream fileStream = new(PackagePath, FileMode.Create, FileAccess.Write, FileShare.Delete);

                packageStream?.CopyTo(fileStream);
                fileStream.Close();

                // Extract the package contents to the program directory
                ZipHelper.ExtractZipToDir(PackagePath, ProgramDirectory, true);
            }
        }
    }

    /// <inheritdoc/>
    protected override WindowHostPanel CreatePanel(IWorkshopContext context)
    {
        if (!File.Exists(ProgramPath))
        {
            throw new FileNotFoundException("The executable file was not found.", ProgramPath);
        }

        try
        {
            if (ProgramPath.EndsWith(".ps1"))
            {
                ProcessStartInfo processStartInfo = new()
                {
                    FileName = "powershell.exe",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = $"-ExecutionPolicy Bypass -File \"{ProgramPath}\"",
                };
                using Process process = Process.Start(processStartInfo);
                string[] exePaths = [.. Directory.EnumerateFiles(ProgramDirectory, "*.exe", SearchOption.AllDirectories)];
                DefaultHostPanel panel = new();

                PWP.PollProcessWindow(exePaths, windowHandle =>
                {
                    Debug.WriteLine($"[PWP] Found window handle: {windowHandle}");

                    panel.Dispatcher.Invoke(() => panel.AttachExternalWindow(windowHandle));
                });

                return panel;
            }
            else
            {
                string fileName = ProgramPath;

                ProcessStartInfo processStartInfo = new()
                {
                    FileName = fileName,
                    CreateNoWindow = false,
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Normal,
                    Arguments = null,
                };
                using Process process = Process.Start(processStartInfo);
                string[] exePaths = [.. Directory.EnumerateFiles(ProgramDirectory, "*.exe", SearchOption.AllDirectories)];
                DefaultHostPanel panel = new();

                PWP.PollProcessWindow(exePaths, windowHandle =>
                {
                    Debug.WriteLine($"[PWP] Found window handle: {windowHandle}");

                    panel.Dispatcher.Invoke(() => panel.AttachExternalWindow(windowHandle));
                });

                return panel;
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }

        return null!;
    }
}
