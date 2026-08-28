#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace MakeKits.Workshop;

#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Local minimum contract stub: The assembly name/version should align with the official MakeKits.Workshop.Abstractions, only used to satisfy CLR dependency resolution when loads plugin DLLs.
/// On the business side, use pure reflection to read metadata, without compiling references to this type.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class WorkshopAttribute(string id) : Attribute
{
    public string Id { get; set; } = id;
}

public interface IWorkshopDescriptor
{
    public string Name { get; }

    public string Author { get; }

    public string Description { get; }
}

public interface IWorkshopLogger
{
    public void None(params object[] args);

    public void Trace(params object[] args);

    public void Debug(params object[] args);

    public void Information(params object[] args);

    public void Warning(params object[] args);

    public void Error(params object[] args);

    public void Critical(params object[] args);
}

public enum WorkshopTheme
{
    None = 0,
    System = 1,
    Dark = 2,
    Light = 3,
}

public interface IWorkshopViewContext
{
    public object? Source { get; set; }

    public object? Icon { get; set; }

    string? Title { get; set; }

    public object? ViewerContent { get; set; }

    public bool IsImmersiveMode { get; set; }

    public int PreferredWidth { get; set; }

    public int PreferredHeight { get; set; }

    public bool CanResize { get; set; }

    public WorkshopTheme Theme { get; set; }
}

public interface IWorkshopContext
{
    public IWorkshopDescriptor? Descriptor { get; set; }

    public IWorkshopLogger? Logger { get; set; }

    public IWorkshopViewContext? ViewContext { get; set; }

    public IDictionary<string, object?>? Properties { get; set; }
}

public interface IWorkshop
{
    public IWorkshopContext? Context { get; set; }

    public IWorkshopDescriptor Descriptor { get; }

    public void Init();

    public void Prepare(IWorkshopContext context);

    public void View(IWorkshopContext context);

    public void Cleanup();
}
