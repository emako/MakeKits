using MakeKits.Workshop;
using System.Runtime.InteropServices;

[assembly: Guid("00000000-0000-0000-0000-000000000000")]
[assembly: Workshop("WPFHelloWorld")]

namespace MakeKits.Workshop.WPF.HelloWorld;

public static partial class Configuration
{
    public static string Id { get; set; } = "HelloWorldWPF";

    public static string Name { get; set; } = "Hello WPF";

    public static string Author { get; set; } = "MakeKits";

    public static string Description { get; set; } = "Native WPF hello world demo.";

    public static string Title { get; set; } = "Hello World";

    public static string? Theme { get; set; } = $"{WorkshopTheme.System}";
}
