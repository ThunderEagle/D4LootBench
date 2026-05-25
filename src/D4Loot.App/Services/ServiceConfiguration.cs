using D4Loot.App.ViewModels;
using D4Loot.App.ViewModels.Conditions;
using D4Loot.Core.Data;
using D4Loot.Core.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace D4Loot.App.Services;

internal static class ServiceConfiguration
{
    public static IServiceProvider Build()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IFilterDataService, FilterDataService>();
        services.AddSingleton<IFilterValidator, FilterValidator>();
        services.AddSingleton<IConditionViewModelFactory, ConditionViewModelFactory>();

        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<MainWindow>();

        return services.BuildServiceProvider();
    }
}
