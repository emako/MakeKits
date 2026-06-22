using System.Windows;

namespace MakeKits.Workshop.Webview.HelloWorld.Host;

public partial class MainWindow : Window
{
    private readonly HelloWorldWorkshop _workshop = new();
    private readonly HelloWorldWorkshopContext _context = new();

    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Closed += OnClosed;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _workshop.Context = _context;
        _context.ViewContext!.Source = this;
        _workshop.Init();
        _workshop.Prepare(_context);
        _workshop.View(_context);

        Title = _context.ViewContext!.Title;
        Width = _context.ViewContext.PreferredWidth;
        Height = _context.ViewContext.PreferredHeight;
        Content = _context.ViewContext.ViewerContent;
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        _workshop.Cleanup();
    }
}
