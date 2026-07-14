using MakeKits.Workshop;

[assembly: Workshop("ExecutableHelloWorld")]

namespace MakeKits.Workshop.Executable.HelloWorld;

public static partial class Configuration
{
    public static string Id { get; set; } = "ExecutableHelloWorld";
    public static string Name { get; set; } = "Hello World Webview";
    public static string Author { get; set; } = "MakeKits";
    public static string Description { get; set; } = "Embedded Webview hello world demo.";
    public static string Title { get; set; } = "Hello World";
    public static string? Theme { get; set; } = $"{WorkshopTheme.System}";
    public static string LaunchType { get; set; } = $"{Executable.LaunchType.Process}";
    public static string ExecName { get; set; } = "start.ps1";
}
