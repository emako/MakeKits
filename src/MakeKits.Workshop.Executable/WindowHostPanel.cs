using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Threading;

namespace MakeKits.Workshop.Executable;

/// <summary>
/// Abstract base panel interface class, inherit WPF Panel and implement resource release logic
/// </summary>
public abstract class WindowHostPanel : WindowsFormsHost, IDisposable
{
    private bool _maximizeQueued;

    /// <summary>
    /// WinForms container used as the native parent for embedded external windows.
    /// </summary>
    protected System.Windows.Forms.Panel Container { get; }

    protected WindowHostPanel()
    {
        Container = new System.Windows.Forms.Panel
        {
            Dock = System.Windows.Forms.DockStyle.Fill,
            BackColor = System.Drawing.Color.FromArgb(26, 26, 46),
        };
        Child = Container;
        HorizontalAlignment = HorizontalAlignment.Stretch;
        VerticalAlignment = VerticalAlignment.Stretch;
        Background = System.Windows.Media.Brushes.Transparent;

        Container.HandleCreated += (_, _) => OnContainerHandleCreated();
        Loaded += OnHostLoaded;
    }

    private void OnHostLoaded(object sender, RoutedEventArgs e)
    {
        if (Window.GetWindow(this) is Window window)
            window.DpiChanged += OnWindowDpiChanged;

        UpdateWindowPos();
        QueueEmbeddedMaximize();
    }

    private void OnWindowDpiChanged(object sender, DpiChangedEventArgs e)
    {
        UpdateWindowPos();
        QueueEmbeddedMaximize();
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);

        if (!sizeInfo.WidthChanged && !sizeInfo.HeightChanged)
            return;

        UpdateWindowPos();
        QueueEmbeddedMaximize();
    }

    protected virtual void OnContainerHandleCreated()
    {
    }

    /// <summary>
    /// Native handle of the WinForms container, or zero when not yet created.
    /// </summary>
    protected nint ContainerHwnd =>
        Container.IsHandleCreated ? Container.Handle : 0;

    /// <summary>
    /// Coalesce maximize requests to one dispatcher callback per layout pass.
    /// </summary>
    protected void QueueEmbeddedMaximize()
    {
        if (_maximizeQueued || !IsLoaded)
            return;

        _maximizeQueued = true;
        _ = Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
        {
            _maximizeQueued = false;
            OnEmbeddedMaximizeRequested();
        });
    }

    /// <summary>
    /// Called after host layout changes; override to maximize the embedded native window.
    /// </summary>
    protected virtual void OnEmbeddedMaximizeRequested()
    {
    }

    /// <summary>
    /// Set the parent window of an external window to the host window
    /// </summary>
    /// <param name="externalHwnd">The handle of the external window</param>
    /// <param name="hostHwnd">The handle of the host window</param>
    /// <returns>The handle of the previous parent window</returns>
    public virtual nint SetParent(nint externalHwnd, nint hostHwnd)
    {
        if (externalHwnd == 0 || hostHwnd == 0)
            return 0;

        nint previousParent = User32.SetParent(externalHwnd, hostHwnd);

        uint style = User32.GetWindowLong(externalHwnd, User32.GWL_STYLE);
        style &= ~(User32.WS_POPUP | User32.WS_CAPTION | User32.WS_THICKFRAME | User32.WS_MINIMIZEBOX | User32.WS_MAXIMIZEBOX | User32.WS_SYSMENU | User32.WS_MINIMIZE);
        style |= User32.WS_CHILD | User32.WS_VISIBLE;
        _ = User32.SetWindowLong(externalHwnd, User32.GWL_STYLE, style);

        uint exStyle = User32.GetWindowLong(externalHwnd, User32.GWL_EXSTYLE);
        exStyle &= ~User32.WS_EX_APPWINDOW;
        _ = User32.SetWindowLong(externalHwnd, User32.GWL_EXSTYLE, exStyle);

        UpdateWindowPos();
        PresentEmbeddedWindow(externalHwnd);

        return previousParent;
    }

    /// <summary>
    /// Show the embedded window and maximize it within the host container.
    /// </summary>
    protected static void PresentEmbeddedWindow(nint externalHwnd)
    {
        if (externalHwnd == 0)
            return;

        _ = User32.ShowWindow(externalHwnd, User32.SW_SHOW);
        _ = User32.ShowWindow(externalHwnd, User32.SW_MAXIMIZE);
        _ = User32.UpdateWindow(externalHwnd);
    }

    protected void MaximizeEmbeddedWindow(nint externalHwnd)
    {
        if (externalHwnd == 0)
            return;

        _ = User32.ShowWindow(externalHwnd, User32.SW_MAXIMIZE);
    }

    /// <inheritdoc />
    public new virtual void Dispose()
    {
        base.Dispose();
    }

    protected internal static class User32
    {
        [DllImport("user32.dll")]
        public static extern nint SetParent(nint hWndChild, nint hWndNewParent);

        [DllImport("user32.dll")]
        public static extern uint GetWindowLong(nint hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(nint hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(nint hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool UpdateWindow(nint hWnd);

        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;
        public const int SW_SHOW = 5;
        public const int SW_MAXIMIZE = 3;
        public const uint WS_CHILD = 0x40000000;
        public const uint WS_VISIBLE = 0x10000000;
        public const uint WS_MINIMIZE = 0x20000000;
        public const uint WS_POPUP = 0x80000000;
        public const uint WS_CAPTION = 0x00C00000;
        public const uint WS_THICKFRAME = 0x00040000;
        public const uint WS_MINIMIZEBOX = 0x00020000;
        public const uint WS_MAXIMIZEBOX = 0x00010000;
        public const uint WS_SYSMENU = 0x00080000;
        public const uint WS_EX_APPWINDOW = 0x00040000;
    }
}
