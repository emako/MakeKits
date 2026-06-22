namespace MakeKits.Workshop.Webview.HelloWorld;

public sealed class HelloWorldWorkshopContext : WorkshopContext
{
    public HelloWorldWorkshopContext()
    {
        Descriptor = new HelloWorldWorkshopDescriptor();
        ViewContext = new HelloWorldWorkshopViewContext();
    }
}
