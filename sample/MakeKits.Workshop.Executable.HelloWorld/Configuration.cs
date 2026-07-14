using MakeKits.Workshop;
using System.Runtime.InteropServices;

[assembly: Guid("00000000-0000-0000-0000-000000000000")]
[assembly: Workshop("ExecutableHelloWorld")]

namespace MakeKits.Workshop.Executable.HelloWorld;

public static partial class Configuration
{
    public static string Id { get; set; } = "ExecutableHelloWorld";

    public static string Name { get; set; } = "Hello World Executable";

    public static string Author { get; set; } = "MakeKits";

    public static string Description { get; set; } = "Embedded Executable hello world demo.";

    public static string Title { get; set; } = "Hello World";

    public static string? Theme { get; set; } = $"{WorkshopTheme.System}";

    public static string LaunchType { get; set; } = $"{Executable.LaunchType.Process}";

    public static string ExecName { get; set; } = "ModernWinver.exe";

    public static int ResizeOffsetWidth { get; set; } = 0;

    public static int ResizeOffsetHeight { get; set; } = 0;
}
