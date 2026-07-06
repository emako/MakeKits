namespace MakeKits.Workshop.Webview.Default;

/// <summary>
/// Default options for the workshop builder.
/// </summary>
internal static class DefaultOption
{
    /// <summary>
    /// Name of the default workshop card.
    /// </summary>
    public static string Name { get; set; } = "Default Webview";

    /// <summary>
    /// Author of the default workshop card.
    /// </summary>
    public static string Author { get; set; } = "MakeKits";

    /// <summary>
    /// Description of the default workshop card.
    /// </summary>
    public static string Description { get; set; } = "Embedded Webview default demo.";

    /// <summary>
    /// Title of the default workshop breadcrumbs.
    /// </summary>
    public static string Title { get; set; } = "Default Webview";

    /// <summary>
    /// Link to DefaultWebpagePanel::Theme
    /// </summary>
    public static string? Theme { get; set; } = $"{WorkshopTheme.Light}";

    /// <summary>
    /// Link to DefaultWebpagePanel::UserDataFolder
    /// Default value is null, which means the default user data folder
    /// will be used `%USERPROFILE%\Documents\MakeKits\Workshop\Webview2_Data\`.
    /// </summary>
    public static string? UserDataFolder { get; set; } = null;
}
