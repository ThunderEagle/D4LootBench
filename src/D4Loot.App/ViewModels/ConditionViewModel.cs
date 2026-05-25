using CommunityToolkit.Mvvm.ComponentModel;
using D4Loot.Core.Models;

namespace D4Loot.App.ViewModels;

public abstract partial class ConditionViewModel : ObservableObject
{
    public abstract string TypeName { get; }
    public virtual string Summary => "";
    public abstract Condition BuildModel();
}
