using System.Windows;
using D4Loot.App.Services;
using Microsoft.Extensions.DependencyInjection;

namespace D4Loot.App;

public partial class App
{
    public static IServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        Services = ServiceConfiguration.Build();
        base.OnStartup(e);

        var window = Services.GetRequiredService<MainWindow>();
        window.Show();
    }
}
