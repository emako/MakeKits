namespace MakeKits.Workshop.Webview.HelloWorld;

public sealed class HelloWorldWorkshop : WebviewWorkshop
{
    public override IWorkshopContext Context { get; set; } = new HelloWorldWorkshopContext();

    protected override WebpagePanel CreatePanel(IWorkshopContext context)
    {
        HelloWorldWorkshopViewContext? viewContext = context.ViewContext as HelloWorldWorkshopViewContext;
        return new HelloWorldWebpagePanel
        {
            Theme = viewContext?.Theme ?? WorkshopTheme.System,
        };
    }

    protected override void ConfigureViewContext(IWorkshopContext context)
    {
        base.ConfigureViewContext(context);

        context.ViewContext?.Title = "MakeKits Hello World";
    }

    protected override void NavigatePanel(WebpagePanel panel, IWorkshopContext context)
    {
        if (panel is HelloWorldWebpagePanel helloWorldPanel)
            helloWorldPanel.NavigateToHomePage();
    }
}
