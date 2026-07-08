namespace MakeKits.Workshop.Webview.Default;

public sealed class DefaultWorkshopViewContext : WorkshopViewContext
{
    public DefaultWorkshopViewContext()
    {
        Title = Configuration.Title;
        Theme = Enum.TryParse(Configuration.Theme, out WorkshopTheme theme) ? theme : WorkshopTheme.Light;
    }
}
