namespace MakeKits.Workshop.WPF;

/// <inheritdoc/>
public abstract class WorkshopDescriptor : IWorkshopDescriptor
{
    /// <inheritdoc/>
    public virtual string Name => "Workshop WPF";

    /// <inheritdoc/>
    public virtual string? Author => "MakeKits";

    /// <inheritdoc/>
    public virtual string? Description => "Support for WPF Native integration";
}
