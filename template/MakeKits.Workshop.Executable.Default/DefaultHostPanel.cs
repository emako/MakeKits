namespace MakeKits.Workshop.Executable.Default;

public class DefaultHostPanel : WindowHostPanel
{
    private nint _externalHwnd;
    private nint _attachedHwnd;

    public DefaultHostPanel()
    {
        Loaded += (_, _) => TryAttachExternalWindow();
    }

    protected override void OnContainerHandleCreated()
    {
        TryAttachExternalWindow();
    }

    protected override void OnEmbeddedMaximizeRequested()
    {
        if (_attachedHwnd == 0 && _externalHwnd != 0)
            TryAttachExternalWindow();

        if (_attachedHwnd != 0)
            _ = User32.ShowWindow(_attachedHwnd, User32.SW_MAXIMIZE);
    }

    public void AttachExternalWindow(nint externalHwnd)
    {
        _externalHwnd = externalHwnd;
        TryAttachExternalWindow();

        if (_attachedHwnd == 0)
            QueueEmbeddedMaximize();
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
    }
}
