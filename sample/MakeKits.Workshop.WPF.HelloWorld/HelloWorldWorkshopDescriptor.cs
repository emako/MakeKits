namespace MakeKits.Workshop.WPF.HelloWorld;

public sealed class HelloWorldWorkshopDescriptor : IWorkshopDescriptor
{
    public string Name => Configuration.Name;

    public string? Author => Configuration.Author;

    public string? Description => Configuration.Description;
}
