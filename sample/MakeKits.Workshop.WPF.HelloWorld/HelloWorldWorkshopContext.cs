namespace MakeKits.Workshop.WPF.HelloWorld;

public sealed class HelloWorldWorkshopContext : WorkshopContext
{
    public HelloWorldWorkshopContext()
    {
        Descriptor = new HelloWorldWorkshopDescriptor();
        ViewContext = new HelloWorldWorkshopViewContext();
    }
}
