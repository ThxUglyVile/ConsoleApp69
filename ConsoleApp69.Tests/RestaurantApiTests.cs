using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestaurantProgram.Initialization;
using RestaurantProgram.Services;
using RestaurantProgram.Web;
using Xunit;

namespace RestaurantProgram.Tests;

public sealed class RestaurantApiTests
{
    [Fact]
    public async Task CartCheckoutDiscountAndTestPaymentUseRestaurantDomainModels()
    {
        var state = new RestaurantDataInitializer().Initialize(null);
        var builder = WebApplication.CreateBuilder();
        builder.Logging.ClearProviders();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        await using var app = builder.Build();
        using var loggerFactory = LoggerFactory.Create(logging => logging.SetMinimumLevel(LogLevel.Warning));
        RestaurantApi.Map(app, state, loggerFactory.CreateLogger("ApiTests"), new RestaurantReportBuilder(), new RestaurantOrderService());

        await app.StartAsync();
        try
        {
            var addresses = app.Services.GetRequiredService<IServer>().Features
                .Get<IServerAddressesFeature>()!.Addresses;
            using var client = new HttpClient { BaseAddress = new Uri(addresses.Single()) };
            var customerResponse = await client.PostAsJsonAsync("/api/customers",
                new { name = "Тестовий клієнт", age = 25, phone = "09912345678" });
            customerResponse.EnsureSuccessStatusCode();
            var customer = await customerResponse.Content.ReadFromJsonAsync<CustomerView>();

            (await client.PostAsJsonAsync("/api/cart/items", new { dishId = 1, quantity = 2 })).EnsureSuccessStatusCode();
            var cart = await client.GetFromJsonAsync<CartView>("/api/cart");
            Assert.Equal(500, cart!.Total);

            var checkoutResponse = await client.PostAsJsonAsync("/api/cart/checkout", new { customerId = customer!.Id });
            Assert.Equal(HttpStatusCode.Created, checkoutResponse.StatusCode);
            var checkout = await checkoutResponse.Content.ReadFromJsonAsync<CheckoutView>();
            Assert.Single(checkout!.Orders);
            Assert.Equal(500, checkout.Total);

            var orderId = checkout.Orders[0].Id;
            var discountResponse = await client.PostAsJsonAsync($"/api/orders/{orderId}/discount", new { percent = 10 });
            discountResponse.EnsureSuccessStatusCode();
            var paymentResponse = await client.PostAsJsonAsync($"/api/orders/{orderId}/payment", new { method = "card-test" });
            paymentResponse.EnsureSuccessStatusCode();
            var payment = await paymentResponse.Content.ReadFromJsonAsync<PaymentView>();

            Assert.Equal("test", payment!.Mode);
            Assert.True(payment.Order.IsPaid);
            Assert.Equal(450, payment.Order.TotalPrice);
        }
        finally { await app.StopAsync(); }
    }

    [Fact]
    public async Task CartRejectsAnInvalidQuantity()
    {
        var state = new RestaurantDataInitializer().Initialize(null);
        var builder = WebApplication.CreateBuilder();
        builder.Logging.ClearProviders();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        await using var app = builder.Build();
        using var loggerFactory = LoggerFactory.Create(logging => logging.SetMinimumLevel(LogLevel.Warning));
        RestaurantApi.Map(app, state, loggerFactory.CreateLogger("ApiTests"), new RestaurantReportBuilder(), new RestaurantOrderService());

        await app.StartAsync();
        try
        {
            var address = app.Services.GetRequiredService<IServer>().Features
                .Get<IServerAddressesFeature>()!.Addresses.Single();
            using var client = new HttpClient { BaseAddress = new Uri(address) };
            var response = await client.PostAsJsonAsync("/api/cart/items", new { dishId = 1, quantity = 51 });
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        finally { await app.StopAsync(); }
    }

    private sealed record CustomerView(int Id);
    private sealed record CartView(double Total);
    private sealed record CheckoutView(List<OrderView> Orders, double Total);
    private sealed record PaymentView(string Mode, OrderView Order);
    private sealed record OrderView(int Id, double TotalPrice, bool IsPaid);
}
