namespace MakeKits.Workshop.Executable.Default;

public sealed class DefaultWorkshopContext : WorkshopContext
{
    public DefaultWorkshopContext()
    {
        Descriptor = new DefaultWorkshopDescriptor();
        ViewContext = new DefaultWorkshopViewContext();
    }
}
