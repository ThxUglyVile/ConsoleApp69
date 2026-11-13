using RestaurantProgram.Services;

namespace RestaurantProgram.Initialization;

public interface IRestaurantDataInitializer
{
    RestaurantState Initialize(RestaurantData? importedData);
}
