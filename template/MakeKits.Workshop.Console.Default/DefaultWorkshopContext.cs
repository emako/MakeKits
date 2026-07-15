using MakeKits.Workshop.Executable;

namespace MakeKits.Workshop.Console.Default;

public sealed class DefaultWorkshopContext : WorkshopContext
{
    public DefaultWorkshopContext()
    {
        Descriptor = new DefaultWorkshopDescriptor();
        ViewContext = new DefaultWorkshopViewContext();
    }
}
