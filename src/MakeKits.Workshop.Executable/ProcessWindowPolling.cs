using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MakeKits.Workshop.Executable;

public static class ProcessWindowPolling
{
    public static void PollProcessWindow(string[] processNames, Action<nint> onWindowFound, int pollInterval = 100)
    {
        Task task = Task.Factory.StartNew(() =>
        {
            Thread.CurrentThread.Name = "[MakeKits]ProcessWindowPollingThread";
            while (true)
            {
                foreach (string processName in processNames)
                {
                    string[] dd = [.. Process.GetProcesses().Select(p => ProcessMonitor.GetExeNameByProcessId((uint)p.Id))!];

                    Process[] processes = Process.GetProcesses();
                    foreach (Process process in processes)
                    {
                        string? exeName = ProcessMonitor.GetExeNameByProcessId((uint)process.Id);

                        if (processNames.Any(name => name == exeName))
                        {
                            nint mainWindowHandle = process.MainWindowHandle;
                            string? windowTitle = ProcessMonitor.GetWindowText(mainWindowHandle);

                            if (!string.IsNullOrWhiteSpace(windowTitle) && mainWindowHandle != IntPtr.Zero)
                            {
                                onWindowFound?.Invoke(mainWindowHandle);
                                return;
                            }
                        }
                    }
                }
                Thread.Sleep(pollInterval);
            }
        }, TaskCreationOptions.LongRunning);

        Debug.WriteLine($"[ProcessWindowPolling] Task Status: {task.Status}"); // Task Status: Running
    }

    public static void PollProcessWindow(Process process, Action<nint> onWindowFound, int pollInterval = 1000)
    {
        Task task = Task.Factory.StartNew(() =>
        {
            Thread.CurrentThread.Name = "[MakeKits]ProcessWindowPollingThread";
            while (!process.HasExited)
            {
                nint mainWindowHandle = process.MainWindowHandle;
                if (mainWindowHandle != IntPtr.Zero)
                {
                    onWindowFound?.Invoke(mainWindowHandle);
                    break;
                }
                Thread.Sleep(pollInterval);
            }
        }, TaskCreationOptions.LongRunning);

        Debug.WriteLine($"[ProcessWindowPolling] Task Status: {task.Status}"); // Task Status: Running
    }
}
