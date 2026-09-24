using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MakeKits.Workshop.Host;

internal sealed partial class WorkshopItemViewModel : ObservableObject
{
    private const string OpenInNewWindowActionKey = "OpenInNewWindowAction";

    private readonly Action<IWorkshopItem> _open;

    public WorkshopItemViewModel(IWorkshopItem item, Action<IWorkshopItem> open)
    {
        Item = item;
        _open = open;
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
        if (!TryGetOpenInNewWindowAction(out Func<bool>? runDirect) || runDirect == null)
            return;

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
    }

    private bool CanOpenInNewWindow() => TryGetOpenInNewWindowAction(out _);

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
}
