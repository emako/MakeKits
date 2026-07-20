namespace MakeKits.Workshop.Executable;

/// <inheritdoc/>
public abstract class WorkshopViewContext : WorkshopObject, IWorkshopViewContext
{
    private object _source = null!;
    private object? _icon;
    private string _title = null!;
    private object _viewerContent = null!;
    private bool _isImmersiveMode = false;
    private int _preferredWidth = 800;
    private int _preferredHeight = 600;
    private bool _canResize = true;
    private WorkshopTheme _theme = WorkshopTheme.None;

    /// <inheritdoc/>
    public virtual object Source
    {
        get => _source;
        set => SetProperty(ref _source, value);
    }

    /// <inheritdoc/>
    public virtual object? Icon
    {
        get => _icon;
        set => SetProperty(ref _icon, value);
    }

    /// <inheritdoc/>
    public virtual string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    /// <inheritdoc/>
    public virtual object ViewerContent
    {
        get => _viewerContent;
        set => SetProperty(ref _viewerContent, value);
    }

    /// <inheritdoc/>
    public virtual bool IsImmersiveMode
    {
        get => _isImmersiveMode;
        set => SetProperty(ref _isImmersiveMode, value);
    }

    /// <inheritdoc/>
    public virtual int PreferredWidth
    {
        get => _preferredWidth;
        set => SetProperty(ref _preferredWidth, value);
    }

    /// <inheritdoc/>
    public virtual int PreferredHeight
    {
        get => _preferredHeight;
        set => SetProperty(ref _preferredHeight, value);
    }

    /// <inheritdoc/>
    public virtual bool CanResize
    {
        get => _canResize;
        set => SetProperty(ref _canResize, value);
    }

    /// <inheritdoc/>
    public virtual WorkshopTheme Theme
    {
        get => _theme;
        set => SetProperty(ref _theme, value);
    }
}
