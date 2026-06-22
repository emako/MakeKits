using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace MakeKits.Workshop.Webview;

public abstract class WorkshopContext : ObservableObject, IWorkshopContext
{
    public virtual IWorkshopDescriptor Descriptor { get; set; } = null!;

    public virtual IWorkshopLogger? Logger { get; set; } = null!;

    public virtual IWorkshopViewContext? ViewContext { get; set; } = null!;

    public virtual IDictionary<string, object?>? Properties { get; set; } = new Dictionary<string, object?>();
}
