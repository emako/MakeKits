namespace MakeKits.Workshop.Webview2.HelloWorld;

public sealed class HelloWorldWebpagePanel : EmbeddedResourceWebpagePanel
{
    protected override string ResourcePrefix => "MakeKits.Workshop.Webview2.HelloWorld.Resources.";

    protected override string HomePageResourcePath => "/helloworld/index.html";
}
