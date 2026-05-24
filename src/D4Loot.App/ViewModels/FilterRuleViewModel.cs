using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using D4Loot.App.Utilities;
using D4Loot.Core.Data;
using D4Loot.Core.Models;

namespace D4Loot.App.ViewModels;

/// <summary>A named color entry used in the color swatch palette.</summary>
public sealed class NamedColor(string name, uint abgr)
{
    public string          Name  { get; } = name;
    public uint            Abgr  { get; } = abgr;
    public SolidColorBrush Brush { get; } = new(ColorUtility.AbgrToWpf(abgr));
}

public partial class FilterRuleViewModel : ObservableObject
{
    private readonly Func<FilterRuleViewModel, IEnumerable<uint>> _getPeerColors;
    private SolidColorBrush? _wpfBrush;

    [ObservableProperty] private string     _name;
    [ObservableProperty] private Visibility _visibility;
    [ObservableProperty] private bool       _isEnabled;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(WpfBrush))]
    [NotifyPropertyChangedFor(nameof(ColorHex))]
    private uint _color;

    public ObservableCollection<ConditionViewModel> Conditions { get; } = [];

    public static IReadOnlyList<NamedColor> StandardColors { get; } =
    [
        new("Blue (Default)", FilterColors.Default),
        new("Cyan",           FilterColors.Cyan),
        new("Green",          FilterColors.Green),
        new("Orange",         FilterColors.Orange),
        new("Gold",           FilterColors.Gold)
    ];

    public static Visibility[] VisibilityValues { get; } = Enum.GetValues<Visibility>();

    public SolidColorBrush WpfBrush
    {
        get
        {
            _wpfBrush ??= new SolidColorBrush(ColorUtility.AbgrToWpf(Color));
            return _wpfBrush;
        }
    }

    public string ColorHex
    {
        get => Color.ToString("X8");
        set
        {
            if (uint.TryParse(value, NumberStyles.HexNumber, null, out var parsed))
                Color = parsed;
        }
    }

    public FilterRuleViewModel(FilterRule rule, Func<FilterRuleViewModel, IEnumerable<uint>>? getPeerColors = null)
    {
        _name          = rule.Name;
        _visibility    = rule.Visibility;
        _color         = rule.Color;
        _isEnabled     = rule.IsEnabled;
        _getPeerColors = getPeerColors ?? (_ => []);

        foreach (var condition in rule.Conditions)
            Conditions.Add(new ConditionViewModel(condition));
    }

    public FilterRule BuildRule() =>
        new(Name, Visibility, Color, Conditions.Select(c => c.Model).ToList(), IsEnabled);

    // value unused intentionally — only need to invalidate the cached brush on any change.
    // ReSharper disable once UnusedParameter.Local
    partial void OnColorChanging(uint value) => _wpfBrush = null;

    [RelayCommand]
    private void SelectColor(uint abgr) => Color = abgr;

    [RelayCommand]
    private void GenerateDistinctColor() =>
        Color = ColorUtility.GenerateDistinctColor(_getPeerColors(this));

    [RelayCommand]
    private void DeleteCondition(ConditionViewModel condition) => Conditions.Remove(condition);
}
