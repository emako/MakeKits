using MakeKits.Workshop.Executable;

namespace MakeKits.Workshop.Console.Default;

public sealed class DefaultWorkshopViewContext : WorkshopViewContext
{
    public DefaultWorkshopViewContext()
    {
        Title = Configuration.Title;
        Theme = Enum.TryParse(Configuration.Theme, out WorkshopTheme theme) ? theme : WorkshopTheme.System;
    }
}
