using System;

namespace MakeKits.Workshop.Webview;

/// <inheritdoc/>
public abstract class WorkshopLogger : IWorkshopLogger
{
    public virtual Action<int, object[]>? Logger { get; set; } = null;

    /// <inheritdoc/>
    public virtual void None(params object[] values)
    {
        Logger?.Invoke(0, values);
    }

    /// <inheritdoc/>
    public virtual void Trace(params object[] values)
    {
        Logger?.Invoke(0, values);
    }

    /// <inheritdoc/>
    public virtual void Debug(params object[] values)
    {
        Logger?.Invoke(0, values);
    }

    /// <inheritdoc/>
    public virtual void Error(params object[] values)
    {
        Logger?.Invoke(0, values);
    }

    /// <inheritdoc/>
    public virtual void Information(params object[] values)
    {
        Logger?.Invoke(0, values);
    }

    /// <inheritdoc/>
    public virtual void Warning(params object[] values)
    {
        Logger?.Invoke(0, values);
    }

    /// <inheritdoc/>
    public virtual void Critical(params object[] values)
    {
        Logger?.Invoke(0, values);
    }
}
