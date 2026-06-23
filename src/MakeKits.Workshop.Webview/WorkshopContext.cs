using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace MakeKits.Workshop.Webview;

/// <inheritdoc/>
public abstract class WorkshopContext : ObservableObject, IWorkshopContext
{
    /// <inheritdoc/>
    public virtual IWorkshopDescriptor Descriptor { get; set; } = null!;

    /// <inheritdoc/>
    public virtual IWorkshopLogger? Logger { get; set; } = null!;

    /// <inheritdoc/>
    public virtual IWorkshopViewContext? ViewContext { get; set; } = null!;

    /// <inheritdoc/>
    public virtual IDictionary<string, object?>? Properties { get; set; } = new Dictionary<string, object?>();
}
