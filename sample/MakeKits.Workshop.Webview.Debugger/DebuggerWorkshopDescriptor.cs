namespace MakeKits.Workshop.Webview.Debugger;

/// <inheritdoc/>
public sealed class DebuggerWorkshopDescriptor : WorkshopDescriptor
{
    /// <inheritdoc/>
    public override string Name => "Debugger Webview";

    /// <inheritdoc/>
    public override string? Author => "MakeKits";

    /// <inheritdoc/>
    public override string? Description => "Webview debugger for developer tools.\nAutomatically load from URL 'http://localhost:5173'.";
}
