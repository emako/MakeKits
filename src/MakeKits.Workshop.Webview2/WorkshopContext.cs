using CommunityToolkit.Mvvm.ComponentModel;

namespace MakeKits.Workshop.Webview2;

public class WorkshopContext : ObservableObject, IWorkshopContext
{
    public IWorkshopDescriptor Descriptor { get; set; } = new WorkshopDescriptor();

    public IWorkshopLogger? Logger { get; set; } = new WorkshopLogger();

    public IWorkshopViewContext? ViewContext { get; set; } = new WorkshopViewContext();

    public IDictionary<string, object?>? Properties { get; set; } = new Dictionary<string, object?>();
}
