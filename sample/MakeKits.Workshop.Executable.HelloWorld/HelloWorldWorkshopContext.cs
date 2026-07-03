namespace MakeKits.Workshop.Executable.HelloWorld;

public sealed class HelloWorldWorkshopContext : WorkshopContext
{
    public HelloWorldWorkshopContext()
    {
        Descriptor = new HelloWorldWorkshopDescriptor();
        ViewContext = new HelloWorldWorkshopViewContext();
    }
}
