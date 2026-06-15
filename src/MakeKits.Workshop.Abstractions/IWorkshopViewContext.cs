using System.ComponentModel;
using System.Drawing;

namespace MakeKits.Workshop;

/// <summary>
/// View-layer context supplied by the host while a workshop is displayed.
/// </summary>
/// <remarks>
/// Workshops update these properties to drive presentation without referencing host UI types
/// directly. Property changes are surfaced through <see cref="INotifyPropertyChanged"/>.
/// </remarks>
public interface IWorkshopViewContext : INotifyPropertyChanged
{
    /// <summary>
    /// Gets or sets the host window instance that contains the workshop view.
    /// </summary>
    public object Source { get; set; }

    /// <summary>
    /// Gets or sets the window title shown while the workshop is active.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the root content object rendered in the view.
    /// </summary>
    public object ViewerContent { get; set; }

    /// <summary>
    /// Gets or sets whether the host should display a busy indicator.
    /// </summary>
    public bool IsBusy { get; set; }

    /// <summary>
    /// Gets or sets the preferred initial size of the view.
    /// </summary>
    public Size PreferredSize { get; set; }

    /// <summary>
    /// Gets or sets whether the user can resize the view.
    /// </summary>
    public bool CanResize { get; set; }

    /// <summary>
    /// Gets or sets the visual theme applied to the view.
    /// </summary>
    public WorkshopTheme Theme { get; set; }
}
