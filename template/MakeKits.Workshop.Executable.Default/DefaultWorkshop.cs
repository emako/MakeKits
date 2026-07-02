namespace MakeKits.Workshop.Executable.Default;

/// <inheritdoc/>
public sealed class DefaultWorkshop : ExecutableWorkshop
{
    /// <inheritdoc/>
    public override IWorkshopContext Context { get; set; } = new DefaultWorkshopContext();

    /// <inheritdoc/>
    protected override DisposablePanel CreatePanel(IWorkshopContext context)
    {
        return null!;
    }
}
