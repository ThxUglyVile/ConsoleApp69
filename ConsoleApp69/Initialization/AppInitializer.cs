using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestaurantProgram.Services;
using RestaurantProgram.UI;

namespace RestaurantProgram.Initialization;

public static class AppInitializer
{
    public static ServiceProvider Build()
    {
        var workspaceRoot = GetWorkspaceRoot();
        return new ServiceCollection()
            .AddLogging(builder => builder.AddProvider(new FileLoggerProvider(
                Path.Combine(workspaceRoot, "logs", "restaurant.log"))))
            .AddSingleton<IFileDataService, FileDataService>()
            .AddSingleton<IReportBuilder, RestaurantReportBuilder>()
            .AddSingleton<IRestaurantDataInitializer, RestaurantDataInitializer>()
            .AddSingleton<IMainMenu, MainMenu>()
            .AddSingleton<IRestaurantConsole, RestaurantConsole>()
            .AddSingleton<IRestaurantApplication, RestaurantApplication>()
            .BuildServiceProvider();
    }

    public static string GetWorkspaceRoot()
    {
        foreach (var startPath in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
        {
            for (DirectoryInfo? directory = new(startPath);
                 directory is not null;
                 directory = directory.Parent)
            {
                if (File.Exists(Path.Combine(directory.FullName, "ConsoleApp69.slnx")))
                    return directory.FullName;
            }
        }

        return Directory.GetCurrentDirectory();
    }
}
