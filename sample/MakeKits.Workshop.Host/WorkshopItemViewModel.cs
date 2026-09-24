using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MakeKits.Workshop.Host;

internal sealed partial class WorkshopItemViewModel : ObservableObject
{
    private const string OpenInNewWindowActionKey = "OpenInNewWindowAction";

    private readonly Action<IWorkshopItem> _open;
    private readonly Action<IWorkshopItem> _openInContentWindow;

    public WorkshopItemViewModel(
        IWorkshopItem item,
        Action<IWorkshopItem> open,
        Action<IWorkshopItem> openInContentWindow)
    {
        Item = item;
        _open = open;
        _openInContentWindow = openInContentWindow;
    }

    public IWorkshopItem Item { get; }

    public string Name => Item.Name;

    public string Author => Item.Author;

    public string Description => Item.Description;

    public string Version => Item.Version;

    [RelayCommand]
    private void Open() => _open(Item);

    [RelayCommand(CanExecute = nameof(CanOpenInNewWindow))]
    private void OpenInNewWindow()
    {
        if (TryGetOpenInNewWindowAction(out Func<bool>? runDirect) && runDirect != null)
        {
            try
            {
                if (!runDirect())
                    System.Windows.MessageBox.Show($"Failed to run {Name}.", "Open in New Window", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[WorkshopItemViewModel] Open in New Window error: {ex}");
                System.Windows.MessageBox.Show($"Failed to run {Name}.\n\n{ex.Message}", "Open in New Window", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            }

            return;
        }

        if (!IsWebOrConsoleWorkshop())
            return;

        try
        {
            _openInContentWindow(Item);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[WorkshopItemViewModel] Open in ContentWindow error: {ex}");
            System.Windows.MessageBox.Show($"Failed to open {Name}.\n\n{ex.Message}", "Open in New Window", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
        }
    }

    private bool CanOpenInNewWindow() =>
        TryGetOpenInNewWindowAction(out _) || IsWebOrConsoleWorkshop();

    private bool TryGetOpenInNewWindowAction(out Func<bool>? runDirect)
    {
        runDirect = null;

        IDictionary<string, object?>? properties = Item.Workshop?.Context?.Properties;
        if (properties == null)
            return false;

        if (!properties.TryGetValue(OpenInNewWindowActionKey, out object? value))
            return false;

        if (value is Func<bool> func)
        {
            runDirect = func;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Shared-project types are duplicated per workshop DLL, so detect by base type name.
    /// </summary>
    private bool IsWebOrConsoleWorkshop()
    {
        IWorkshop? workshop = Item.Workshop;
        if (workshop == null)
            return false;

        for (Type? type = workshop.GetType(); type != null; type = type.BaseType)
        {
            if (type.Name == "WebviewWorkshop")
                return true;

            if (type.Name == "ExecutableWorkshop")
            {
                object? launchType = workshop.GetType().GetProperty("LaunchType")?.GetValue(workshop);
                return string.Equals(launchType?.ToString(), "Console", StringComparison.Ordinal);
            }
        }

        return false;
    }
}
