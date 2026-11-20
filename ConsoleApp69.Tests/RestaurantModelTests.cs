using RestaurantProgram;
using RestaurantProgram.Enums;
using RestaurantProgram.Initialization;
using RestaurantProgram.Services;
using RestaurantProgram.UI;
using Xunit;

namespace RestaurantProgram.Tests;

public class RestaurantModelTests
{
    [Fact]
    public void MenuFindsDishById()
    {
        var menu = new Menu("Test menu");
        var dish = new Dish(1, "Borshch", "First course", 120, 350);
        menu.AddDish(dish);
        Assert.Same(dish, menu.FindDish(1));
    }

    [Fact]
    public void DiscountCalculatesTotalPrice()
    {
        var order = new Order
        {
            Id = 1, CustomerName = "Ivan", DishName = "Borshch",
            Quantity = 2, OriginalTotalPrice = 200
        };
        order.ApplyDiscount(25);
        Assert.Equal(150, order.TotalPrice);
        Assert.Equal(50, order.GetSavedMoney());
    }

    [Fact]
    public void TableReservationCanBeCancelled()
    {
        var table = new Table(1, 4);
        Assert.True(table.Reserve("Ivan", 3));
        Assert.False(table.IsFree);
        table.CancelReservation();
        Assert.True(table.IsFree);
    }

    [Fact]
    public void FileDataServiceReadsRestaurantDataFromJson()
    {
        var path = Path.Combine(Path.GetTempPath(), $"restaurant-{Guid.NewGuid():N}.json");
        File.WriteAllText(path, """
            { "dishes": [{ "id": 7, "name": "Soup", "category": "First", "price": 90, "weight": 300 }] }
            """);

        try
        {
            var data = new FileDataService().ReadRestaurantData(path);
            Assert.Single(data.Dishes);
            Assert.Equal("Soup", data.Dishes[0].Name);
            Assert.Equal(90, data.Dishes[0].Price);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void InitializerCreatesDefaultRestaurantState()
    {
        var state = new RestaurantDataInitializer().Initialize(null);

        Assert.Equal("La Piazza", state.Restaurant.Name);
        Assert.Equal(6, state.Menu.Dishes.Count);
        Assert.Single(state.Customers);
        Assert.Equal(5, state.Tables.Count);
        Assert.Empty(state.Orders);
    }

    [Fact]
    public void MainMenuMapsNumberToNamedEnumAndRejectsUnknownNumber()
    {
        var originalInput = Console.In;
        var originalOutput = Console.Out;
        try
        {
            Console.SetIn(new StringReader("7\n99\n"));
            Console.SetOut(new StringWriter());
            var menu = new MainMenu();

            Assert.Equal(MenuChoice.CreateOrder, menu.ReadChoice());
            Assert.Equal(MenuChoice.Invalid, menu.ReadChoice());
        }
        finally
        {
            Console.SetIn(originalInput);
            Console.SetOut(originalOutput);
        }
    }

    [Fact]
    public void TestPaymentMarksOrderAsPaidAndPreventsFurtherDiscounts()
    {
        var order = new Order
        {
            Id = 1,
            CustomerName = "Ivan",
            DishName = "Borshch",
            Quantity = 1,
            OriginalTotalPrice = 100
        };
        var originalOutput = Console.Out;
        try
        {
            Console.SetOut(new StringWriter());
            order.SetPaymentMethod(new CardPayment());
            Assert.True(order.ProcessPayment());
            Assert.True(order.IsPaid);
            order.ApplyDiscount(50);
            Assert.Equal(100, order.TotalPrice);
        }
        finally { Console.SetOut(originalOutput); }
    }

    [Fact]
    public void DishRejectsInvalidPriceAndWeight()
    {
        var dish = new Dish(1, "Soup", "First", 100, 300);
        Assert.Throws<ArgumentException>(() => dish.Price = double.NaN);
        Assert.Throws<ArgumentException>(() => dish.Weight = 0);
    }

    [Fact]
    public void SharedOrderServiceCreatesOrdersFromValidatedRestaurantEntities()
    {
        var state = new RestaurantDataInitializer().Initialize(null);
        var service = new RestaurantOrderService();

        var order = service.Create(state, customerId: 1, dishId: 1, quantity: 2);

        Assert.Equal(1, order.Id);
        Assert.Equal(1, order.CustomerId);
        Assert.Equal("Михайло", order.CustomerName);
        Assert.Equal(500, order.TotalPrice);
        Assert.Same(order, Assert.Single(state.Orders));
    }
}
