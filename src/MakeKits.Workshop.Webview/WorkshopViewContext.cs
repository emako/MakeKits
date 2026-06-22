using CommunityToolkit.Mvvm.ComponentModel;

namespace MakeKits.Workshop.Webview;

public abstract class WorkshopViewContext : ObservableObject, IWorkshopViewContext
{
    public virtual object Source { get; set; } = null!;

    public virtual string Title { get; set; } = null!;

    public virtual object ViewerContent { get; set; } = null!;

    public virtual bool IsBusy { get; set; } = false;

    public bool IsImmersiveMode { get; set; } = false;

    public virtual int PreferredWidth { get; set; } = 800;

    public virtual int PreferredHeight { get; set; } = 600;

    public virtual bool CanResize { get; set; } = true;

    public virtual WorkshopTheme Theme { get; set; } = WorkshopTheme.None;
}
