using System.Windows.Forms.Integration;

namespace MakeKits.Workshop.Executable.Default;

public class DefaultHostPanel : DisposablePanel
{
    public WindowsFormsHost Host = new();

    public DefaultHostPanel()
    {
        Children.Add(Host);
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
    }
}
