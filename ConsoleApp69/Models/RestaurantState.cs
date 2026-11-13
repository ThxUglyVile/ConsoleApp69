namespace RestaurantProgram;

public sealed class RestaurantState(
    Restaurant restaurant,
    Menu menu,
    List<Customer> customers,
    List<Order> orders,
    List<Table> tables)
{
    public Restaurant Restaurant { get; } = restaurant;
    public Menu Menu { get; } = menu;
    public List<Customer> Customers { get; } = customers;
    public List<Order> Orders { get; } = orders;
    public List<Table> Tables { get; } = tables;
}
