namespace MakeKits.Workshop.Webview.Debugger;

public sealed class DebuggerWorkshopDescriptor : WorkshopDescriptor
{
    public override string Name => Configuration.Name;

    public override string? Author => Configuration.Author;

    public override string? Description => Configuration.Description;
}
