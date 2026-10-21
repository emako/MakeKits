using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MakeKits.Workshop.Webview;

public class WebpagePanel : UserControl, IDisposable
{
    protected Uri? _currentUri;
    protected WebView2 _webView = null!;

    private WorkshopTheme _theme = WorkshopTheme.None;

    public virtual WorkshopTheme Theme
    {
        get => _theme;
        set
        {
            _theme = value;
            ApplyTheme();
        }
    }

    public virtual string UserDataFolder { get; set; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        @"MakeKits\Workshop\Webview2_Data\"
    );

    protected virtual string WebResourceRequestedFilter => "*";

    public WebpagePanel()
    {
        if (!WebviewHelper.IsWebviewAvailable())
            Content = CreateDownloadButton();
        else
            InitializeComponent();
    }

    public void Dispose()
    {
        _webView?.Dispose();
        _webView = null!;
        GC.SuppressFinalize(this);
    }

    public void NavigateToUri(Uri uri)
    {
        if (_webView == null)
            return;

        _webView.Source = uri;
        _currentUri = _webView.Source;
    }

    public void NavigateToHtml(string html)
    {
        _webView?.EnsureCoreWebView2Async()
            .ContinueWith(_ => Dispatcher.Invoke(() => _webView?.NavigateToString(html)));
    }

    protected virtual object CreateDownloadButton()
    {
        Button button = new()
        {
            Content = "Viewing this file requires Microsoft Edge Webview to be installed.\nClick here to download it.",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Padding = new Thickness(20, 6, 20, 6),
        };
        button.Click += (_, _) => Process.Start("https://go.microsoft.com/fwlink/p/?LinkId=2124703");
        return button;
    }

    protected virtual void InitializeComponent()
    {
        CoreWebView2CreationProperties creationProperties = new()
        {
            UserDataFolder = UserDataFolder,
        };

        _webView = new WebView2()
        {
            CreationProperties = creationProperties,
            DefaultBackgroundColor = GetThemeBackgroundColor(),
        };

        _webView.NavigationStarting += Webview_NavigationStarting;
        _webView.NavigationCompleted += WebView_NavigationCompleted;
        _webView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
        Content = _webView;

        ApplyTheme();
    }

    /// <summary>
    /// Resolves the effective theme, falling back to <see cref="WorkshopTheme.System"/> when <see cref="Theme"/> is <see cref="WorkshopTheme.None"/>.
    /// </summary>
    protected WorkshopTheme EffectiveTheme =>
        Theme == WorkshopTheme.None ? WorkshopTheme.System : Theme;

    /// <summary>
    /// Returns the background color matching the effective theme.
    /// </summary>
    protected virtual System.Drawing.Color GetThemeBackgroundColor()
    {
        return EffectiveTheme switch
        {
            WorkshopTheme.Dark => System.Drawing.Color.FromArgb(255, 32, 32, 32),
            WorkshopTheme.Light => System.Drawing.Color.White,
            WorkshopTheme.System or _ => OSThemeHelper.AppsUseDarkTheme()
                ? System.Drawing.Color.FromArgb(255, 32, 32, 32)
                : System.Drawing.Color.White,
        };
    }

    /// <summary>
    /// Returns whether the effective theme is dark.
    /// </summary>
    protected virtual bool ShouldUseDarkMode()
    {
        return EffectiveTheme switch
        {
            WorkshopTheme.Dark => true,
            WorkshopTheme.Light => false,
            WorkshopTheme.System or _ => OSThemeHelper.AppsUseDarkTheme(),
        };
    }

    /// <summary>
    /// Applies the current theme to the WebView2 control and the panel background.
    /// Safe to call before and after CoreWebView2 initialization.
    /// </summary>
    protected virtual void ApplyTheme()
    {
        // Set the panel background to match the theme so there is no white flash
        // during the brief moment before the WebView2 renders content.
        System.Drawing.Color bgColor = GetThemeBackgroundColor();
        Background = new SolidColorBrush(Color.FromArgb(
            bgColor.A, bgColor.R, bgColor.G, bgColor.B));

        if (_webView != null)
        {
            _webView.DefaultBackgroundColor = bgColor;

            // Once the CoreWebView2 is initialized we can set the preferred color scheme
            // so that WebView2's own UI (context menus, dialogs, etc.) matches the theme.
            if (_webView.CoreWebView2 != null)
            {
                _webView.CoreWebView2.Profile.PreferredColorScheme = ShouldUseDarkMode()
                    ? CoreWebView2PreferredColorScheme.Dark
                    : CoreWebView2PreferredColorScheme.Light;
            }
        }
    }

    protected virtual void Webview_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        if (e.Uri.StartsWith("data:", StringComparison.Ordinal))
            return;

        Uri newUri = new(e.Uri);
        if (_currentUri != null && newUri == _currentUri)
            return;

        e.Cancel = true;

        try
        {
            if (!Uri.TryCreate(e.Uri, UriKind.Absolute, out Uri? uri))
            {
                Debug.WriteLine($"Invalid URI format: {e.Uri}");
                return;
            }

            if (uri.Scheme == Uri.UriSchemeHttp
                || uri.Scheme == Uri.UriSchemeHttps
                || uri.Scheme == Uri.UriSchemeMailto)
            {
                Process.Start(uri.AbsoluteUri);
            }
        }
        catch (Exception exception)
        {
            Debug.WriteLine($"Failed to open URL: {exception.Message}");
        }
    }

    protected virtual void WebView_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        // Re-apply the theme background after navigation so it stays consistent
        // with the configured theme instead of always resetting to white.
        ApplyTheme();
    }

    protected virtual void WebView_CoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
    {
        if (e.IsSuccess)
        {
            _webView.CoreWebView2.AddWebResourceRequestedFilter(WebResourceRequestedFilter, CoreWebView2WebResourceContext.All);
            _webView.CoreWebView2.WebResourceRequested += WebView_WebResourceRequested;

            // Apply the preferred color scheme now that CoreWebView2 is available.
            ApplyTheme();
        }
    }

    protected virtual void WebView_WebResourceRequested(object? sender, CoreWebView2WebResourceRequestedEventArgs args)
    {
    }
}
