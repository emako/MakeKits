using System.ComponentModel;

namespace MakeKits.Workshop;

/// <summary>
/// Metadata that identifies and describes a workshop.
/// </summary>
/// <remarks>
/// Property changes are surfaced through <see cref="INotifyPropertyChanged"/> so the host
/// can refresh UI when workshop metadata changes at runtime.
/// </remarks>
public interface IWorkshopDescriptor : INotifyPropertyChanged
{
    /// <summary>
    /// Gets the display name shown to users.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets an optional longer description of the workshop.
    /// </summary>
    public string? Description { get; }
}
