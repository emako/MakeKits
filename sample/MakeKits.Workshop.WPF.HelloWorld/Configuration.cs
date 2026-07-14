using MakeKits.Workshop;

namespace MakeKits.Workshop.WPF.HelloWorld;

public static partial class Configuration
{
    public static string Id { get; set; } = "WpfHelloWorld";
    public static string Name { get; set; } = "Hello World WPF";
    public static string Author { get; set; } = "MakeKits";
    public static string Description { get; set; } = "Native WPF hello world demo.";
    public static string Title { get; set; } = "Hello World";
    public static string? Theme { get; set; } = $"{WorkshopTheme.System}";
}
