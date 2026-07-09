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
    /// The resource file location.
    /// Support 7z/zip file.
    /// </summary>
    [Category("Webview")]
    public string Resource { get; set; } = "./resource.zip";

    /// <summary>
    /// ???
    /// </summary>
    [Category("Normal")]
    public string Id { get; set; } = "HelloWorld";

    /// <summary>
    /// ???
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
    /// ???
    /// </summary>
    [Category("Normal")]
    public string Name { get; set; } = "MyWorkshop";

    /// <summary>
    /// ???
    /// </summary>
    [Category("Normal")]
    public string Author { get; set; } = "MakeKits";

    /// <summary>
    /// ???
    /// </summary>
    [Category("Normal")]
    public string Description { get; set; } = "I'm a perfect human.";

    /// <summary>
    /// ???
    /// </summary>
    [Category("Normal")]
    public string Title { get; set; } = "My Title";

    /// <summary>
    /// ???
    /// </summary>
    [Category("Normal")]
    public string Theme { get; set; } = "Light";

    /// <summary>
    /// ???
    /// </summary>
    [Category("Webview")]
    public string? UserDataFolder { get; set; } = null;
}
