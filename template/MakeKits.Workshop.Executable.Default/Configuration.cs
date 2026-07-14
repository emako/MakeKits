using System.Runtime.InteropServices;

[assembly: Guid("00000000-0000-0000-0000-000000000000")]

namespace MakeKits.Workshop.Executable.Default;

/// <summary>
/// Default options for the workshop builder.
/// </summary>
public static partial class Configuration
{
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
}
