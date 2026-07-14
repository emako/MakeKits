using MakeKits.Workshop;

[assembly: Workshop("WebviewDebugger")]

namespace MakeKits.Workshop.Webview.Debugger;

public static partial class Configuration
{
    public static string Id { get; set; } = "WebviewDebugger";
    public static string Name { get; set; } = "Debugger Webview";
    public static string Author { get; set; } = "MakeKits";
    public static string Description { get; set; } = "Webview debugger for developer tools.\nAutomatically load from URL 'http://localhost:5173'.";
    public static string Title { get; set; } = "Debugger Webview";
    public static string? Theme { get; set; } = $"{WorkshopTheme.System}";
}
