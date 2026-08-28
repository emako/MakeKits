namespace WhatsShop;

internal sealed class WorkshopMetaInfo
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Error messages on reading failure;
    /// If there are no errors, it is an empty string.
    /// </summary>
    public string Error { get; set; } = string.Empty;
}
