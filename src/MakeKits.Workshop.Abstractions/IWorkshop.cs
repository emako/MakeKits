namespace MakeKits.Workshop;

/// <summary>
/// Core workshop contract that defines the lifecycle of a host-hosted feature module.
/// </summary>
/// <remarks>
/// The host invokes methods in order: <see cref="Init"/>, <see cref="Prepare"/>,
/// <see cref="View"/>, and finally <see cref="Cleanup"/> when the workshop is torn down.
/// </remarks>
public interface IWorkshop
{
    /// <summary>
    /// Gets or sets the active runtime context for this workshop.
    /// </summary>
    /// <remarks>
    /// The host assigns this before invoking any lifecycle method. The same instance is
    /// typically passed to <see cref="Prepare"/> and <see cref="View"/>.
    /// </remarks>
    public IWorkshopContext Context { get; set; }

    /// <summary>
    /// Gets metadata that identifies and describes this workshop.
    /// </summary>
    public IWorkshopDescriptor Descriptor { get; }

    /// <summary>
    /// Performs one-time initialization when the application starts.
    /// </summary>
    /// <remarks>
    /// Use this method to load resources that can be shared across multiple invocations.
    /// </remarks>
    public void Init();

    /// <summary>
    /// Prepares the workshop before its view is shown.
    /// </summary>
    /// <param name="context">Runtime context that exposes host services and shared state.</param>
    /// <remarks>
    /// Keep this method lightweight. Avoid long-running or blocking work here.
    /// </remarks>
    public void Prepare(IWorkshopContext context);

    /// <summary>
    /// Loads content and drives presentation in the host view.
    /// </summary>
    /// <param name="context">Runtime context that exposes host services and shared state.</param>
    /// <remarks>
    /// The host may display a busy indicator until this method completes its initial work.
    /// </remarks>
    public void View(IWorkshopContext context);

    /// <summary>
    /// Releases resources held by this workshop.
    /// </summary>
    /// <remarks>
    /// Called when the workshop is no longer needed. Prefer releasing unmanaged resources here.
    /// </remarks>
    public void Cleanup();
}
