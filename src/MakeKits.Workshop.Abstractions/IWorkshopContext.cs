using System.Collections.Generic;
using System.ComponentModel;

namespace MakeKits.Workshop;

/// <summary>
/// Runtime context available while a workshop is active.
/// </summary>
/// <remarks>
/// Property changes are surfaced through <see cref="INotifyPropertyChanged"/> so the host
/// can react to updates made by the workshop at runtime.
/// </remarks>
public interface IWorkshopContext : INotifyPropertyChanged
{
    /// <summary>
    /// Gets or sets metadata for the active workshop.
    /// </summary>
    /// <remarks>
    /// The workshop may update this to reflect runtime changes in title, description, or state.
    /// </remarks>
    public IWorkshopDescriptor Descriptor { get; set; }

    /// <summary>
    /// Gets or sets the logger available to the active workshop.
    /// </summary>
    public IWorkshopLogger? Logger { get; set; }

    /// <summary>
    /// Gets or sets view-layer state for the active workshop.
    /// </summary>
    /// <remarks>
    /// The workshop may update this to change title, content, theme, or other view properties
    /// without taking a direct dependency on the host UI framework.
    /// </remarks>
    public IWorkshopViewContext? ViewContext { get; set; }

    /// <summary>
    /// Gets or sets a shared property bag for passing arbitrary data through the workshop pipeline.
    /// </summary>
    public IDictionary<string, object?>? Properties { get; set; }
}
