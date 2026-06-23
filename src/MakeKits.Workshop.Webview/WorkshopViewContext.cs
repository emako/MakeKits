using CommunityToolkit.Mvvm.ComponentModel;

namespace MakeKits.Workshop.Webview;

/// <inheritdoc/>
public abstract partial class WorkshopViewContext : ObservableObject, IWorkshopViewContext
{
    /// <inheritdoc/>
    [ObservableProperty]
    public virtual partial object Source { get; set; } = null!;

    /// <inheritdoc/>
    [ObservableProperty]
    public virtual partial string Title { get; set; } = null!;

    /// <inheritdoc/>
    [ObservableProperty]
    public virtual partial object ViewerContent { get; set; } = null!;

    /// <inheritdoc/>
    [ObservableProperty]
    public virtual partial bool IsBusy { get; set; } = false;

    /// <inheritdoc/>
    [ObservableProperty]
    public virtual partial bool IsImmersiveMode { get; set; } = false;

    /// <inheritdoc/>
    [ObservableProperty]
    public virtual partial int PreferredWidth { get; set; } = 800;

    /// <inheritdoc/>
    [ObservableProperty]
    public virtual partial int PreferredHeight { get; set; } = 600;

    /// <inheritdoc/>
    [ObservableProperty]
    public virtual partial bool CanResize { get; set; } = true;

    /// <inheritdoc/>
    [ObservableProperty]
    public virtual partial WorkshopTheme Theme { get; set; } = WorkshopTheme.None;
}
