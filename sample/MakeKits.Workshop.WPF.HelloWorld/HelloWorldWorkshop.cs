namespace MakeKits.Workshop.WPF.HelloWorld;

/// <inheritdoc/>
public sealed class HelloWorldWorkshop : WebviewWorkshop
{
    /// <inheritdoc/>
    public override IWorkshopContext Context { get; set; } = new HelloWorldWorkshopContext();

    /// <inheritdoc/>
    protected override object CreatePanel(IWorkshopContext context)
    {
        HelloWorldWorkshopViewContext? viewContext = context.ViewContext as HelloWorldWorkshopViewContext;
        return new HelloWorldWpfPanel(viewContext?.Theme ?? WorkshopTheme.System);
    }

    /// <inheritdoc/>
    protected override void ConfigureViewContext(IWorkshopContext context)
    {
        base.ConfigureViewContext(context);
    }
}
