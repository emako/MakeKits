namespace MakeKits.Workshop.Webview;

/// <inheritdoc/>
public abstract class Workshop : IWorkshop
{
    /// <inheritdoc/>
    public virtual IWorkshopContext Context { get; set; } = null!;

    /// <inheritdoc/>
    public virtual IWorkshopDescriptor Descriptor => Context.Descriptor;

    /// <inheritdoc/>
    public abstract void Cleanup();

    /// <inheritdoc/>
    public abstract void Init();

    /// <inheritdoc/>
    public abstract void Prepare(IWorkshopContext context);

    /// <inheritdoc/>
    public abstract void View(IWorkshopContext context);
}
