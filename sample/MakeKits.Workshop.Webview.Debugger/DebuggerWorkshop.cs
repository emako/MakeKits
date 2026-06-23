namespace MakeKits.Workshop.Webview.Debugger;

/// <inheritdoc/>
public sealed class DebuggerWorkshop : WebviewWorkshop
{
    /// <inheritdoc/>
    public override IWorkshopContext Context { get; set; } = new DebuggerWorkshopContext();

    /// <inheritdoc/>
    protected override WebpagePanel CreatePanel(IWorkshopContext context)
    {
        DebuggerWorkshopViewContext? viewContext = context.ViewContext as DebuggerWorkshopViewContext;
        return new DebuggerWebpagePanel
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
        if (panel is DebuggerWebpagePanel debuggerPanel)
            debuggerPanel.NavigateToHomePage();
    }
}
