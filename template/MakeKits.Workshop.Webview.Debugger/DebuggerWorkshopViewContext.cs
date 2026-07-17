namespace MakeKits.Workshop.Webview.Debugger;

public sealed class DebuggerWorkshopViewContext : WorkshopViewContext
{
    public DebuggerWorkshopViewContext()
    {
        Title = Configuration.Title;
        Theme = Enum.TryParse(Configuration.Theme, out WorkshopTheme theme) ? theme : WorkshopTheme.System;
    }
}
