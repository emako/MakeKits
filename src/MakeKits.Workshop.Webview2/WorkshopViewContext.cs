using CommunityToolkit.Mvvm.ComponentModel;

namespace MakeKits.Workshop.Webview2;

public class WorkshopViewContext : ObservableObject, IWorkshopViewContext
{
    public object Source { get; set; } = null!;

    public string Title { get; set; } = null!;

    public object ViewerContent { get; set; } = null!;

    public bool IsBusy { get; set; } = false;

    public bool IsImmersiveMode { get; set; } = false;

    public int PreferredWidth { get; set; } = 800;

    public int PreferredHeight { get; set; } = 600;

    public bool CanResize { get; set; } = true;

    public WorkshopTheme Theme { get; set; } = WorkshopTheme.None;
}
