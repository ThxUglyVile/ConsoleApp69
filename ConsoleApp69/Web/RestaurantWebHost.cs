using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestaurantProgram.Services;

namespace RestaurantProgram.Web;

/// <summary>Hosts the browser UI and API over the application's existing restaurant state.</summary>
public sealed class RestaurantWebHost(
    ILogger<RestaurantWebHost> logger,
    IReportBuilder reportBuilder,
    IRestaurantOrderService orderService) : IRestaurantWebHost
{
    public void Run(RestaurantState state)
    {
        var webRoot = Path.Combine(AppContext.BaseDirectory, "wwwroot");
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = [],
            ContentRootPath = AppContext.BaseDirectory,
            WebRootPath = webRoot
        });
        var address = Environment.GetEnvironmentVariable("RESTAURANT_WEB_URL") ?? "http://localhost:5080";
        builder.WebHost.UseUrls(address);

        var app = builder.Build();
        app.Use(async (context, next) =>
        {
            try
            {
                await next();
            }
            catch (Exception error)
            {
                logger.LogError(error, "Unhandled HTTP error for {Method} {Path}",
                    context.Request.Method, context.Request.Path);
                if (!context.Response.HasStarted)
                {
                    context.Response.Clear();
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsJsonAsync(new { message = "Внутрішня помилка сервера." });
                }
            }

            if (context.Response.StatusCode >= 400)
                logger.LogWarning("HTTP {StatusCode} for {Method} {Path}", context.Response.StatusCode,
                    context.Request.Method, context.Request.Path);
            else
                logger.LogInformation("HTTP {StatusCode} for {Method} {Path}", context.Response.StatusCode,
                    context.Request.Method, context.Request.Path);
        });
        app.UseDefaultFiles();
        app.UseStaticFiles();
        RestaurantApi.Map(app, state, logger, reportBuilder, orderService);

        logger.LogInformation("Restaurant web interface available at {Address}", address);
        Console.WriteLine($"La Piazza запущено у браузері: {address}");
        Console.WriteLine("Щоб зупинити сервер, натисніть Ctrl+C.");
        app.Run();
    }
}
