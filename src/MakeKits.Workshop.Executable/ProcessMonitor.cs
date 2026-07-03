using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace MakeKits.Workshop.Executable;

/// <summary>
/// Windows process introspection helpers backed by kernel32, ntdll, and user32 P/Invoke.
/// </summary>
[SuppressMessage("Usage", "CA2263:Prefer generic overload when type is known")]
[SuppressMessage("Globalization", "CA2101:Specify marshalling for P/Invoke string arguments")]
[SuppressMessage("Interoperability", "CA1401:P/Invokes should not be visible")]
public static class ProcessMonitor
{
    /// <summary>
    /// Returns the parent process ID for <paramref name="pid"/>, or <see langword="null"/> when the
    /// process cannot be opened or <c>NtQueryInformationProcess</c> fails.
    /// </summary>
    public static unsafe int? GetParentProcessId(int pid)
    {
        using Kernel32.SafeHPROCESS hProcess = Kernel32.OpenProcess(ACCESS_MASK.GENERIC_READ, false, (uint)pid);

        if (hProcess.IsInvalid)
        {
            return null!;
        }

        NtDll.PROCESS_BASIC_INFORMATION pbi = new();
        NTStatus status = NtDll.NtQueryInformationProcess(hProcess, NtDll.PROCESSINFOCLASS.ProcessBasicInformation, (nint)(&pbi), (uint)Marshal.SizeOf<NtDll.PROCESS_BASIC_INFORMATION>(), out var returnLength);

        if (status == NTStatus.STATUS_SUCCESS)
        {
            return (int)pbi.InheritedFromUniqueProcessId;
        }
        else
        {
            return null!;
        }
    }

    public static string? GetWindowText(nint hWnd)
    {
        StringBuilder title = new(256);
        _ = User32.GetWindowText(hWnd, title, title.Capacity);
        string windowTitle = title.ToString();

        return windowTitle;
    }

    /// <summary>
    /// Returns the title of the first top-level window owned by <paramref name="pid"/>,
    /// or <see langword="null"/> when no matching window is found.
    /// </summary>
    public static string? GetWindowTextByProcessId(int pid)
    {
        string? windowTitle = null;

        User32.EnumWindows((hWnd, lParam) =>
        {
            _ = User32.GetWindowThreadProcessId(hWnd, out uint processId);

            if (processId == pid)
            {
                StringBuilder title = new(256);
                _ = User32.GetWindowText(hWnd, title, title.Capacity);
                windowTitle = title.ToString();
                // Stop enumeration after the first matching window.
                return false;
            }
            return true;
        }, IntPtr.Zero);

        return windowTitle;
    }

    /// <summary>
    /// Returns all window handles (top-level and child) owned by <paramref name="pid"/>.
    /// </summary>
    public static nint[] GetWindowHandlesByProcessId(int pid)
    {
        List<nint> handles = new List<nint>();
        uint targetPid = (uint)pid;

        User32.EnumWindows((hWnd, lParam) =>
        {
            User32.GetWindowThreadProcessId(hWnd, out uint processId);
            if (processId != targetPid)
                return true;

            handles.Add((nint)hWnd);
            CollectChildWindowHandles((nint)hWnd, targetPid, handles);
            return true;
        }, IntPtr.Zero);

        return handles.ToArray();
    }

    private static void CollectChildWindowHandles(nint parentHwnd, uint pid, List<nint> handles)
    {
        User32.EnumChildWindows((IntPtr)parentHwnd, (hWnd, lParam) =>
        {
            User32.GetWindowThreadProcessId(hWnd, out uint processId);
            if (processId == pid)
                handles.Add((nint)hWnd);

            return true;
        }, IntPtr.Zero);
    }

    /// <summary>
    /// Looks up a running process by executable file name (for example, <c>notepad.exe</c>).
    /// Returns <c>0</c> when no match is found.
    /// </summary>
    public static uint GetProcessIdByName(string processName)
    {
        uint pid = 0;
        Kernel32.PROCESSENTRY32 pe32 = new()
        {
            dwSize = (uint)Marshal.SizeOf(typeof(Kernel32.PROCESSENTRY32))
        };

        using Kernel32.SafeHSNAPSHOT snap = Kernel32.CreateToolhelp32Snapshot(Kernel32.TH32CS.TH32CS_SNAPPROCESS, 0);
        if (!snap.IsInvalid)
        {
            if (Kernel32.Process32First(snap, ref pe32))
            {
                do
                {
                    if (pe32.szExeFile.Equals(processName, StringComparison.OrdinalIgnoreCase))
                    {
                        pid = pe32.th32ProcessID;
                        break;
                    }
                } while (Kernel32.Process32Next(snap, ref pe32));
            }
        }
        return pid;
    }

    /// <summary>
    /// Returns the full executable path for <paramref name="pid"/>, or <see langword="null"/>
    /// when the process cannot be queried.
    /// </summary>
    public static string? GetExeNameByProcessId(uint pid)
    {
        using Kernel32.SafeHPROCESS hProcess = Kernel32.OpenProcess(new ACCESS_MASK(Kernel32.ProcessAccess.PROCESS_QUERY_INFORMATION), false, pid);

        if (!hProcess.IsInvalid)
        {
            StringBuilder exeName = new(260);
            uint size = (uint)exeName.Capacity;

            if (Kernel32.QueryFullProcessImageName(hProcess, Kernel32.PROCESS_NAME.PROCESS_NAME_WIN32, exeName, ref size))
            {
                return exeName.ToString();
            }
            else
            {
                Debug.WriteLine($"Error getting process image file name. Error code: {Marshal.GetLastWin32Error()}");
            }
        }
        return null;
    }

