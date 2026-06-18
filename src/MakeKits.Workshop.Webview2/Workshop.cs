namespace MakeKits.Workshop.Webview2;

public abstract class Workshop : IWorkshop
{
    public virtual IWorkshopContext Context { get; set; } = null!;

    public virtual IWorkshopDescriptor Descriptor => Context.Descriptor;

    public abstract void Cleanup();

    public abstract void Init();

    public abstract void Prepare(IWorkshopContext context);

    public abstract void View(IWorkshopContext context);
}
