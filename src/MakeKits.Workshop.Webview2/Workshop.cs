namespace MakeKits.Workshop.Webview2;

public class Workshop : IWorkshop
{
    public IWorkshopContext Context { get; set; } = new WorkshopContext();

    public IWorkshopDescriptor Descriptor => Context.Descriptor;

    public void Cleanup()
    {
    }

    public void Init()
    {
    }

    public void Prepare(IWorkshopContext context)
    {
    }

    public void View(IWorkshopContext context)
    {
    }
}
