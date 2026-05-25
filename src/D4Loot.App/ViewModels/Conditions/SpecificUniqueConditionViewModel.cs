using CommunityToolkit.Mvvm.ComponentModel;
using D4Loot.Core.Data;
using D4Loot.Core.Models;

namespace D4Loot.App.ViewModels.Conditions;

public sealed partial class SpecificUniqueConditionViewModel : ConditionViewModel
{
    public PickerViewModel Picker { get; }

    public SpecificUniqueConditionViewModel()
    {
        Picker = new PickerViewModel(
            UniqueItemDatabase.Released.Select(e => new PickerEntry(e.SnoId, e.Name)));
        Picker.Selected.CollectionChanged += (_, _) => OnPropertyChanged(nameof(Summary));
    }

    public SpecificUniqueConditionViewModel(SpecificUniqueCondition m) : this()
    {
        foreach (var id in m.UniqueIds)
            Picker.Selected.Add(new PickerEntry(id, UniqueItemDatabase.GetDisplayName(id)));
    }

    public override string TypeName => "Specific Unique";
    public override string Summary =>
        $"{Picker.Selected.Count} unique{(Picker.Selected.Count == 1 ? "" : "s")}";

    public override Condition BuildModel() =>
        new SpecificUniqueCondition(Picker.Selected.Select(e => e.Hash).ToList());
}
