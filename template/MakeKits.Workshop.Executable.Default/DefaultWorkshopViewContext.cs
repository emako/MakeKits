namespace MakeKits.Workshop.Executable.Default;

public sealed class DefaultWorkshopViewContext : WorkshopViewContext
{
    public DefaultWorkshopViewContext()
    {
        Title = Configuration.Title;
        Theme = Enum.TryParse(Configuration.Theme, out WorkshopTheme theme) ? theme : WorkshopTheme.System;
    }
}
