namespace MakeKits.Workshop.WPF.HelloWorld;

public sealed class HelloWorldWorkshopViewContext : WorkshopObject, IWorkshopViewContext
{
    private object _source = null!;
    private string _title = null!;
    private object _viewerContent = null!;
    private bool _isImmersiveMode = false;
    private int _preferredWidth = 0;
    private int _preferredHeight = 0;
    private bool _canResize = true;
    private WorkshopTheme _theme = WorkshopTheme.None;

    /// <inheritdoc/>
    public object Source
    {
        get => _source;
        set => SetProperty(ref _source, value);
    }

    /// <inheritdoc/>
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    /// <inheritdoc/>
    public object ViewerContent
    {
        get => _viewerContent;
        set => SetProperty(ref _viewerContent, value);
    }

    /// <inheritdoc/>
    public bool IsImmersiveMode
    {
        get => _isImmersiveMode;
        set => SetProperty(ref _isImmersiveMode, value);
    }

    /// <inheritdoc/>
    public int PreferredWidth
    {
        get => _preferredWidth;
        set => SetProperty(ref _preferredWidth, value);
    }

    /// <inheritdoc/>
    public int PreferredHeight
    {
        get => _preferredHeight;
        set => SetProperty(ref _preferredHeight, value);
    }

    /// <inheritdoc/>
    public bool CanResize
    {
        get => _canResize;
        set => SetProperty(ref _canResize, value);
    }

    /// <inheritdoc/>
    public WorkshopTheme Theme
    {
        get => _theme;
        set => SetProperty(ref _theme, value);
    }

    public HelloWorldWorkshopViewContext()
    {
        Title = Configuration.Title;
        Theme = Enum.TryParse(Configuration.Theme, out WorkshopTheme theme) ? theme : WorkshopTheme.System;
    }
}
