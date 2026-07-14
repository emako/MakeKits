using System.ComponentModel;

namespace MakeKits.Cli;

public sealed class Configuration
{
    /// <summary>
    /// The minimal required version of makemica tool.
    /// When set, makemica will exit with error if its version is lower than this value.
    /// When null, no version check is performed.
    /// </summary>
    [Category("Normal")]
    public string? MinimalVersion { get; set; } = null;

    /// <summary>
    /// The version of workshop plugin assembly.
    /// </summary>
    [Category("Normal")]
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// The template file location.
    /// When `.dist` dir is empty, extract it to `.dist`.
    /// Support 7z/zip template.
    /// </summary>
    [Category("Normal")]
    public string Template { get; set; } = "${KitsDir}/template/default.7z";

    /// <summary>
    /// The package file location.
    /// Support only zip file.
    /// </summary>
    [Category("Normal")]
    public string Package { get; set; } = "./package.zip";

    /// <summary>
    /// The package directory.
    /// Support only zip file.
    /// </summary>
    [Category("Normal")]
    public string PackageDirectory { get; set; } = "./build";

    /// <summary>
    /// The resource file location.
    /// Support 7z/zip file.
    /// </summary>
    [Category("Webview")]
    public string Resource { get; set; } = "./resource.zip";

    /// <summary>
    /// The resource directory.
    /// Support 7z/zip file.
    /// </summary>
    [Category("Webview")]
    public string ResourceDirectory { get; set; } = "./dist";

    /// <summary>
    /// The unique identifier for the workshop plugin.
    /// </summary>
    [Category("Normal")]
    public string Id { get; set; } = "HelloWorld";

    /// <summary>
    /// The assembly name of the workshop plugin.
    /// </summary>
    [Category("Normal")]
    public string AssemblyName { get; set; } = "MakeKits.Workshop.${Id}";

    /// <summary>
    /// The output setup file location.
    /// </summary>
    [Category("Normal")]
    public string Output { get; set; } = "./${AssemblyName}.dll";

    /// <summary>
    /// .NET Framework version.
    /// </summary>
    [Category("Normal")]
    public string TargetFramework { get; set; } = "net48";

    /// <summary>
    /// Assembly GUID for the setup.
    /// </summary>
    [Category("Normal")]
    public string Guid { get; set; } = "00000000-0000-0000-0000-000000000000";

    /// <summary>
    /// The display name of the workshop.
    /// </summary>
    [Category("Normal")]
    public string Name { get; set; } = "My Workshop";

    /// <summary>
    /// The author of the workshop.
    /// </summary>
    [Category("Normal")]
    public string Author { get; set; } = "MakeKits";

    /// <summary>
    /// The description of the workshop.
    /// </summary>
    [Category("Normal")]
    public string Description { get; set; } = "I'm a perfect human.";

    /// <summary>
    /// The title of the workshop window.
    /// </summary>
    [Category("Normal")]
    public string Title { get; set; } = "My Title";

    /// <summary>
    /// The UI theme of the workshop (Light or Dark).
    /// </summary>
    [Category("Normal")]
    public string Theme { get; set; } = "Light";

    /// <summary>
    /// The folder path for persistent user data storage.
    /// When null, use default location.
    /// </summary>
    [Category("Webview")]
    public string? UserDataFolder { get; set; } = null;

    /// <summary>
    /// Launch type for the embedded executable.
    /// </summary>
    [Category("Executable")]
    public string LaunchType { get; set; } = "Process";

    /// <summary>
    /// File name of the executable to launch.
    /// </summary>
    [Category("Executable")]
    public string ExecName { get; set; } = "start.ps1";

    /// <summary>
    /// Width offset applied when resizing the embedded window.
    /// </summary>
    [Category("Executable")]
    public int ResizeOffsetWidth { get; set; } = 0;

    /// <summary>
    /// Height offset applied when resizing the embedded window.
    /// </summary>
    [Category("Executable")]
    public int ResizeOffsetHeight { get; set; } = 0;
}
