using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MakeKits.Workshop.Executable;

/// <summary>
/// A base class for objects that need to implement <see cref="INotifyPropertyChanged"/>.
/// </summary>
/// <remarks>
/// This is a lightweight implementation of the observable pattern,
/// providing <see cref="SetProperty{T}"/> and <see cref="OnPropertyChanged"/> helpers.
/// </remarks>
public abstract class WorkshopObject : INotifyPropertyChanged
{
    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Sets the property to the new value, raising <see cref="PropertyChanged"/> only when the value actually changes.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    /// <param name="field">A reference to the backing field.</param>
    /// <param name="newValue">The new value to assign.</param>
    /// <param name="propertyName">The name of the property (auto-supplied by the compiler).</param>
    /// <returns><see langword="true"/> if the value changed; otherwise <see langword="false"/>.</returns>
    protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, newValue))
        {
            return false;
        }

        field = newValue;
        OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event for the specified property.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed (auto-supplied by the compiler).</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
