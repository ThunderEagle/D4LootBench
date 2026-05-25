using CommunityToolkit.Mvvm.ComponentModel;
using D4Loot.Core.Models;

namespace D4Loot.App.ViewModels.Conditions;

public sealed partial class GreaterAffixConditionViewModel : ConditionViewModel
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Summary))]
    private int _minimumCount;

    public GreaterAffixConditionViewModel() { }

    public GreaterAffixConditionViewModel(GreaterAffixCondition m) =>
        _minimumCount = m.MinimumCount;

    public override string TypeName => "Greater Affix";
    public override string Summary => $"Min {MinimumCount}";
    public override Condition BuildModel() => new GreaterAffixCondition(MinimumCount);
}
