using Microsoft.Web.WebView2.Core;
using System;
using System.Diagnostics;

namespace MakeKits.Workshop.Webview;

public class LocalHostWebpagePanel : WebpagePanel
{
    protected virtual string VirtualHost => "http://localhost:5173/";

    public virtual void NavigateToHomePage()
    {
        NavigateToUri(new Uri(VirtualHost));
    }

    protected override void WebView_WebResourceRequested(object? sender, CoreWebView2WebResourceRequestedEventArgs args)
    {
        Debug.WriteLine($"[{args.Request.Method}] {args.Request.Uri}");
    }
}
