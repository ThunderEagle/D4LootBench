using System.Collections;
using System.Windows;

namespace D4Loot.App.Themes;

public enum AppTheme { Light, Dark, Diablo }

public static class ThemeManager
{
    private static readonly List<object> AppliedKeys = [];

    public static AppTheme Current { get; private set; } = AppTheme.Light;

    public static void Initialize() => Apply(AppTheme.Light);

    public static void Apply(AppTheme theme)
    {
        Current = theme;

        var incoming = new ResourceDictionary
        {
            Source = new Uri($"pack://application:,,,/Themes/{theme}Theme.xaml")
        };

        var resources = Application.Current.Resources;

        // Remove every key written by the previous theme
        foreach (var key in AppliedKeys)
            resources.Remove(key);
        AppliedKeys.Clear();

        // Write directly into Application.Current.Resources — guaranteed DynamicResource notifications
        foreach (DictionaryEntry entry in incoming)
        {
            resources[entry.Key] = entry.Value;
            AppliedKeys.Add(entry.Key);
        }

        // Visible confirmation that Apply fired — remove once theming is confirmed working
        if (Application.Current.MainWindow is { } w)
            w.Title = $"D4Loot — Filter Editor  [{theme}]";
    }
}
