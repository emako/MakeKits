namespace MakeKits.Workshop.Executable.HelloWorld;

/// <inheritdoc/>
public sealed class HelloWorldWorkshop : ExecutableWorkshop
{
    /// <inheritdoc/>
    public override LaunchType LaunchType { get; set; } = LaunchType.Process;

    /// <inheritdoc/>
    public override IWorkshopContext Context { get; set; } = new HelloWorldWorkshopContext();

    /// <inheritdoc/>
    protected override DisposablePanel CreatePanel(IWorkshopContext context)
    {
        HelloWorldWorkshopViewContext? viewContext = context.ViewContext as HelloWorldWorkshopViewContext;
        return new HelloWorldExecutablePanel();
    }

    /// <inheritdoc/>
    protected override void ConfigureViewContext(IWorkshopContext context)
    {
        base.ConfigureViewContext(context);
    }
}
