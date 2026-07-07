namespace MakeKits.Cli;

public class Makeup
{
    /// <summary>
    /// The minimal required version of makemica tool.
    /// When set, makemica will exit with error if its version is lower than this value.
    /// When null, no version check is performed.
    /// </summary>
    public string? MinimalVersion { get; set; } = null;

    /// <summary>
    /// The template file location.
    /// When `.dist` dir is empty, extract it to `.dist`.
    /// Support 7z/zip template.
    /// </summary>
    public string Template { get; set; } = "${KitsDir}/template/default.7z";

    /// <summary>
    /// The package file location.
    /// Support 7z/zip file.
    /// </summary>
    public string Package { get; set; } = "./publish.7z";

    /// <summary>
    /// The output setup file location.
    /// </summary>
    public string Output { get; set; } = "./${AppName}Setup_v${Version}.exe";

    /// <summary>
    /// Assembly GUID for the setup.
    /// </summary>
    public string Guid { get; set; } = "00000000-0000-0000-0000-000000000000";

    /// <summary>
    /// ???
    /// </summary>
    public string Name { get; set; } = "WorkshopPlugin";

    /// <summary>
    /// ???
    /// </summary>
    public string Author { get; set; } = "MakeKits";

    /// <summary>
    /// ???
    /// </summary>
    public string Description { get; set; } = "I am a perfect human.";

    /// <summary>
    /// ???
    /// </summary>
    public string Title { get; set; } = "Workshop Title";

    /// <summary>
    /// ???
    /// </summary>
    public string Theme { get; set; } = "Light";

    /// <summary>
    /// ???
    /// </summary>
    public string? UserDataFolder { get; set; } = null;
}
