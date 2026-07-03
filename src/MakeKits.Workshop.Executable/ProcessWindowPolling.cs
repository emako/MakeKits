using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MakeKits.Workshop.Executable;

public class ProcessWindowPolling
{
    public bool IsRunning { private get; set; } = false;

    public void PollProcessWindow(string[] processNames, Action<nint> onWindowFound, int pollInterval = 100)
    {
        IsRunning = true;

        Task task = Task.Factory.StartNew(() =>
        {
            Thread.CurrentThread.Name = "MakeKits_ProcessWindowPollingThread";
            while (IsRunning)
            {
                foreach (string processName in processNames)
                {
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
                                Debug.WriteLine($"[ProcessWindowPolling] Hit Main Window Handle: {windowTitle}");

                                onWindowFound?.Invoke(mainWindowHandle);

                                ChildProcessTracer.Default.AddChildProcess(process.Handle);
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
}
