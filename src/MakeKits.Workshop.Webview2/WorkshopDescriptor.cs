using CommunityToolkit.Mvvm.ComponentModel;

namespace MakeKits.Workshop.Webview2;

public abstract class WorkshopDescriptor : ObservableObject, IWorkshopDescriptor
{
    public virtual string Name => "Workshop Webview2";

    public virtual string? Description => "Support for Webview2 integration";
}
