using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MakeKits.Workshop.Host;

internal sealed partial class MainWindowViewModel : ObservableObject
{
    private readonly Action<IWorkshopItem> _openWorkshop;
    private readonly Action<IWorkshopItem> _openWorkshopInContentWindow;

    public MainWindowViewModel(
        Action<IWorkshopItem> openWorkshop,
        Action<IWorkshopItem> openWorkshopInContentWindow)
    {
        Workshops = [];
        _openWorkshop = openWorkshop;
        _openWorkshopInContentWindow = openWorkshopInContentWindow;
    }

    public ObservableCollection<WorkshopItemViewModel> Workshops { get; }

    public bool IsEmpty => Workshops.Count == 0;

    public string WorkshopCountText => Workshops.Count == 0
        ? string.Empty
        : $"{Workshops.Count} plugin(s)";

    public void LoadWorkshops(IReadOnlyList<IWorkshopItem> items)
    {
        Workshops.Clear();

        foreach (IWorkshopItem item in items)
            Workshops.Add(new WorkshopItemViewModel(item, _openWorkshop, _openWorkshopInContentWindow));

        OnPropertyChanged(nameof(IsEmpty));
        OnPropertyChanged(nameof(WorkshopCountText));
    }
}
