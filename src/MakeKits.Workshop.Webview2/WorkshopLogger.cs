namespace MakeKits.Workshop.Webview2;

public class WorkshopLogger : IWorkshopLogger
{
    public Action<int, object[]>? Logger = null;

    public void None(params object[] values)
    {
        Logger?.Invoke(0, values);
    }

    public void Trace(params object[] values)
    {
        Logger?.Invoke(0, values);
    }

    public void Debug(params object[] values)
    {
        Logger?.Invoke(0, values);
    }

    public void Error(params object[] values)
    {
        Logger?.Invoke(0, values);
    }

    public void Information(params object[] values)
    {
        Logger?.Invoke(0, values);
    }

    public void Warning(params object[] values)
    {
        Logger?.Invoke(0, values);
    }

    public void Critical(params object[] values)
    {
        Logger?.Invoke(0, values);
    }
}
