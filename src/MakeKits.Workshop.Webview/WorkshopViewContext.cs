using CommunityToolkit.Mvvm.ComponentModel;

namespace MakeKits.Workshop.Webview;

/// <inheritdoc/>
public abstract class WorkshopViewContext : ObservableObject, IWorkshopViewContext
{
    /// <inheritdoc/>
    public virtual object Source { get; set; } = null!;

    /// <inheritdoc/>
    public virtual string Title { get; set; } = null!;

    /// <inheritdoc/>
    public virtual object ViewerContent { get; set; } = null!;

    /// <inheritdoc/>
    public virtual bool IsBusy { get; set; } = false;

    /// <inheritdoc/>
    public bool IsImmersiveMode { get; set; } = false;

    /// <inheritdoc/>
    public virtual int PreferredWidth { get; set; } = 800;

    /// <inheritdoc/>
    public virtual int PreferredHeight { get; set; } = 600;

    /// <inheritdoc/>
    public virtual bool CanResize { get; set; } = true;

    /// <inheritdoc/>
    public virtual WorkshopTheme Theme { get; set; } = WorkshopTheme.None;
}
