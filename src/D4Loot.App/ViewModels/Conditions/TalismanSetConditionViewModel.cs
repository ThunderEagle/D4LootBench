using CommunityToolkit.Mvvm.ComponentModel;
using D4Loot.Core.Data;
using D4Loot.Core.Models;

namespace D4Loot.App.ViewModels.Conditions;

public sealed partial class TalismanSetConditionViewModel : ConditionViewModel
{
    // SetEntries semantics not fully understood — preserved for lossless round-trips.
    // TODO: expose SetEntries editing once wire format is fully mapped
    private readonly IReadOnlyList<TalismanSetEntry> _preservedSetEntries;

    public PickerViewModel Picker { get; }

    public TalismanSetConditionViewModel()
    {
        _preservedSetEntries = [];
        Picker = new PickerViewModel(
            TalismanSetDatabase.All.Select(e => new PickerEntry(e.Hash, e.Name)));
        Picker.Selected.CollectionChanged += (_, _) => OnPropertyChanged(nameof(Summary));
    }

    public TalismanSetConditionViewModel(TalismanSetCondition m) : this()
    {
        _preservedSetEntries = m.SetEntries;
        foreach (var id in m.SetIds)
            Picker.Selected.Add(new PickerEntry(id, TalismanSetDatabase.GetSetName(id)));
    }

    public override void ApplyClassFilter(PlayerClass playerClass)
    {
        if (playerClass == PlayerClass.All)
            Picker.SourceFilter = null;
        else
        {
            var allowed = TalismanSetDatabase.ForClass(playerClass.ToString())
                .Select(e => e.Hash)
                .ToHashSet();
            Picker.SourceFilter = e => allowed.Contains(e.Hash);
        }
    }

    public override string TypeName => "Talisman Set";
    public override string Summary => Picker.Selected.Count == 0
        ? "any set"
        : $"{Picker.Selected.Count} set{(Picker.Selected.Count == 1 ? "" : "s")}";

    public override Condition BuildModel() => new TalismanSetCondition
    {
        SetIds     = Picker.Selected.Select(e => e.Hash).ToList(),
        SetEntries = _preservedSetEntries
    };
}
