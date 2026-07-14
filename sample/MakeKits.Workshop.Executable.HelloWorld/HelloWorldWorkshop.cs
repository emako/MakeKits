using MakeKits.Workshop;

namespace MakeKits.Workshop.Executable.HelloWorld;

public sealed class HelloWorldWorkshop : ExecutableWorkshop
{
    public override LaunchType LaunchType
    {
        get
        {
            if (Enum.TryParse(Configuration.LaunchType, out LaunchType launchType))
                return launchType;
            return LaunchType.Process;
        }
    }

    public override IWorkshopContext Context { get; set; } = new HelloWorldWorkshopContext();

    protected override WindowHostPanel CreatePanel(IWorkshopContext context)
    {
        HelloWorldWorkshopViewContext? viewContext = context.ViewContext as HelloWorldWorkshopViewContext;
        return new HelloWorldExecutablePanel();
    }

    protected override void ConfigureViewContext(IWorkshopContext context)
    {
        base.ConfigureViewContext(context);
    }
}
