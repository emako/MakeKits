namespace MakeKits.Workshop.Webview.HelloWorld;

/// <inheritdoc/>
public sealed class HelloWorldWorkshop : WebviewWorkshop
{
    /// <inheritdoc/>
    public override IWorkshopContext Context { get; set; } = new HelloWorldWorkshopContext();

    /// <inheritdoc/>
    protected override WebpagePanel CreatePanel(IWorkshopContext context)
    {
        HelloWorldWorkshopViewContext? viewContext = context.ViewContext as HelloWorldWorkshopViewContext;
        return new HelloWorldWebpagePanel()
        {
            Theme = viewContext?.Theme ?? WorkshopTheme.System,
        };
    }

    /// <inheritdoc/>
    protected override void ConfigureViewContext(IWorkshopContext context)
    {
        base.ConfigureViewContext(context);
    }

    /// <inheritdoc/>
    protected override void NavigatePanel(WebpagePanel panel, IWorkshopContext context)
    {
        if (panel is EmbeddedResourceWebpagePanel panel2)
            panel2.NavigateToHomePage();
    }
}
