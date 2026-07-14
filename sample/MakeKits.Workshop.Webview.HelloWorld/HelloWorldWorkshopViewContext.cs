namespace MakeKits.Workshop.Webview.HelloWorld;

public sealed class HelloWorldWorkshopViewContext : WorkshopViewContext
{
    public HelloWorldWorkshopViewContext()
    {
        Title = Configuration.Title;
        Theme = Enum.TryParse(Configuration.Theme, out WorkshopTheme theme) ? theme : WorkshopTheme.System;
    }
}
