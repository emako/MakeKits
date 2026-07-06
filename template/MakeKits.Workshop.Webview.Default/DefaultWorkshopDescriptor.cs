namespace MakeKits.Workshop.Webview.Default;

/// <inheritdoc/>
public sealed class DefaultWorkshopDescriptor : WorkshopDescriptor
{
    /// <inheritdoc/>
    public override string Name => DefaultOption.Name;

    /// <inheritdoc/>
    public override string? Author => DefaultOption.Author;

    /// <inheritdoc/>
    public override string? Description => DefaultOption.Description;
}
