namespace RestaurantProgram.Services;

/// <summary>Shared order creation rules for console commands and the web API.</summary>
public sealed class RestaurantOrderService : IRestaurantOrderService
{
    public Order Create(RestaurantState state, int customerId, int dishId, int quantity)
    {
        ArgumentNullException.ThrowIfNull(state);
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Кількість має бути більшою за нуль.");

        var customer = state.Customers.FirstOrDefault(item => item.Id == customerId)
            ?? throw new KeyNotFoundException("Клієнта не знайдено.");
        var dish = state.Menu.FindDish(dishId)
            ?? throw new KeyNotFoundException("Страву не знайдено.");
        var total = dish.Price * quantity;
        if (!double.IsFinite(total)) throw new ArgumentException("Загальна сума замовлення завелика.");

        var order = new Order
        {
            Id = state.Orders.Select(item => item.Id).DefaultIfEmpty(0).Max() + 1,
            CustomerId = customer.Id,
            CustomerName = customer.Name,
            DishName = dish.Name,
            Quantity = quantity,
            OriginalTotalPrice = total
        };
        state.Orders.Add(order);
        return order;
    }
}
