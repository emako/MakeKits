using System;
using System.Windows.Controls;

namespace MakeKits.Workshop.Executable;

/// <summary>
/// Abstract base panel interface class, inherit WPF Panel and implement resource release logic
/// </summary>
public abstract class DisposablePanel : Panel, IDisposable
{
    /// <summary>
    /// Release all unmanaged / managed resources
    /// </summary>
    public abstract void Dispose();
}
