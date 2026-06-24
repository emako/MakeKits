using System.Collections.Generic;

namespace MakeKits.Workshop.Webview;

/// <inheritdoc/>
public abstract class WorkshopContext : ObservableObject, IWorkshopContext
{
    private IWorkshopDescriptor _descriptor = null!;
    private IWorkshopLogger? _logger;
    private IWorkshopViewContext? _viewContext;
    private IDictionary<string, object?>? _properties = new Dictionary<string, object?>();

    /// <inheritdoc/>
    public virtual IWorkshopDescriptor Descriptor
    {
        get => _descriptor;
        set => SetProperty(ref _descriptor, value);
    }

    /// <inheritdoc/>
    public virtual IWorkshopLogger? Logger
    {
        get => _logger;
        set => SetProperty(ref _logger, value);
    }

    /// <inheritdoc/>
    public virtual IWorkshopViewContext? ViewContext
    {
        get => _viewContext;
        set => SetProperty(ref _viewContext, value);
    }

    /// <inheritdoc/>
    public virtual IDictionary<string, object?>? Properties
    {
        get => _properties;
        set => SetProperty(ref _properties, value);
    }
}
