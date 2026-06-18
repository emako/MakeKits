namespace MakeKits.Workshop.Webview2;

public abstract class WorkshopLogger : IWorkshopLogger
{
    public virtual Action<int, object[]>? Logger { get; set; } = null;

    public virtual void None(params object[] values)
    {
        Logger?.Invoke(0, values);
    }

    public virtual void Trace(params object[] values)
    {
        Logger?.Invoke(0, values);
    }

    public virtual void Debug(params object[] values)
    {
        Logger?.Invoke(0, values);
    }

    public virtual void Error(params object[] values)
    {
        Logger?.Invoke(0, values);
    }

    public virtual void Information(params object[] values)
    {
        Logger?.Invoke(0, values);
    }

    public virtual void Warning(params object[] values)
    {
        Logger?.Invoke(0, values);
    }

    public virtual void Critical(params object[] values)
    {
        Logger?.Invoke(0, values);
    }
}
