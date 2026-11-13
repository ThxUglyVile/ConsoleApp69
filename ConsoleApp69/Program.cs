using Microsoft.Extensions.DependencyInjection;
using RestaurantProgram.Initialization;
using RestaurantProgram.Services;

namespace RestaurantProgram;

internal static class Program
{
    private static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        using var services = AppInitializer.Build();
        services.GetRequiredService<IRestaurantApplication>().Run(args);
    }
}