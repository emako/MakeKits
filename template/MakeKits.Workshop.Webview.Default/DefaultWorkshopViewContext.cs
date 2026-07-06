namespace MakeKits.Workshop.Webview.Default;

public sealed class DefaultWorkshopViewContext : WorkshopViewContext
{
    public DefaultWorkshopViewContext()
    {
        Title = DefaultOption.Title;
        Theme = Enum.TryParse(DefaultOption.Theme, out WorkshopTheme theme) ? theme : WorkshopTheme.Light;
    }
}
