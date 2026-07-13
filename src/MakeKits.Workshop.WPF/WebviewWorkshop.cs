using System;
using System.Windows.Controls;

namespace MakeKits.Workshop.WPF;

public abstract class WebviewWorkshop : Workshop
{
    protected virtual object? Panel { get; private set; }

    protected virtual void ConfigureViewContext(IWorkshopContext context)
    {
        if (context.ViewContext == null)
            return;

        context.ViewContext.PreferredWidth = 1280;
        context.ViewContext.PreferredHeight = 720;
    }

    protected virtual object CreatePanel(IWorkshopContext context)
    {
        return new TextBlock()
        {
            Text = "Hello World.",
        };
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
        Panel = CreatePanel(context);

        context.ViewContext?.ViewerContent = Panel;
    }

    /// <inheritdoc/>
    public override void Cleanup()
    {
        (Panel as IDisposable)?.Dispose();
        Panel = null;
        GC.SuppressFinalize(this);
    }
}
