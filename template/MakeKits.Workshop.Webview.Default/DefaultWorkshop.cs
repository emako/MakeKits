namespace MakeKits.Workshop.Webview.Default;

/// <inheritdoc/>
public sealed class DefaultWorkshop : WebviewWorkshop
{
    /// <inheritdoc/>
    public override IWorkshopContext Context { get; set; } = new DefaultWorkshopContext();

    /// <inheritdoc/>
    protected override WebpagePanel CreatePanel(IWorkshopContext context)
    {
        DefaultWorkshopViewContext? viewContext = context.ViewContext as DefaultWorkshopViewContext;
        return new DefaultWebpagePanel()
        {
            Theme = viewContext?.Theme ?? WorkshopTheme.Light,
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
        (panel as EmbeddedResourceWebpagePanel)?.NavigateToHomePage();
    }
}
