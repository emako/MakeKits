using System.Windows.Controls;

namespace MakeKits.Workshop.Executable.HelloWorld;

public sealed class HelloWorldExecutablePanel : DisposablePanel
{
    public string Path { get; set; } = null!;

    public HelloWorldExecutablePanel()
    {
    }

    public override void Dispose()
    {
    }
}
