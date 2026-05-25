using CommunityToolkit.Mvvm.ComponentModel;
using D4Loot.Core.Data;
using D4Loot.Core.Models;

namespace D4Loot.App.ViewModels.Conditions;

public sealed partial class ItemTypeConditionViewModel : ConditionViewModel
{
    public PickerViewModel Picker { get; }

    public ItemTypeConditionViewModel()
    {
        Picker = new PickerViewModel(
            ItemTypeDatabase.All.Select(e => new PickerEntry(e.Hash, e.Name)));
        Picker.Selected.CollectionChanged += (_, _) => OnPropertyChanged(nameof(Summary));
    }

    public ItemTypeConditionViewModel(ItemTypeCondition m) : this()
    {
        foreach (var id in m.TypeIds)
            Picker.Selected.Add(new PickerEntry(id, ItemTypeDatabase.GetDisplayName(id)));
    }

    public override string TypeName => "Item Type";
    public override string Summary =>
        $"{Picker.Selected.Count} type{(Picker.Selected.Count == 1 ? "" : "s")}";

    public override Condition BuildModel() =>
        new ItemTypeCondition(Picker.Selected.Select(e => e.Hash).ToList());
}
