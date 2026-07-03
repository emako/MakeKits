using System.Windows.Threading;

namespace MakeKits.Workshop.Executable.Default;

public class DefaultHostPanel : WindowHostPanel
{
    private nint _externalHwnd;
    private nint _attachedHwnd;

    public DefaultHostPanel()
    {
        Loaded += (_, _) => TryAttachExternalWindow();
        SizeChanged += (_, _) => ResizeExternalWindow();
        LayoutUpdated += (_, _) =>
        {
            if (_attachedHwnd == 0 && _externalHwnd != 0)
                TryAttachExternalWindow();
        };
    }

    protected override void OnContainerHandleCreated()
    {
        TryAttachExternalWindow();
    }

    protected override void OnContainerResized()
    {
        ResizeExternalWindow();
    }

    public void AttachExternalWindow(nint externalHwnd)
    {
        _externalHwnd = externalHwnd;
        TryAttachExternalWindow();

        if (_attachedHwnd == 0)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, TryAttachExternalWindow);
        }
    }

    private void TryAttachExternalWindow()
    {
        if (_externalHwnd == 0 || _attachedHwnd != 0)
            return;

        if (!IsLoaded || ContainerHwnd == 0)
            return;

        SetParent(_externalHwnd, ContainerHwnd);
        _attachedHwnd = _externalHwnd;
        _externalHwnd = 0;
        ResizeExternalWindow();
    }

    private void ResizeExternalWindow()
    {
        if (_attachedHwnd == 0 || ContainerHwnd == 0)
            return;

        ResizeEmbeddedWindow(_attachedHwnd, ContainerHwnd, this);
    }
}
