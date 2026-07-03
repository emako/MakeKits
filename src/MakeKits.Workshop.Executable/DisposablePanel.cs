using System;
using System.Runtime.InteropServices;
using System.Windows.Controls;

namespace MakeKits.Workshop.Executable;

/// <summary>
/// Abstract base panel interface class, inherit WPF Panel and implement resource release logic
/// </summary>
public abstract class DisposablePanel : Panel, IDisposable
{
    /// <summary>
    /// Set the parent window of an external window to the host window
    /// </summary>
    /// <param name="externalHwnd">The handle of the external window</param>
    /// <param name="hostHwnd">The handle of the host window</param>
    /// <returns>The handle of the previous parent window</returns>
    public virtual nint SetParent(nint externalHwnd, nint hostHwnd)
    {
        return User32.SetParent(hostHwnd, externalHwnd);
    }

    /// <summary>
    /// Release all unmanaged / managed resources
    /// </summary>
    public abstract void Dispose();

    protected internal static class User32
    {
        [DllImport("user32.dll")]
        public static extern nint SetParent(nint hWndChild, nint hWndNewParent);

        [DllImport("user32.dll")]
        public static extern uint GetWindowLong(nint hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(nint hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(nint hWnd, nint insertAfter, int x, int y, int cx, int cy, uint flags);

        public const int GWL_STYLE = -16;
        public const uint WS_CHILD = 0x40000000;
        public const uint SWP_NOZORDER = 0x4;
        public const uint SWP_NOACTIVATE = 0x10;
    }
}
