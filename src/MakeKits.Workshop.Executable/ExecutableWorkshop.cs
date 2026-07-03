using System;
using System.Diagnostics;
using System.Windows.Controls;

namespace MakeKits.Workshop.Executable;

public abstract class ExecutableWorkshop : Workshop
{
    /// <summary>
    /// Gets or sets the launch type of the workshop.
    /// </summary>
    public virtual LaunchType LaunchType { get; set; } = LaunchType.None;

    /// <summary>
    /// Gets or sets the directory of the program to be executed by the workshop.
    /// </summary>
    public virtual string CachesDirectory { get; set; } = null!;

    /// <summary>
    /// Gets or sets the directory of the program to be executed by the workshop.
    /// </summary>
    public virtual string ProgramDirectory { get; set; } = null!;

    /// <summary>
    /// Gets or sets the path of the program to be executed by the workshop.
    /// </summary>
    public virtual string ProgramPath { get; set; } = null!;

    /// <summary>
    /// Gets or sets the path of the package to be used by the workshop.
    /// </summary>
    public virtual string PackagePath { get; set; } = null!;

    /// <summary>
    /// Gets or sets the alias of the workshop.
    /// </summary>
    public virtual string Alias { get; set; } = "Package.zip";

    /// <summary>
    /// Gets or sets the name of the executable file.
    /// </summary>
    public virtual string ExecName { get; set; } = null!;

    protected DisposablePanel? Panel { get; private set; }

    protected abstract DisposablePanel CreatePanel(IWorkshopContext context);

    protected virtual void ConfigureViewContext(IWorkshopContext context)
    {
        if (context.ViewContext == null)
            return;

        context.ViewContext.PreferredWidth = 1280;
        context.ViewContext.PreferredHeight = 720;
    }

    protected virtual void NavigatePanel(DisposablePanel panel, IWorkshopContext context)
    {
        context?.ViewContext?.ViewerContent = panel;
    }

    protected virtual void FallbackPanel(string message, IWorkshopContext context)
    {
        // Initialize the panel with a simple message label
        Label label = new()
        {
            Content = message ?? "Error Occurred",
            HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
            VerticalContentAlignment = System.Windows.VerticalAlignment.Center,
            FontSize = 24,
            FontWeight = System.Windows.FontWeights.Bold,
        };
        context?.ViewContext?.ViewerContent = label;
    }

    /// <inheritdoc/>
    public override void Init()
    {
    }

    /// <inheritdoc/>
    public override void Prepare(IWorkshopContext context)
    {
        Context = context;
        ConfigureViewContext(context);
    }

    /// <inheritdoc/>
    public override void View(IWorkshopContext context)
    {
        try
        {
            Panel = CreatePanel(context);
            NavigatePanel(Panel, context);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            FallbackPanel(e.ToString(), context);
            return;
        }

        context.ViewContext?.ViewerContent = Panel;
    }

    /// <inheritdoc/>
    public override void Cleanup()
    {
        Panel?.Dispose();
        Panel = null;
        GC.SuppressFinalize(this);
    }
}
