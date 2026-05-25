using CommunityToolkit.Mvvm.ComponentModel;
using D4Loot.App.ViewModels;
using D4Loot.Core.Models;

namespace D4Loot.App.ViewModels.Conditions;

public sealed partial class CodexConditionViewModel : ConditionViewModel
{
    public override string TypeName => "Codex of Power";
    public override Condition BuildModel() => new CodexCondition();
}
