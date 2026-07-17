namespace MakeKits.Workshop.WPF.HelloWorld;

/// <inheritdoc/>
public sealed class HelloWorldWorkshop : IWorkshop
{
    /// <inheritdoc/>
    public IWorkshopContext Context { get; set; } = new HelloWorldWorkshopContext();

    /// <inheritdoc/>
    public IWorkshopDescriptor Descriptor => Context.Descriptor;

    /// <inheritdoc/>
    public void Init()
    {
    }

    /// <inheritdoc/>
    public void Prepare(IWorkshopContext context)
    {
        Context = context;
    }

    /// <inheritdoc/>
    public void View(IWorkshopContext context)
    {
        context.ViewContext?.ViewerContent = new HelloWorldWpfPanel(WorkshopTheme.System);
    }

    /// <inheritdoc/>
    public void Cleanup()
    {
    }
}
