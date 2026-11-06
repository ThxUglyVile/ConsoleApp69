using RestaurantProgram;
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
}
