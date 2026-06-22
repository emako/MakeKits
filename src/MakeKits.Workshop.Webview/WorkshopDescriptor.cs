using CommunityToolkit.Mvvm.ComponentModel;

namespace MakeKits.Workshop.Webview;

/// <inheritdoc/>
public abstract class WorkshopDescriptor : ObservableObject, IWorkshopDescriptor
{
    /// <inheritdoc/>
    public virtual string Name => "Workshop Webview";

    /// <inheritdoc/>
    public virtual string? Author => "MakeKits";

    /// <inheritdoc/>
    public virtual string? Description => "Support for Webview integration";
}
