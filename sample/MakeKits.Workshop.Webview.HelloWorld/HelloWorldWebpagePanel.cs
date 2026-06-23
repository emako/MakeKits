namespace MakeKits.Workshop.Webview.HelloWorld;

/// <inheritdoc/>
public sealed class HelloWorldWebpagePanel : EmbeddedResourceWebpagePanel
{
    /// <inheritdoc/>
    protected override string ResourcePrefix => "MakeKits.Workshop.Webview.HelloWorld.Resources.";

    /// <inheritdoc/>
    protected override string HomePageResourcePath => "/index.html";
}
