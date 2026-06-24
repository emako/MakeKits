using System;

namespace MakeKits.Workshop.Webview;

public abstract class WebviewWorkshop : Workshop
{
    protected WebpagePanel? Panel { get; private set; }

    protected abstract WebpagePanel CreatePanel(IWorkshopContext context);

    protected virtual void ConfigureViewContext(IWorkshopContext context)
    {
        if (context.ViewContext == null)
            return;

        context.ViewContext.PreferredWidth = 1280;
        context.ViewContext.PreferredHeight = 720;
    }

    protected virtual void NavigatePanel(WebpagePanel panel, IWorkshopContext context)
    {
        context?.ViewContext?.ViewerContent = panel;
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
        NavigatePanel(Panel, context);

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
