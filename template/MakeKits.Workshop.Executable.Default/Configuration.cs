using MakeKits.Workshop;
using System.Runtime.InteropServices;

[assembly: Guid("00000000-0000-0000-0000-000000000000")]
[assembly: Workshop("HelloWorld")]

namespace MakeKits.Workshop.Executable.Default;

/// <summary>
/// Default options for the workshop builder.
/// </summary>
public static partial class Configuration
{
    /// <summary>
    /// The unique identifier for the workshop plugin.
    /// </summary>
    public static string Id { get; set; } = "HelloWorld";

    /// <summary>
    /// Name of the default workshop card.
    /// </summary>
    public static string Name { get; set; } = "Default Executable";

    /// <summary>
    /// Author of the default workshop card.
    /// </summary>
    public static string Author { get; set; } = "MakeKits";

    /// <summary>
    /// Description of the default workshop card.
    /// </summary>
    public static string Description { get; set; } = "Embedded Executable default demo.";

    /// <summary>
    /// Title of the default workshop breadcrumbs.
    /// </summary>
    public static string Title { get; set; } = "Default Executable";

    /// <summary>
    /// Link to DefaultWebpagePanel::Theme
    /// </summary>
    public static string? Theme { get; set; } = $"{WorkshopTheme.Light}";

    /// <summary>
    /// Launch type for the embedded executable.
    /// </summary>
    public static string LaunchType { get; set; } = $"{Executable.LaunchType.Process}";

    /// <summary>
    /// File name of the executable to launch.
    /// </summary>
    public static string ExecName { get; set; } = "start.ps1";

    /// <summary>
    /// Width offset applied when resizing the embedded window.
    /// </summary>
    public static int ResizeOffsetWidth { get; set; } = 0;

    /// <summary>
    /// Height offset applied when resizing the embedded window.
    /// </summary>
    public static int ResizeOffsetHeight { get; set; } = 0;
}
