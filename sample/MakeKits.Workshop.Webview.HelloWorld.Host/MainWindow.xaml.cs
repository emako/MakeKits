using System.Windows;

namespace MakeKits.Workshop.Webview.HelloWorld.Host;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Closed += OnClosed;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        WorkshopManager.LoadWorkshops();
    }

    private void OnClosed(object? sender, EventArgs e)
    {
    }
}
