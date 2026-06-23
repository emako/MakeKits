using System.Windows;

namespace MakeKits.Workshop.Host;

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
    }

    private void OnClosed(object? sender, EventArgs e)
    {
    }
}