    /// <summary>
    /// Converts a Win32 error code into a localized system message.
    /// </summary>
    public static string GetLastErrorAsString(Win32Error errorCode)
    {
        StringBuilder messageBuffer = new(256);
        int formatResult = Kernel32.FormatMessage(
            Kernel32.FormatMessageFlags.FORMAT_MESSAGE_FROM_SYSTEM | Kernel32.FormatMessageFlags.FORMAT_MESSAGE_IGNORE_INSERTS,
            IntPtr.Zero,
            (uint)errorCode,
            0,
            messageBuffer,
            (uint)messageBuffer.Capacity,
            IntPtr.Zero
        );

        // FormatMessage returns 0 on failure, which coincides with STATUS_SUCCESS.
        if (formatResult == (int)NTStatus.STATUS_SUCCESS)
        {
            return $"Unknown error (Code {errorCode})";
        }

        return messageBuffer.ToString().Trim();
    }

    /// <summary>kernel32.dll P/Invoke declarations used by <see cref="ProcessMonitor"/>.</summary>
    public static class Kernel32
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern SafeHPROCESS OpenProcess(
            ACCESS_MASK dwDesiredAccess,
            [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
            uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern SafeHSNAPSHOT CreateToolhelp32Snapshot(TH32CS dwFlags, uint th32ProcessID);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool Process32First(SafeHSNAPSHOT hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool Process32Next(SafeHSNAPSHOT hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryFullProcessImageName(
            SafeHPROCESS hProcess,
            PROCESS_NAME dwFlags,
            StringBuilder lpExeName,
            ref uint lpdwSize);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int FormatMessage(
            FormatMessageFlags dwFlags,
            IntPtr lpSource,
            uint dwMessageId,
            int dwLanguageId,
            StringBuilder lpBuffer,
            uint nSize,
            IntPtr arguments);

        [Flags]
        public enum TH32CS : uint
        {
            TH32CS_SNAPPROCESS = 0x00000002,
        }

        [Flags]
        public enum ProcessAccess : uint
        {
            PROCESS_QUERY_INFORMATION = 0x0400,
        }

        public enum PROCESS_NAME : uint
        {
            PROCESS_NAME_WIN32 = 0,
        }

        [Flags]
        public enum FormatMessageFlags : uint
        {
            FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000,
            FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200,
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct PROCESSENTRY32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public nuint th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExeFile;
        }

        public sealed class SafeHPROCESS : SafeHandleZeroOrMinusOneIsInvalid
        {
            public SafeHPROCESS() : base(true)
            {
            }

            public SafeHPROCESS(IntPtr handle, bool ownsHandle) : base(ownsHandle)
            {
                SetHandle(handle);
            }

            protected override bool ReleaseHandle() => CloseHandle(handle);

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool CloseHandle(IntPtr hObject);
        }

        public sealed class SafeHSNAPSHOT : SafeHandleZeroOrMinusOneIsInvalid
        {
            public SafeHSNAPSHOT() : base(true)
            {
            }

            public SafeHSNAPSHOT(IntPtr handle, bool ownsHandle) : base(ownsHandle)
            {
                SetHandle(handle);
            }

            protected override bool ReleaseHandle() => CloseHandle(handle);

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool CloseHandle(IntPtr hObject);
        }
    }

    /// <summary>ntdll.dll P/Invoke declarations used by <see cref="ProcessMonitor"/>.</summary>
    public static class NtDll
    {
        [DllImport("ntdll.dll")]
        public static extern NTStatus NtQueryInformationProcess(
            SafeHandle processHandle,
            PROCESSINFOCLASS processInformationClass,
            nint processInformation,
            uint processInformationLength,
            out uint returnLength);

        public enum PROCESSINFOCLASS
        {
            ProcessBasicInformation = 0,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_BASIC_INFORMATION
        {
            public nint ExitStatus;
            public nint PebBaseAddress;
            public nint AffinityMask;
            public nint BasePriority;

            /// <summary>Parent process ID.</summary>
            public nuint InheritedFromUniqueProcessId;
        }
    }

    /// <summary>user32.dll P/Invoke declarations used by <see cref="ProcessMonitor"/>.</summary>
    public static class User32
    {
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
    }

    /// <summary>Win32 access-right mask passed to <see cref="Kernel32.OpenProcess"/>.</summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct ACCESS_MASK
    {
        private readonly uint value;

        public ACCESS_MASK(uint value) => this.value = value;

        public ACCESS_MASK(Kernel32.ProcessAccess access) => this.value = (uint)access;

        public static ACCESS_MASK GENERIC_READ => new(0x80000000);

        public static implicit operator uint(ACCESS_MASK mask) => mask.value;
    }

    /// <summary>NTSTATUS values returned by ntdll entry points.</summary>
    public enum NTStatus : uint
    {
        STATUS_SUCCESS = 0,
    }

    /// <summary>Strongly typed wrapper for Win32 error codes.</summary>
    public readonly struct Win32Error(uint value)
    {
        private readonly uint value = value;

        public static implicit operator uint(Win32Error error) => error.value;
    }
}
