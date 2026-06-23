namespace MakeKits.Workshop.Webview.HelloWorld;

public sealed class HelloWorldWebpagePanel : EmbeddedResourceWebpagePanel
{
    protected override string ResourcePrefix => "MakeKits.Workshop.Webview.HelloWorld.Resources.";

    protected override string HomePageResourcePath => "/index.html";
}
