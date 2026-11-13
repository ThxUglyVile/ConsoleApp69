using RestaurantProgram.Services;

namespace RestaurantProgram.Initialization;

public sealed class RestaurantDataInitializer : IRestaurantDataInitializer
{
    public RestaurantState Initialize(RestaurantData? importedData)
    {
        var restaurant = new Restaurant
        {
            Name = "La Piazza",
            Address = "\u0432\u0443\u043b. \u0426\u0435\u043d\u0442\u0440\u0430\u043b\u044c\u043d\u0430, 15",
            Cuisine = "\u0406\u0442\u0430\u043b\u0456\u0439\u0441\u044c\u043a\u0430",
            TableCount = importedData?.Tables.Count ?? 5
        };
        var menu = new Menu("\u041e\u0441\u043d\u043e\u0432\u043d\u0435 \u043c\u0435\u043d\u044e");
        var customers = new List<Customer>();
        var orders = new List<Order>();
        var tables = new List<Table>();

        if (importedData is not null)
        {
            foreach (var dish in importedData.Dishes)
                menu.Dishes.Add(new Dish(dish.Id, dish.Name, dish.Category, dish.Price, dish.Weight));
            foreach (var customer in importedData.Customers)
                customers.Add(new Customer(customer.Id, customer.Name, customer.Age, customer.Phone));
            foreach (var table in importedData.Tables)
                tables.Add(new Table(table.Number, table.Seats));
        }
        else
        {
            menu.Dishes.Add(new Dish(1, "\u041c\u0430\u0440\u0433\u0430\u0440\u0438\u0442\u0430", "\u041f\u0456\u0446\u0430", 250, 500));
            menu.Dishes.Add(new Dish(2, "\u041f\u0435\u043f\u0435\u0440\u043e\u043d\u0456", "\u041f\u0456\u0446\u0430", 300, 550));
            menu.Dishes.Add(new Dish(3, "\u041a\u0430\u0440\u0431\u043e\u043d\u0430\u0440\u0430", "\u041f\u0430\u0441\u0442\u0430", 220, 350));
            menu.Dishes.Add(new Drink(4, "\u041b\u0438\u043c\u043e\u043d\u0430\u0434", "\u041d\u0430\u043f\u043e\u0457", 80, 300, true));
            menu.Dishes.Add(new Drink(5, "\u041a\u0430\u043f\u0443\u0447\u0438\u043d\u043e", "\u041d\u0430\u043f\u043e\u0457", 95, 250, false));
            menu.Dishes.Add(new Drink(6, "\u0410\u0439\u0441-\u043b\u0430\u0442\u0435", "\u041d\u0430\u043f\u043e\u0457", 110, 350, true));
            customers.Add(new Customer(1, "\u041c\u0438\u0445\u0430\u0439\u043b\u043e", 21, "380502223344"));

            for (var number = 1; number <= 5; number++)
                tables.Add(new Table(number, number % 2 == 0 ? 4 : 2));
        }

        return new RestaurantState(restaurant, menu, customers, orders, tables);
    }
}
