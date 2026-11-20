namespace RestaurantProgram.Services;

public interface IRestaurantOrderService
{
    Order Create(RestaurantState state, int customerId, int dishId, int quantity);
}
