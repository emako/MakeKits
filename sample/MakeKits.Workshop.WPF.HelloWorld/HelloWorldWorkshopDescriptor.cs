namespace MakeKits.Workshop.WPF.HelloWorld;

public sealed class HelloWorldWorkshopDescriptor : WorkshopDescriptor
{
    public override string Name => Configuration.Name;
    public override string? Author => Configuration.Author;
    public override string? Description => Configuration.Description;
}
