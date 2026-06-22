using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using System.IO;

namespace MakeKits.Workshop.Webview2;

public abstract class EmbeddedResourceWebpagePanel : WebpagePanel
{
    protected readonly Dictionary<string, byte[]> EmbeddedResources;
    protected readonly byte[] HomePage;

    protected EmbeddedResourceWebpagePanel()
    {
        EmbeddedResources = EmbeddedResourceLoader.LoadResources(GetType().Assembly, ResourcePrefix);
        HomePage = EmbeddedResourceLoader.ReadResource(EmbeddedResources, HomePageResourcePath) ?? [];
    }

    protected abstract string ResourcePrefix { get; }

    protected virtual string VirtualHost => "http://makekits.local/";

    protected virtual string HomePageResourcePath => "/index.html";

    public virtual void NavigateToHomePage()
    {
        NavigateToUri(new Uri(VirtualHost));
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
            if (absolutePath == "/")
            {
                args.Response = _webView.CoreWebView2.Environment.CreateWebResourceResponse(
                    new MemoryStream(HomePage),
                    200,
                    "OK",
                    MimeTypes.GetContentType(".html"));
                return;
            }

            if (TryCreateEmbeddedResourceResponse(absolutePath, out CoreWebView2WebResourceResponse? response))
                args.Response = response;
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
        }

        base.WebView_WebResourceRequested(sender, args);
    }

    protected virtual bool TryCreateEmbeddedResourceResponse(
        string absolutePath,
        out CoreWebView2WebResourceResponse? response)
    {
        response = null;

        if (!EmbeddedResources.TryGetValue(absolutePath, out byte[]? content))
            return false;

        response = _webView.CoreWebView2.Environment.CreateWebResourceResponse(
            new MemoryStream(content),
            200,
            "OK",
            MimeTypes.GetContentType(Path.GetExtension(absolutePath)));

        return true;
    }
}
