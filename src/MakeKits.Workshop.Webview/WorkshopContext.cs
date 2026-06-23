using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace MakeKits.Workshop.Webview;

/// <inheritdoc/>
public abstract partial class WorkshopContext : ObservableObject, IWorkshopContext
{
    /// <inheritdoc/>
    [ObservableProperty]
    public virtual partial IWorkshopDescriptor Descriptor { get; set; } = null!;

    /// <inheritdoc/>
    [ObservableProperty]
    public virtual partial IWorkshopLogger? Logger { get; set; } = null!;

    /// <inheritdoc/>
    [ObservableProperty]
    public virtual partial IWorkshopViewContext? ViewContext { get; set; } = null!;

    /// <inheritdoc/>
    [ObservableProperty]
    public virtual partial IDictionary<string, object?>? Properties { get; set; } = new Dictionary<string, object?>();
}
