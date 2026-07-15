using MakeKits.Workshop;
using System.Runtime.InteropServices;

[assembly: Guid("00000000-0000-0000-0000-000000000000")]
[assembly: Workshop("HelloWorld")]

namespace MakeKits.Workshop.Console.Default;

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
    public static string Name { get; set; } = "Default Console";

    /// <summary>
    /// Author of the default workshop card.
    /// </summary>
    public static string Author { get; set; } = "MakeKits";

    /// <summary>
    /// Description of the default workshop card.
    /// </summary>
    public static string Description { get; set; } = "Embedded Console default demo.";

    /// <summary>
    /// Title of the default workshop breadcrumbs.
    /// </summary>
    public static string Title { get; set; } = "Default Console";

    /// <summary>
    /// Link to DefaultWebpagePanel::Theme
    /// </summary>
    public static string? Theme { get; set; } = $"{WorkshopTheme.Dark}";

    /// <summary>
    /// Launch type for the embedded executable.
    /// </summary>
    public static string LaunchType { get; set; } = $"{Executable.LaunchType.Console}";

    /// <summary>
    /// File name of the executable to launch.
    /// </summary>
    public static string ExecName { get; set; } = "powershell.exe";
}
