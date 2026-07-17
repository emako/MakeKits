namespace MakeKits.Workshop;

/// <summary>
/// Represents an abstract base implementation of <see cref="IWorkshopItem"/>
/// that provides observable properties for workshop item metadata.
/// </summary>
public class WorkshopItem : IWorkshopItem
{
    /// <inheritdoc/>
    public virtual string Name { get; set; } = null!;

    /// <inheritdoc/>
    public virtual string Author { get; set; } = null!;

    /// <inheritdoc/>
    public virtual string Description { get; set; } = null!;

    /// <inheritdoc/>
    public virtual string Version { get; set; } = null!;

    /// <inheritdoc/>
    public virtual string FileName { get; set; } = null!;

    /// <inheritdoc/>
    public virtual string FilePath { get; set; } = null!;

    /// <inheritdoc/>
    public virtual IWorkshop Workshop { get; set; } = null!;
}
