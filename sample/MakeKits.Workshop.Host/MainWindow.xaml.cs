using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Violeta.Controls;
using Wpf.Ui.Violeta.Win32;

namespace MakeKits.Workshop.Host;

public partial class MainWindow : ShellWindow
{
    private const string DefaultTitle = "MakeKits Workshop";

    private readonly MainWindowViewModel _viewModel;

    // The currently active workshop context; kept alive so PropertyChanged fires correctly.
    private IWorkshopContext? _activeContext;

    // The workshop item currently being shown.
    private IWorkshopItem? _activeItem;

    private ImageSource? _defaultIcon;

    public MainWindow()
    {
        _viewModel = new MainWindowViewModel(OpenWorkshop, OpenWorkshopInContentWindow);
        DataContext = _viewModel;

        InitializeComponent();
        _defaultIcon = Icon;
        Loaded += OnLoaded;
        Closed += OnClosed;
    }

    // -----------------------------------------------------------------
    //  Lifecycle
    // -----------------------------------------------------------------

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        WorkshopManager.GetInstance();
        _viewModel.LoadWorkshops(WorkshopManager.LoadedWorkshops);
        ShowListChrome();
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        CleanupActiveWorkshop();
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
            System.Windows.MessageBox.Show($"Workshop {item.Name} was not loaded correctly.", "Cannot be opened", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _activeItem = item;
        _activeContext = workshop.Context;

        // Show busy overlay while the workshop initialises.
        WorkshopContentControl.Content = null;

        // Navigate to the workshop page.
        ListPage.Visibility = Visibility.Collapsed;
        WorkshopPage.Visibility = Visibility.Visible;
        ShowWorkshopChrome(item.Name);

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

    /// <summary>
    /// Opens web / console workshops in a separate <see cref="ContentWindow"/> hosting their panel.
    /// </summary>
    private void OpenWorkshopInContentWindow(IWorkshopItem item)
    {
        IWorkshop? workshop = item.Workshop;
        if (workshop == null)
        {
            System.Windows.MessageBox.Show($"Workshop {item.Name} was not loaded correctly.", "Cannot be opened", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        IWorkshopContext? context = workshop.Context;
        if (context == null)
        {
            System.Windows.MessageBox.Show($"Workshop {item.Name} has no context.", "Cannot be opened", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        WorkshopContentWindowControl control = new() { Title = item.Name };
        ContentWindow window = ContentWindow.Create(control);
        window.Owner = this;
        window.Title = item.Name;
        window.Width = 1280;
        window.Height = 720;
        window.MinimizeButtonVisibility = Visibility.Visible;
        window.MaximizeButtonVisibility = Visibility.Visible;
        window.ShowInTaskbar = true;
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        window.ResizeMode = ResizeMode.CanResize;

        PropertyChangedEventHandler? viewContextHandler = null;
        bool cleaned = false;

        void CleanupWindow()
        {
            if (cleaned)
                return;

            cleaned = true;

            context.PropertyChanged -= OnContentWindowContextPropertyChanged;

            if (viewContextHandler != null && context.ViewContext != null)
                context.ViewContext.PropertyChanged -= viewContextHandler;

            try { workshop.Cleanup(); }
            catch (Exception ex) { Debug.WriteLine($"[MainWindow] ContentWindow cleanup error: {ex}"); }
        }

        viewContextHandler = (_, e) =>
        {
            if (context.ViewContext is not IWorkshopViewContext vc)
                return;

            Dispatcher.Invoke(() => ApplyContentWindowViewContext(window, control, vc, e.PropertyName));
        };

        void OnContentWindowContextPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(IWorkshopContext.ViewContext))
                return;

            Dispatcher.Invoke(() =>
            {
                if (context.ViewContext == null)
                    return;

                context.ViewContext.Source = window;
                context.ViewContext.PropertyChanged -= viewContextHandler;
                context.ViewContext.PropertyChanged += viewContextHandler;
                ApplyContentWindowViewContext(window, control, context.ViewContext, propertyName: null);
            });
        }

        context.PropertyChanged += OnContentWindowContextPropertyChanged;

        if (context.ViewContext != null)
        {
            context.ViewContext.Source = window;
            context.ViewContext.PropertyChanged += viewContextHandler;
        }

        window.Closed += (_, _) => CleanupWindow();

        try
        {
            workshop.Prepare(context);
            workshop.View(context);

            if (context.ViewContext != null)
                ApplyContentWindowViewContext(window, control, context.ViewContext, propertyName: null);

            window.Show();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MainWindow] ContentWindow workshop lifecycle error: {ex}");
            CleanupWindow();
            try { window.Close(); } catch { /* ignore */ }
            System.Windows.MessageBox.Show($"Failed to open {item.Name}.\n\n{ex.Message}", "Open in New Window", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private static void ApplyContentWindowViewContext(
        ContentWindow window,
        WorkshopContentWindowControl control,
        IWorkshopViewContext vc,
        string? propertyName)
    {
        bool applyAll = propertyName == null;

        if (applyAll || propertyName == nameof(IWorkshopViewContext.Title))
        {
            if (!string.IsNullOrWhiteSpace(vc.Title))
                window.Title = vc.Title;
        }

        if (applyAll || propertyName == nameof(IWorkshopViewContext.ViewerContent))
            control.Content = vc.ViewerContent;

        if (applyAll || propertyName is nameof(IWorkshopViewContext.PreferredWidth) or nameof(IWorkshopViewContext.PreferredHeight))
        {
            if (vc.PreferredWidth > 0)
                window.Width = vc.PreferredWidth;
            if (vc.PreferredHeight > 0)
                window.Height = vc.PreferredHeight;
        }

        if (applyAll || propertyName == nameof(IWorkshopViewContext.Icon))
        {
            if (vc.Icon is ImageSource imageSource)
                window.Icon = imageSource;
        }
    }

    private void OnTitleBarBackClick(object? sender, EventArgs e)
    {
        CleanupActiveWorkshop();
        WorkshopPage.Visibility = Visibility.Collapsed;
        ListPage.Visibility = Visibility.Visible;
        ShowListChrome();
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
    }

    private void ShowListChrome()
    {
        Title = DefaultTitle;
        Icon = _defaultIcon;
        AppTitleBar.IsBackButtonEnabled = false;
        AppTitleBar.BackButtonVisibility = Visibility.Collapsed;
    }

    private void ShowWorkshopChrome(string? title)
    {
        if (!string.IsNullOrWhiteSpace(title))
            Title = title!;

        AppTitleBar.IsBackButtonEnabled = true;
        AppTitleBar.BackButtonVisibility = Visibility.Visible;
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
                        Title = vc.Title;
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
            Title = vc.Title;

        WorkshopContentControl.Content = vc.ViewerContent;
        ApplyIcon(vc);
        ApplyPreferredSize(vc);
    }

    private void ApplyIcon(IWorkshopViewContext vc)
    {
        if (vc.Icon is ImageSource imageSource)
            Icon = imageSource;
        else
            Icon = _defaultIcon;
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
