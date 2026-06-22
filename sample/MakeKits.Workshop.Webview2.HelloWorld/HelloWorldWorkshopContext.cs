namespace MakeKits.Workshop.Webview2.HelloWorld;

public sealed class HelloWorldWorkshopContext : WorkshopContext
{
    public HelloWorldWorkshopContext()
    {
        Descriptor = new HelloWorldWorkshopDescriptor();
        ViewContext = new HelloWorldWorkshopViewContext();
    }
}
