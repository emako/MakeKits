using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace MakeKits.Workshop.Host;

public partial class MainWindow : Window
{
    // The currently active workshop context; kept alive so PropertyChanged fires correctly.
    private IWorkshopContext? _activeContext;

    // The workshop item currently being shown.
    private IWorkshopItem? _activeItem;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Closed += OnClosed;
    }

    // -----------------------------------------------------------------
    //  Lifecycle
    // -----------------------------------------------------------------

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        WorkshopManager.GetInstance();
        PopulateWorkshopList();
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        CleanupActiveWorkshop();
    }

    // -----------------------------------------------------------------
    //  List page
    // -----------------------------------------------------------------

    private void PopulateWorkshopList()
    {
        IReadOnlyList<IWorkshopItem> items = WorkshopManager.LoadedWorkshops;

        WorkshopList.ItemsSource = items;

        if (items.Count == 0)
        {
            EmptyStatePanel.Visibility = Visibility.Visible;
            WorkshopCountText.Text = string.Empty;
        }
        else
        {
            EmptyStatePanel.Visibility = Visibility.Collapsed;
            WorkshopCountText.Text = $"({items.Count} plugin(s))";
        }
    }

    private void OnWorkshopCardClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: IWorkshopItem item })
            OpenWorkshop(item);
    }

    // -----------------------------------------------------------------
    //  Open / close workshop
    // -----------------------------------------------------------------

    private void OpenWorkshop(IWorkshopItem item)
    {
        // Clean up any previously active workshop first.
        CleanupActiveWorkshop();

        IWorkshop? workshop = item.Workshop;
        if (workshop == null)
        {
            MessageBox.Show($"Workshop {item.Name} was not loaded correctly.", "Cannot be opened", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _activeItem = item;
        _activeContext = workshop.Context;

        // Show busy overlay while the workshop initialises.
        WorkshopContentControl.Content = null;

        // Navigate to the workshop page.
        ListPage.Visibility = Visibility.Collapsed;
        WorkshopPage.Visibility = Visibility.Visible;

        // Subscribe to context / view-context changes so we can react to title and content updates.
        if (_activeContext != null)
        {
            _activeContext.PropertyChanged += OnContextPropertyChanged;

            if (_activeContext.ViewContext != null)
            {
                _activeContext.ViewContext.Source = this;
                _activeContext.ViewContext.PropertyChanged += OnViewContextPropertyChanged;
            }
        }

        // Both Prepare() and View() must run on the UI (STA) thread.
        try
        {
            workshop.Prepare(_activeContext!);
            workshop.View(_activeContext!);

            // View() sets ViewerContent synchronously; apply it directly because
            // PropertyChanged only fires on *changes*, not on the initial assignment
            // made before we subscribed.
            if (_activeContext?.ViewContext != null)
                ApplyViewContext(_activeContext.ViewContext);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MainWindow] Workshop lifecycle error: {ex}");
            WorkshopContentControl.Content = CreateErrorPanel(ex.Message);
        }
    }

    private void OnBackClicked(object sender, RoutedEventArgs e)
    {
        CleanupActiveWorkshop();
        WorkshopPage.Visibility = Visibility.Collapsed;
        ListPage.Visibility = Visibility.Visible;
    }

    private void CleanupActiveWorkshop()
    {
        if (_activeItem?.Workshop != null)
        {
            try { _activeItem.Workshop.Cleanup(); }
            catch (Exception ex) { Debug.WriteLine($"[MainWindow] Cleanup error: {ex}"); }
        }

        if (_activeContext != null)
        {
            _activeContext.PropertyChanged -= OnContextPropertyChanged;

            _activeContext.ViewContext?.PropertyChanged -= OnViewContextPropertyChanged;
        }

        _activeContext = null;
        _activeItem = null;

        WorkshopContentControl.Content = null;
        WorkshopTitleText.Text = string.Empty;
        WorkshopIconImage.Source = null;
        WorkshopIconImage.Visibility = Visibility.Collapsed;
        Icon = null;
    }

    // -----------------------------------------------------------------
    //  Context change handlers
    // -----------------------------------------------------------------

    private void OnContextPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IWorkshopContext.ViewContext))
        {
            // ViewContext was swapped out; re-subscribe.
            Dispatcher.Invoke(() =>
            {
                if (_activeContext?.ViewContext != null)
                {
                    _activeContext.ViewContext.Source = this;
                    _activeContext.ViewContext.PropertyChanged -= OnViewContextPropertyChanged;
                    _activeContext.ViewContext.PropertyChanged += OnViewContextPropertyChanged;
                    ApplyViewContext(_activeContext.ViewContext);
                }
            });
        }
    }

    private void OnViewContextPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not IWorkshopViewContext vc) return;

        // All UI updates must run on the dispatcher.
        Dispatcher.Invoke(() =>
        {
            switch (e.PropertyName)
            {
                case nameof(IWorkshopViewContext.ViewerContent):
                    WorkshopContentControl.Content = vc.ViewerContent;
                    break;

                case nameof(IWorkshopViewContext.Title):
                    if (!string.IsNullOrWhiteSpace(vc.Title))
                        WorkshopTitleText.Text = vc.Title;
                    break;

                case nameof(IWorkshopViewContext.Icon):
                    ApplyIcon(vc);
                    break;

                case nameof(IWorkshopViewContext.PreferredWidth):
                case nameof(IWorkshopViewContext.PreferredHeight):
                    ApplyPreferredSize(vc);
                    break;
            }
        });
    }

    /// <summary>
    /// Apply all relevant ViewContext properties to the UI at once
    /// (called when the ViewContext reference itself is replaced).
    /// </summary>
    private void ApplyViewContext(IWorkshopViewContext vc)
    {
        if (!string.IsNullOrWhiteSpace(vc.Title))
            WorkshopTitleText.Text = vc.Title;

        WorkshopContentControl.Content = vc.ViewerContent;
        ApplyIcon(vc);
        ApplyPreferredSize(vc);
    }

    private void ApplyIcon(IWorkshopViewContext vc)
    {
        if (vc.Icon is ImageSource imageSource)
        {
            Icon = imageSource;
            WorkshopIconImage.Source = imageSource;
            WorkshopIconImage.Visibility = Visibility.Visible;
        }
        else
        {
            WorkshopIconImage.Source = null;
            WorkshopIconImage.Visibility = Visibility.Collapsed;
        }
    }

    private void ApplyPreferredSize(IWorkshopViewContext vc)
    {
        if (vc.PreferredWidth > 0)
            Width = vc.PreferredWidth;
        if (vc.PreferredHeight > 0)
            Height = vc.PreferredHeight;
    }

    // -----------------------------------------------------------------
    //  Helpers
    // -----------------------------------------------------------------

    private static UIElement CreateErrorPanel(string message)
    {
        return new TextBlock
        {
            Text = $"⚠️ Workshop loading failed\n\n{message}",
            Margin = new Thickness(24),
            FontSize = 14,
            Foreground = System.Windows.Media.Brushes.OrangeRed,
            TextWrapping = TextWrapping.Wrap,
            VerticalAlignment = VerticalAlignment.Top,
        };
    }
}
