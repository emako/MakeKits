using MakeKits.Workshop.Executable;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace MakeKits.Workshop.Console.Default;

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
            return LaunchType.Console;
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

        if (!File.Exists(PackageMd5Path))
        {
            using Stream? md5Stream = EmbeddedResourceLoader.GetPackageMd5(Assembly.GetExecutingAssembly());
            using StreamReader reader = new(md5Stream,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true,
                bufferSize: 1024,
                leaveOpen: true);
            string md5Source = reader.ReadToEnd();

            File.WriteAllText(PackageMd5Path, md5Source, Encoding.UTF8);
        }

        if (!File.Exists(PackagePath))
        {
            {
                using Stream? packageStream = EmbeddedResourceLoader.GetPackage(Assembly.GetExecutingAssembly());
                using FileStream fileStream = new(PackagePath, FileMode.Create, FileAccess.Write, FileShare.Delete);

                packageStream?.CopyTo(fileStream);
            }

            ZipHelper.ExtractZipToDir(PackagePath, ProgramDirectory, true);

            {
                using Stream? md5Stream = EmbeddedResourceLoader.GetPackageMd5(Assembly.GetExecutingAssembly());
                using StreamReader reader = new(md5Stream,
                    Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: true,
                    bufferSize: 1024,
                    leaveOpen: true);
                string md5Source = reader.ReadToEnd();

                File.WriteAllText(PackageMd5Path, md5Source, Encoding.UTF8);
            }
        }
        else
        {
            using Stream? md5Stream = EmbeddedResourceLoader.GetPackageMd5(Assembly.GetExecutingAssembly());
            using StreamReader reader = new(md5Stream,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true,
                bufferSize: 1024,
                leaveOpen: true);
            string md5Source = reader.ReadToEnd();
            string md5Target = File.ReadAllText(PackageMd5Path);

            if (md5Target != md5Source)
            {
                {
                    using Stream? packageStream = EmbeddedResourceLoader.GetPackage(Assembly.GetExecutingAssembly());
                    using FileStream fileStream = new(PackagePath, FileMode.Create, FileAccess.Write, FileShare.Delete);

                    packageStream?.CopyTo(fileStream);
                }
                {
                    ZipHelper.ExtractZipToDir(PackagePath, ProgramDirectory, true);
                    File.WriteAllText(PackageMd5Path, md5Source);
                }
            }
        }
    }

    /// <inheritdoc/>
    public override void View(IWorkshopContext context)
    {
        try
        {
            Panel = CreatePanel(context);
            NavigatePanel(Panel, context);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            FallbackPanel(e.ToString(), context);
            return;
        }

        context.ViewContext?.ViewerContent = Panel;
    }

    protected override object CreatePanel(IWorkshopContext context)
    {
        return new DefaultHostPanel(Configuration.ExecName);
    }
}
