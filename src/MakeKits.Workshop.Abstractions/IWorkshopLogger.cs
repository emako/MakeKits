namespace MakeKits.Workshop;

/// <summary>
/// Logging surface exposed to workshops by the host application.
/// </summary>
public interface IWorkshopLogger
{
    /// <summary>
    /// Emits a no-op log call for API symmetry.
    /// </summary>
    /// <param name="values">Values that would be written if logging were enabled.</param>
    public void None(params object[] values);

    /// <summary>
    /// Writes a trace-level log entry.
    /// </summary>
    /// <param name="values">Values to include in the log entry.</param>
    public void Trace(params object[] values);

    /// <summary>
    /// Writes a debug-level log entry.
    /// </summary>
    /// <param name="values">Values to include in the log entry.</param>
    public void Debug(params object[] values);

    /// <summary>
    /// Writes an information-level log entry.
    /// </summary>
    /// <param name="values">Values to include in the log entry.</param>
    public void Information(params object[] values);

    /// <summary>
    /// Writes a warning-level log entry.
    /// </summary>
    /// <param name="values">Values to include in the log entry.</param>
    public void Warning(params object[] values);

    /// <summary>
    /// Writes an error-level log entry.
    /// </summary>
    /// <param name="values">Values to include in the log entry.</param>
    public void Error(params object[] values);

    /// <summary>
    /// Writes a critical-level log entry.
    /// </summary>
    /// <param name="values">Values to include in the log entry.</param>
    public void Critical(params object[] values);
}
