namespace RestaurantProgram;

/// <summary>A server-side shopping cart line for the single local restaurant session.</summary>
public sealed class CartItem(int dishId, int quantity)
{
    public int DishId { get; } = dishId;
    public int Quantity { get; set; } = quantity;
}
