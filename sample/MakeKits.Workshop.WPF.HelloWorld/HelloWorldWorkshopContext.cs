using System.ComponentModel;

namespace MakeKits.Workshop.WPF.HelloWorld;

public sealed class HelloWorldWorkshopContext : WorkshopObject, IWorkshopContext
{
    private IWorkshopDescriptor _descriptor = null!;
    private IWorkshopLogger? _logger;
    private IWorkshopViewContext? _viewContext;
    private IDictionary<string, object?>? _properties = new Dictionary<string, object?>();

    /// <inheritdoc/>
    public IWorkshopDescriptor Descriptor
    {
        get => _descriptor;
        set => SetProperty(ref _descriptor, value);
    }

    /// <inheritdoc/>
    public IWorkshopLogger? Logger
    {
        get => _logger;
        set => SetProperty(ref _logger, value);
    }

    /// <inheritdoc/>
    public IWorkshopViewContext? ViewContext
    {
        get => _viewContext;
        set => SetProperty(ref _viewContext, value);
    }

    /// <inheritdoc/>
    public IDictionary<string, object?>? Properties
    {
        get => _properties;
        set => SetProperty(ref _properties, value);
    }

    public HelloWorldWorkshopContext()
    {
        Descriptor = new HelloWorldWorkshopDescriptor();
        ViewContext = new HelloWorldWorkshopViewContext();
    }
}
