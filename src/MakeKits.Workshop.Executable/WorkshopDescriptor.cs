namespace MakeKits.Workshop.Executable;

/// <inheritdoc/>
public abstract class WorkshopDescriptor : IWorkshopDescriptor
{
    /// <inheritdoc/>
    public virtual string Name => "Workshop Executable";

    /// <inheritdoc/>
    public virtual string? Author => "MakeKits";

    /// <inheritdoc/>
    public virtual string? Description => "Support for Executable Program";
}
