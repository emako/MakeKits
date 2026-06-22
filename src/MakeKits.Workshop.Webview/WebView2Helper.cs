using Microsoft.Web.WebView2.Core;
using System;

namespace MakeKits.Workshop.Webview;

public static class WebviewHelper
{
    public static bool IsWebviewAvailable()
    {
        try
        {
            return !string.IsNullOrEmpty(CoreWebView2Environment.GetAvailableBrowserVersionString());
        }
        catch (Exception)
        {
            return false;
        }
    }
}
