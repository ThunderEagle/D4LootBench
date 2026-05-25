using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace D4Loot.App.ViewModels.Conditions;

public sealed record PickerEntry(uint Hash, string DisplayName);

public sealed partial class PickerViewModel : ObservableObject
{
    private readonly IReadOnlyList<PickerEntry> _source;

    public ObservableCollection<PickerEntry> Selected { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredAvailable))]
    private string _searchText = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddItemCommand))]
    private PickerEntry? _selectedAvailable;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RemoveItemCommand))]
    private PickerEntry? _selectedCurrent;

    public PickerViewModel(IEnumerable<PickerEntry> source)
    {
        _source = source.OrderBy(e => e.DisplayName).ToList();
        Selected.CollectionChanged += (_, _) => OnPropertyChanged(nameof(FilteredAvailable));
    }

    public IEnumerable<PickerEntry> FilteredAvailable
    {
        get
        {
            var selectedHashes = Selected.Select(e => e.Hash).ToHashSet();
            var q = _source.Where(e => !selectedHashes.Contains(e.Hash));
            if (!string.IsNullOrWhiteSpace(SearchText))
                q = q.Where(e => e.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            return q;
        }
    }

    [RelayCommand(CanExecute = nameof(CanAdd))]
    private void AddItem()
    {
        if (SelectedAvailable is not null)
        {
            Selected.Add(SelectedAvailable);
            SelectedAvailable = null;
        }
    }

    private bool CanAdd() => SelectedAvailable is not null;

    [RelayCommand(CanExecute = nameof(CanRemove))]
    private void RemoveItem()
    {
        if (SelectedCurrent is not null)
        {
            Selected.Remove(SelectedCurrent);
            SelectedCurrent = null;
        }
    }

    private bool CanRemove() => SelectedCurrent is not null;
}
