using MakeKits.Workshop.Webview;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
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

    // ─────────────────────────────────────────────────────────────────
    //  Lifecycle
    // ─────────────────────────────────────────────────────────────────

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        WorkshopManager.GetInstance();
        PopulateWorkshopList();
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        CleanupActiveWorkshop();
    }

    // ─────────────────────────────────────────────────────────────────
    //  List page
    // ─────────────────────────────────────────────────────────────────

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
            WorkshopCountText.Text = $"({items.Count} 个插件)";
        }
    }

    private void OnWorkshopCardClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: IWorkshopItem item })
            OpenWorkshop(item);
    }

    // ─────────────────────────────────────────────────────────────────
    //  Open / close workshop
    // ─────────────────────────────────────────────────────────────────

    private void OpenWorkshop(IWorkshopItem item)
    {
        // Clean up any previously active workshop first.
        CleanupActiveWorkshop();

        IWorkshop? workshop = item.Workshop;
        if (workshop == null)
        {
            MessageBox.Show($"工坊{item.Name}未正确加载。", "无法打开", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _activeItem = item;
        _activeContext = workshop.Context;

        // Populate the title bar immediately with the item name as a placeholder.
        WorkshopTitleText.Text = item.Name;

        // Show busy overlay while the workshop initialises.
        ShowBusy(true);
        WorkshopContent.Content = null;

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

        // Run Prepare + View on a background thread to avoid blocking the UI.
        // Results (ViewerContent) are marshalled back via PropertyChanged → Dispatcher.
        System.Threading.ThreadPool.QueueUserWorkItem(_ =>
        {
            try
            {
                workshop.Prepare(_activeContext!);

                // After Prepare, ViewContext.IsBusy may have been changed by the plugin.
                // We respect that; if not set, we keep our own busy state.

                workshop.View(_activeContext!);

                // View() is synchronous and, upon return, ViewerContent is already set.
                // The PropertyChanged handler will update the UI on the dispatcher.
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MainWindow] Workshop lifecycle error: {ex}");
                Dispatcher.Invoke(() =>
                {
                    ShowBusy(false);
                    WorkshopContent.Content = CreateErrorPanel(ex.Message);
                });
            }
        });
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

        WorkshopContent.Content = null;
        ShowBusy(false);
        WorkshopTitleText.Text = string.Empty;
    }

    // ─────────────────────────────────────────────────────────────────
    //  Context change handlers
    // ─────────────────────────────────────────────────────────────────

    private void OnContextPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
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

    private void OnViewContextPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (sender is not IWorkshopViewContext vc) return;

        // All UI updates must run on the dispatcher.
        Dispatcher.Invoke(() =>
        {
            switch (e.PropertyName)
            {
                case nameof(IWorkshopViewContext.ViewerContent):
                    WorkshopContent.Content = vc.ViewerContent;
                    // Once content arrives, hide the busy overlay (unless the plugin wants it on).
                    if (!vc.IsBusy)
                        ShowBusy(false);
                    break;

                case nameof(IWorkshopViewContext.IsBusy):
                    ShowBusy(vc.IsBusy);
                    break;

                case nameof(IWorkshopViewContext.Title):
                    if (!string.IsNullOrWhiteSpace(vc.Title))
                        WorkshopTitleText.Text = vc.Title;
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

        ShowBusy(vc.IsBusy);
        WorkshopContent.Content = vc.ViewerContent;
        ApplyPreferredSize(vc);
    }

    private void ApplyPreferredSize(IWorkshopViewContext vc)
    {
        if (vc.PreferredWidth > 0)
            Width = vc.PreferredWidth;
        if (vc.PreferredHeight > 0)
            Height = vc.PreferredHeight;
    }

    // ─────────────────────────────────────────────────────────────────
    //  Helpers
    // ─────────────────────────────────────────────────────────────────

    private void ShowBusy(bool isBusy)
    {
        BusyOverlay.Visibility = isBusy ? Visibility.Visible : Visibility.Collapsed;
    }

    private static UIElement CreateErrorPanel(string message)
    {
        return new TextBlock
        {
            Text = $"⚠️ 工坊加载失败\n\n{message}",
            Margin = new Thickness(24),
            FontSize = 14,
            Foreground = System.Windows.Media.Brushes.OrangeRed,
            TextWrapping = TextWrapping.Wrap,
            VerticalAlignment = VerticalAlignment.Top,
        };
    }
}
