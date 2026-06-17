using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace MakeKits.Workshop.Webview2;

public class WebpagePanel : UserControl
{
    protected WebView2 _webView = null!;

    public virtual WorkshopTheme Theme { get; set; } = WorkshopTheme.None;

    public virtual string UserDataFolder { get; set; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        @"MakeKits.Workshop\WebView2_Data\"
    );

    public WebpagePanel()
    {
        if (!WebView2Helper.IsWebView2Available())
            Content = CreateDownloadButton();
        else
            InitializeComponent();
    }

    public void Dispose()
    {
        _webView?.Dispose();
        _webView = null!;
    }

    protected virtual object CreateDownloadButton()
    {
        Button button = new()
        {
            Content = "Viewing this file requires Microsoft Edge WebView2 to be installed.\nClick here to download it.",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Padding = new Thickness(20, 6, 20, 6),
        };
        button.Click += (sender, e) => Process.Start("https://go.microsoft.com/fwlink/p/?LinkId=2124703");
        return button;
    }

    protected virtual void InitializeComponent()
    {
        _webView = new WebView2
        {
            CreationProperties = new CoreWebView2CreationProperties()
            {
                UserDataFolder = UserDataFolder,
            },
        };

        if (Theme != WorkshopTheme.None)
        {
            // Prevent white flash in dark mode
            _webView.DefaultBackgroundColor = Theme switch
            {
                WorkshopTheme.None => throw new InvalidOperationException("Theme cannot be None when setting background color."),
                WorkshopTheme.Dark => System.Drawing.Color.FromArgb(255, 32, 32, 32),
                WorkshopTheme.Light => System.Drawing.Color.White,
                WorkshopTheme.System or _ => OSThemeHelper.AppsUseDarkTheme()
                    ? System.Drawing.Color.FromArgb(255, 32, 32, 32)
                    : System.Drawing.Color.White,
            };
        }

        _webView.NavigationStarting += Webview_NavigationStarting;
        _webView.NavigationCompleted += WebView_NavigationCompleted;
        _webView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
        Content = _webView;
    }

    protected virtual void Webview_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
    }

    protected virtual void WebView_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        _webView.DefaultBackgroundColor = System.Drawing.Color.White; // Reset to white after page load to match expected default behavior
    }

    protected virtual void WebView_CoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
    {
        if (e.IsSuccess)
        {
            _webView.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
            _webView.CoreWebView2.WebResourceRequested += WebView_WebResourceRequested;
        }
    }

    protected virtual void WebView_WebResourceRequested(object? sender, CoreWebView2WebResourceRequestedEventArgs args)
    {
    }
}
