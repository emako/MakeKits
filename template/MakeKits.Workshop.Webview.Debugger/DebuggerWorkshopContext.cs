namespace MakeKits.Workshop.Webview.Debugger;

public sealed class DebuggerWorkshopContext : WorkshopContext
{
    public DebuggerWorkshopContext()
    {
        Descriptor = new DebuggerWorkshopDescriptor();
        ViewContext = new DebuggerWorkshopViewContext();
    }
}
