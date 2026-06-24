using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MakeKits.Workshop.Webview;

public abstract class EmbeddedResourceWebpagePanel : WebpagePanel
{
    protected internal readonly Dictionary<string, byte[]> _resources;
    protected readonly byte[] _homePage;

    protected virtual string VirtualHost => "http://makekits.local/";

    protected virtual string HomePageResourcePath => "/index.html";

    protected EmbeddedResourceWebpagePanel()
    {
        _resources = EmbeddedResourceLoader.LoadResources(GetType().Assembly);
        _homePage = _resources[HomePageResourcePath];
    }

    public virtual void NavigateToHomePage()
    {
        NavigateToUri(new Uri(VirtualHost));
    }

    protected override void Webview_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        if (new Uri(e.Uri).Authority.Equals(new Uri(VirtualHost).Host, StringComparison.OrdinalIgnoreCase))
            return;

        base.Webview_NavigationStarting(sender, e);
    }

    protected override void WebView_WebResourceRequested(object? sender, CoreWebView2WebResourceRequestedEventArgs args)
    {
        Debug.WriteLine($"[{args.Request.Method}] {args.Request.Uri}");

        try
        {
            Uri requestedUri = new(args.Request.Uri);
            if (requestedUri.Scheme != Uri.UriSchemeHttp && requestedUri.Scheme != Uri.UriSchemeHttps)
                return;

            string absolutePath = Uri.UnescapeDataString(requestedUri.AbsolutePath);

            if (absolutePath == "/" && requestedUri.Authority.Equals(new Uri(VirtualHost).Host, StringComparison.OrdinalIgnoreCase))
            {
                args.Response = _webView.CoreWebView2.Environment.CreateWebResourceResponse(
                    new MemoryStream(_homePage), 200, "OK", MimeTypes.GetContentType(".html"));
            }

            if (TryCreateEmbeddedResourceResponse(absolutePath, out CoreWebView2WebResourceResponse? response))
                args.Response = response;
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
        }
    }

    protected virtual bool TryCreateEmbeddedResourceResponse(string absolutePath, out CoreWebView2WebResourceResponse? response)
    {
        response = null;

        if (!_resources.TryGetValue(absolutePath, out byte[]? content))
            return false;

        response = _webView.CoreWebView2.Environment.CreateWebResourceResponse(
            new MemoryStream(content), 200, "OK", MimeTypes.GetContentType(Path.GetExtension(absolutePath)));

        return true;
    }
}
