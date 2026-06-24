namespace MakeKits.Workshop.Webview.HelloWorld;

public sealed class HelloWorldWebpagePanel : EmbeddedResourceWebpagePanel
{
    public override void InitializeResources()
    {
        ResourcesDictionary = EmbeddedResourceLoader.LoadResources(GetType().Assembly);
        HomePage = ResourcesDictionary[HomePageResourcePath];
    }
}
