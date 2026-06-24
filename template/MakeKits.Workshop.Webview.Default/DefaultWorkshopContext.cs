namespace MakeKits.Workshop.Webview.Default;

public sealed class DefaultWorkshopContext : WorkshopContext
{
    public DefaultWorkshopContext()
    {
        Descriptor = new DefaultWorkshopDescriptor();
        ViewContext = new DefaultWorkshopViewContext();
    }
}
