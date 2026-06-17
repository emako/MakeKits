using CommunityToolkit.Mvvm.ComponentModel;

namespace MakeKits.Workshop.Webview2;

public class WorkshopDescriptor : ObservableObject, IWorkshopDescriptor
{
    public string Name => "Workshop Webview2";

    public string? Description => "Support for Webview2 integration";
}
