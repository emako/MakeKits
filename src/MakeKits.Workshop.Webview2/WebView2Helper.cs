using Microsoft.Web.WebView2.Core;

namespace MakeKits.Workshop.Webview2;

public static class WebView2Helper
{
    public static bool IsWebView2Available()
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
