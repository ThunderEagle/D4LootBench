using System.Windows;
using D4Loot.App.Themes;

namespace D4Loot.App;

public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ThemeManager.Initialize();
    }
}
