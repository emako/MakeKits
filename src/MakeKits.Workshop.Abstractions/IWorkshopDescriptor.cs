namespace MakeKits.Workshop;

/// <summary>
/// Metadata that identifies and describes a workshop.
/// </summary>
public interface IWorkshopDescriptor
{
    /// <summary>
    /// Gets the display name shown to users.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the author of the workshop.
    /// </summary>
    public string? Author { get; }

    /// <summary>
    /// Gets an optional longer description of the workshop.
    /// </summary>
    public string? Description { get; }
}
