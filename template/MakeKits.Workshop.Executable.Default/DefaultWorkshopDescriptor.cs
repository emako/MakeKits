namespace MakeKits.Workshop.Executable.Default;

/// <inheritdoc/>
public sealed class DefaultWorkshopDescriptor : WorkshopDescriptor
{
    /// <inheritdoc/>
    public override string Name => Configuration.Name;

    /// <inheritdoc/>
    public override string? Author => Configuration.Author;

    /// <inheritdoc/>
    public override string? Description => Configuration.Description;
}
