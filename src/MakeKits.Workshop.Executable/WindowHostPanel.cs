using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms.Integration;

namespace MakeKits.Workshop.Executable;

/// <summary>
/// Abstract base panel interface class, inherit WPF Panel and implement resource release logic
/// </summary>
public abstract class WindowHostPanel : WindowsFormsHost, IDisposable
{
    /// <summary>
    /// WinForms container used as the native parent for embedded external windows.
    /// </summary>
    protected System.Windows.Forms.Panel Container { get; }

    protected WindowHostPanel()
    {
        Container = new System.Windows.Forms.Panel
        {
            Dock = System.Windows.Forms.DockStyle.Fill,
            BackColor = System.Drawing.Color.Black,
        };
        Child = Container;
        HorizontalAlignment = HorizontalAlignment.Stretch;
        VerticalAlignment = VerticalAlignment.Stretch;

        Container.HandleCreated += (_, _) => OnContainerHandleCreated();
        Container.Resize += (_, _) => OnContainerResized();
    }

    protected virtual void OnContainerHandleCreated()
    {
    }

    protected virtual void OnContainerResized()
    {
    }

    /// <summary>
    /// Native handle of the WinForms container, or zero when not yet created.
    /// </summary>
    protected nint ContainerHwnd =>
        Container.IsHandleCreated ? Container.Handle : 0;

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

        ResolveHostPixelSize(hostHwnd, Container, this, out int width, out int height);
        _ = User32.MoveWindow(externalHwnd, 0, 0, width, height, true);
        _ = User32.SetWindowPos(externalHwnd, User32.HWND_TOP, 0, 0, 0, 0,
            User32.SWP_SHOWWINDOW | User32.SWP_FRAMECHANGED | User32.SWP_NOMOVE | User32.SWP_NOSIZE);

        // Reset hidden/minimized show state left over from CreateNoWindow / Hidden startup.
        _ = User32.ShowWindow(externalHwnd, User32.SW_HIDE);
        _ = User32.ShowWindow(externalHwnd, User32.SW_SHOWNORMAL);
        _ = User32.ShowWindow(externalHwnd, User32.SW_SHOW);
        _ = User32.ShowWindow(externalHwnd, User32.SW_MAXIMIZE);
        _ = User32.UpdateWindow(externalHwnd);
        _ = User32.InvalidateRect(externalHwnd, IntPtr.Zero, true);

        return previousParent;
    }

    protected internal static void ResizeEmbeddedWindow(
        nint externalHwnd,
        nint hostHwnd,
        System.Windows.Forms.Panel? hostControl = null,
        FrameworkElement? layoutSource = null)
    {
        if (externalHwnd == 0 || hostHwnd == 0)
            return;

        ResolveHostPixelSize(hostHwnd, hostControl, layoutSource, out int width, out int height);
        _ = User32.MoveWindow(externalHwnd, 0, 0, width, height, true);

        _ = User32.ShowWindow(externalHwnd, User32.SW_SHOW);
        _ = User32.ShowWindow(externalHwnd, User32.SW_MAXIMIZE);
    }

    private static void ResolveHostPixelSize(
        nint hostHwnd,
        System.Windows.Forms.Control? hostControl,
        FrameworkElement? layoutSource,
        out int width,
        out int height)
    {
        width = 0;
        height = 0;

        // QuickLook.Plugin.SumatraPDFReader resizes embedded windows with MoveWindow(host, 0, 0, Width, Height)
        // inside the WinForms host control's Resize handler — not GetClientRect/GetWindowRect.
        if (hostControl != null)
        {
            width = hostControl.Width;
            height = hostControl.Height;
            if (width > 0 && height > 0)
                return;
        }

        if (hostHwnd != 0 && User32.GetClientRect(hostHwnd, out User32.RECT clientRect))
        {
            width = clientRect.Width;
            height = clientRect.Height;
            if (width > 0 && height > 0)
                return;
        }

        if (layoutSource != null)
        {
            double dpiX = 1d;
            double dpiY = 1d;
            PresentationSource? source = PresentationSource.FromVisual(layoutSource);
            if (source?.CompositionTarget != null)
            {
                dpiX = source.CompositionTarget.TransformToDevice.M11;
                dpiY = source.CompositionTarget.TransformToDevice.M22;
            }

            width = Math.Max(1, (int)Math.Round(layoutSource.ActualWidth * dpiX));
            height = Math.Max(1, (int)Math.Round(layoutSource.ActualHeight * dpiY));
            return;
        }

        width = Math.Max(1, width);
        height = Math.Max(1, height);
    }

    protected internal static class User32
    {
        public static readonly nint HWND_TOP = 0;

        [DllImport("user32.dll")]
        public static extern nint SetParent(nint hWndChild, nint hWndNewParent);

        [DllImport("user32.dll")]
        public static extern uint GetWindowLong(nint hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(nint hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(nint hWnd, nint insertAfter, int x, int y, int cx, int cy, uint flags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool MoveWindow(nint hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(nint hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(nint hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(nint hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool UpdateWindow(nint hWnd);

        [DllImport("user32.dll")]
        public static extern bool InvalidateRect(nint hWnd, nint lpRect, bool bErase);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public readonly int Width => Right - Left;
            public readonly int Height => Bottom - Top;
        }

        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;
        public const int SW_HIDE = 0;
        public const int SW_SHOWNORMAL = 1;
        public const int SW_SHOWMINIMIZED = 2;
        public const int SW_MAXIMIZE = 3;
        public const int SW_SHOWNOACTIVATE = 4;
        public const int SW_SHOW = 5;
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
        public const uint SWP_SHOWWINDOW = 0x0040;
        public const uint SWP_FRAMECHANGED = 0x0020;
        public const uint SWP_NOMOVE = 0x0002;
        public const uint SWP_NOSIZE = 0x0001;
    }
}
