using Microsoft.Extensions.Logging;
using RestaurantProgram.Enums;
using RestaurantProgram.Services;

namespace RestaurantProgram.UI;

public sealed class RestaurantConsole : IRestaurantConsole
{
    private readonly ILogger<RestaurantConsole> logger;
    private readonly IMainMenu mainMenu;
    private readonly IRestaurantOrderService orderService;
    private RestaurantState state = null!;

    private Restaurant restaurant => state.Restaurant;
    private Menu menu => state.Menu;
    private List<Customer> customers => state.Customers;
    private List<Order> orders => state.Orders;
    private List<Table> tables => state.Tables;

    public RestaurantConsole(ILogger<RestaurantConsole> logger, IMainMenu mainMenu, IRestaurantOrderService orderService)
    {
        this.logger = logger;
        this.mainMenu = mainMenu;
        this.orderService = orderService;
    }
    public void Run(RestaurantState initialState)
    {
        state = initialState;
        var isRunning = true;

        while (isRunning)
        {
            var selectedAction = mainMenu.ReadChoice();
            if (selectedAction is null)
            {
                logger.LogInformation("Input ended; stopping application");
                break;
            }

            logger.LogInformation("Selected menu action {Action}", selectedAction.Value);
            switch (selectedAction.Value)
            {
                case MenuChoice.Invalid:
                    break;
                case MenuChoice.Exit:
                    logger.LogInformation("Application stopped by user");
                    isRunning = false;
                    break;
                case MenuChoice.ShowRestaurantInformation:
                    restaurant.ShowInformation(); Pause(); break;
                case MenuChoice.ShowMenu:
                    menu.Show(); Pause(); break;
                case MenuChoice.AddDish:
                    AddDish(); break;
                case MenuChoice.RemoveDish:
                    RemoveDish(); break;
                case MenuChoice.AddCustomer:
                    AddCustomer(); break;
                case MenuChoice.ShowCustomers:
                    ShowCustomers(); Pause(); break;
                case MenuChoice.CreateOrder:
                    CreateOrder(); break;
                case MenuChoice.ShowOrders:
                    ShowOrders(); Pause(); break;
                case MenuChoice.CancelOrder:
                    CancelOrder(); break;
                case MenuChoice.ReserveTable:
                    ReserveTable(); break;
                case MenuChoice.ShowTables:
                    ShowTables(); Pause(); break;
                case MenuChoice.CancelTableReservation:
                    CancelTableReservation(); break;
                case MenuChoice.ManageDiscount:
                    ManageDiscount(); break;
                default:
                    logger.LogWarning("Unhandled menu action {Action}", selectedAction.Value);
                    break;
            }
        }
    }

    private void AddDish()
    {
        Console.WriteLine("\n===== ДОДАВАННЯ СТРАВИ =====");
        Console.WriteLine("0 — повернутися назад");

        string? name = EnterText("Назва: ");
        if (name == null) return;

        string? category = EnterText("Категорія: ");
        if (category == null) return;

        double price = EnterPositiveNumber("Ціна: ");
        if (price == -1) return;

        int weight = EnterPositiveInteger("Вага (г): ");
        if (weight == -1) return;

        if (menu.Dishes.Any(d =>
            d.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine("Страва з такою назвою вже існує.");
            Pause();
            return;
        }

        int id = menu.Dishes.Count == 0
            ? 1
            : menu.Dishes.Max(d => d.Id) + 1;

        menu.AddDish(new Dish
        {
            Id = id,
            Name = name,
            Category = category,
            Price = price,
            Weight = weight
        });

        Pause();
    }

    void RemoveDish()
    {
        menu.Show();

        if (menu.Dishes.Count == 0)
        {
            Pause();
            return;
        }

        int id = EnterPositiveInteger("\nID страви (0 — назад): ");
        if (id == -1) return;

        menu.RemoveDish(id);
        Pause();
    }

    void AddCustomer()
    {
        Console.WriteLine("\n===== ДОДАВАННЯ КЛІЄНТА =====");
        Console.WriteLine("0 — повернутися назад");

        string? name = EnterName("Ім'я: ");
        if (name == null) return;

        int age = EnterAge("Вік: ");
        if (age == -1) return;

        string? phone = EnterPhone("Телефон: ");
        if (phone == null) return;

        if (customers.Any(c => c.Phone == phone))
        {
            Console.WriteLine(
                "Клієнт з таким номером телефону вже існує.");
            Pause();
            return;
        }

        int id = customers.Count == 0
            ? 1
            : customers.Max(c => c.Id) + 1;

        customers.Add(new Customer
        {
            Id = id,
            Name = name,
            Age = age,
            Phone = phone
        });

        Console.WriteLine("Клієнта додано.");
        Pause();
    }

    void ShowCustomers()
    {
        Console.WriteLine("\n===== КЛІЄНТИ =====");

        if (customers.Count == 0)
        {
            Console.WriteLine("Клієнтів немає.");
            return;
        }

        foreach (Customer customer in customers)
        {
            customer.ShowInformation();
        }
    }

    void CreateOrder()
    {
        Console.WriteLine("\n===== СТВОРЕННЯ ЗАМОВЛЕННЯ =====");

        if (customers.Count == 0)
        {
            Console.WriteLine("Спочатку додайте клієнта.");
            Pause();
            return;
        }

        if (menu.Dishes.Count == 0)
        {
            Console.WriteLine("Меню порожнє.");
            Pause();
            return;
        }

        ShowCustomers();

        int customerId = EnterPositiveInteger(
            "\nID клієнта (0 — назад): ");

        if (customerId == -1) return;

        Customer? customer = customers.FirstOrDefault(
            c => c.Id == customerId);

        if (customer == null)
        {
            Console.WriteLine("Клієнта не знайдено.");
            Pause();
            return;
        }

        menu.Show();

        int dishId = EnterPositiveInteger(
            "\nID страви (0 — назад): ");

        if (dishId == -1) return;

        Dish? dish = menu.Dishes.FirstOrDefault(
            d => d.Id == dishId);

        if (dish == null)
        {
            Console.WriteLine("Страву не знайдено.");
            Pause();
            return;
        }

        int quantity = EnterPositiveInteger(
            "Кількість (0 — назад): ");

        if (quantity == -1) return;

        var order = orderService.Create(state, customer.Id, dish.Id, quantity);

        Console.WriteLine("Замовлення створено.");

        ChoosePayment(order);
        Pause();
    }

    void ShowOrders()
    {
        Console.WriteLine("\n===== ЗАМОВЛЕННЯ =====");

        if (orders.Count == 0)
        {
            Console.WriteLine("Замовлень немає.");
            return;
        }

        foreach (Order order in orders)
        {
            order.ShowInformation();
        }
    }

    void CancelOrder()
    {
        ShowOrders();

        if (orders.Count == 0)
        {
            Pause();
            return;
        }

        int id = EnterPositiveInteger(
            "\nID замовлення (0 — назад): ");

        if (id == -1) return;

        Order? order = orders.FirstOrDefault(o => o.Id == id);

        if (order == null)
        {
            Console.WriteLine("Замовлення не знайдено.");
        }
        else
        {
            orders.Remove(order);
            Console.WriteLine("Замовлення скасовано.");
        }

        Pause();
    }

    void ReserveTable()
    {
        ShowTables();

        int number = EnterPositiveInteger(
            "\nНомер столика (0 — назад): ");

        if (number == -1) return;

        Table? table = tables.FirstOrDefault(
            t => t.Number == number);

        if (table == null)
        {
            Console.WriteLine("Столик не знайдено.");
            Pause();
            return;
        }

        if (!table.IsFree)
        {
            Console.WriteLine(
                "Столик вже заброньований клієнтом: " +
                table.ReservedBy);
            Pause();
            return;
        }

        if (customers.Count == 0)
        {
            Console.WriteLine("Спочатку додайте клієнта.");
            Pause();
            return;
        }

        ShowCustomers();

        int customerId = EnterPositiveInteger(
            "\nID клієнта (0 — назад): ");

        if (customerId == -1) return;

        Customer? customer = customers.FirstOrDefault(
            c => c.Id == customerId);

        if (customer == null)
        {
            Console.WriteLine("Клієнта не знайдено.");
        }
        else if (table.Reserve(customer.Name))
        {
            Console.WriteLine(
                $"Столик №{table.Number} заброньовано для {customer.Name}.");
        }

        Pause();
    }

    void ShowTables()
    {
        Console.WriteLine("\n===== СТОЛИКИ =====");

        foreach (Table table in tables)
        {
            table.ShowInformation();
        }
    }

    void CancelTableReservation()
    {
        ShowTables();

        int number = EnterPositiveInteger(
            "\nНомер столика (0 — назад): ");

        if (number == -1) return;

        Table? table = tables.FirstOrDefault(
            t => t.Number == number);

        if (table == null)
        {
            Console.WriteLine("Столик не знайдено.");
        }
        else if (table.IsFree)
        {
            Console.WriteLine("Столик вільний.");
        }
        else
        {
            Console.WriteLine(
                $"Бронювання клієнта {table.ReservedBy} скасовано.");

            table.CancelReservation();
        }

        Pause();
    }

    private void ManageDiscount()
    {
        ShowOrders();
        if (orders.Count == 0)
        {
            Pause();
            return;
        }

        var id = EnterPositiveInteger("\nID \u0437\u0430\u043c\u043e\u0432\u043b\u0435\u043d\u043d\u044f (0 - \u043d\u0430\u0437\u0430\u0434): ");
        if (id == -1) return;

        var order = orders.FirstOrDefault(item => item.Id == id);
        if (order is null)
        {
            Console.WriteLine("\u0417\u0430\u043c\u043e\u0432\u043b\u0435\u043d\u043d\u044f \u043d\u0435 \u0437\u043d\u0430\u0439\u0434\u0435\u043d\u043e.");
            Pause();
            return;
        }

        Console.WriteLine("\n1. \u0412\u0441\u0442\u0430\u043d\u043e\u0432\u0438\u0442\u0438 \u0437\u043d\u0438\u0436\u043a\u0443");
        Console.WriteLine("2. \u0417\u0431\u0456\u043b\u044c\u0448\u0438\u0442\u0438 \u0437\u043d\u0438\u0436\u043a\u0443");
        Console.WriteLine("3. \u0417\u043c\u0435\u043d\u0448\u0438\u0442\u0438 \u0437\u043d\u0438\u0436\u043a\u0443");
        Console.WriteLine("4. \u041f\u043e\u0432\u043d\u0456\u0441\u0442\u044e \u0441\u043a\u0430\u0441\u0443\u0432\u0430\u0442\u0438 \u0437\u043d\u0438\u0436\u043a\u0443");
        Console.WriteLine("0. \u041d\u0430\u0437\u0430\u0434");
        var action = MenuInput.ReadEnum<DiscountMenuChoice>("\u041e\u0431\u0435\u0440\u0456\u0442\u044c \u0434\u0456\u044e: ");
        if (action is null || action == DiscountMenuChoice.Back) return;

        if (action == DiscountMenuChoice.Clear)
        {
            order.ApplyDiscount(0);
            Console.WriteLine("\u0417\u043d\u0438\u0436\u043a\u0443 \u043f\u043e\u0432\u043d\u0456\u0441\u0442\u044e \u0441\u043a\u0430\u0441\u043e\u0432\u0430\u043d\u043e.");
        }
        else if (action is DiscountMenuChoice.Set or DiscountMenuChoice.Increase or DiscountMenuChoice.Decrease)
        {
            var amount = EnterDiscount("\u0412\u0456\u0434\u0441\u043e\u0442\u043e\u043a (0 - \u043d\u0430\u0437\u0430\u0434): ");
            if (amount == -1) return;

            var newDiscount = action switch
            {
                DiscountMenuChoice.Set => amount,
                DiscountMenuChoice.Increase => order.Discount + amount,
                DiscountMenuChoice.Decrease => order.Discount - amount,
                _ => order.Discount
            };

            if (newDiscount < 0 || newDiscount > 100)
                Console.WriteLine("\u0417\u043d\u0438\u0436\u043a\u0430 \u043c\u0430\u0454 \u0431\u0443\u0442\u0438 \u0432\u0456\u0434 0 \u0434\u043e 100%.");
            else
            {
                order.ApplyDiscount(newDiscount);
                Console.WriteLine($"\u0417\u043d\u0438\u0436\u043a\u0430: {order.Discount}%. \u0414\u043e \u0441\u043f\u043b\u0430\u0442\u0438: {order.TotalPrice:F2} \u0433\u0440\u043d");
            }
        }

        Pause();
    }
    private string? EnterText(string prompt)
    {
        Console.Write(prompt);
        var value = Console.ReadLine()?.Trim();
        if (value is null || value == "0") return null;
        if (string.IsNullOrWhiteSpace(value) || !value.All(character => char.IsLetter(character) || character == ' '))
        {
            Console.WriteLine("\u0412\u0432\u0435\u0434\u0456\u0442\u044c \u043d\u0430\u0437\u0432\u0443 \u043b\u0438\u0448\u0435 \u043b\u0456\u0442\u0435\u0440\u0430\u043c\u0438.");
            return null;
        }
        if (!char.IsUpper(value[0]))
        {
            Console.WriteLine("\u041d\u0430\u0437\u0432\u0430 \u043c\u0430\u0454 \u043f\u043e\u0447\u0438\u043d\u0430\u0442\u0438\u0441\u044f \u0437 \u0432\u0435\u043b\u0438\u043a\u043e\u0457 \u043b\u0456\u0442\u0435\u0440\u0438.");
            return null;
        }
        return value;
    }

    private string? EnterName(string prompt) => EnterText(prompt);

    private string? EnterPhone(string prompt)
    {
        Console.Write(prompt);
        var phone = Console.ReadLine()?.Trim();
        if (phone is null || phone == "0") return null;
        if (phone.All(char.IsDigit) && phone.Length is >= 10 and <= 15) return phone;
        Console.WriteLine("\u0422\u0435\u043b\u0435\u0444\u043e\u043d \u043c\u0430\u0454 \u043c\u0456\u0441\u0442\u0438\u0442\u0438 10-15 \u0446\u0438\u0444\u0440.");
        return null;
    }

    private int EnterAge(string prompt)
    {
        Console.Write(prompt);
        var input = Console.ReadLine();
        if (input is not null && input != "0" && int.TryParse(input, out var age) && age is >= 1 and <= 120)
            return age;
        Console.WriteLine("\u0412\u0432\u0435\u0434\u0456\u0442\u044c \u0432\u0456\u043a \u0432\u0456\u0434 1 \u0434\u043e 120; \u043f\u043e\u0432\u0435\u0440\u0442\u0430\u044e\u0441\u044c \u0434\u043e \u043c\u0435\u043d\u044e.");
        return -1;
    }

    private int EnterPositiveInteger(string prompt)
    {
        Console.Write(prompt);
        var input = Console.ReadLine();
        if (input is not null && input != "0" && int.TryParse(input, out var number) && number > 0)
            return number;
        Console.WriteLine("\u0412\u0432\u0435\u0434\u0456\u0442\u044c \u0446\u0456\u043b\u0435 \u0447\u0438\u0441\u043b\u043e \u0431\u0456\u043b\u044c\u0448\u0435 0; \u043f\u043e\u0432\u0435\u0440\u0442\u0430\u044e\u0441\u044c \u0434\u043e \u043c\u0435\u043d\u044e.");
        return -1;
    }

    private double EnterPositiveNumber(string prompt)
    {
        Console.Write(prompt);
        var input = Console.ReadLine();
        if (input is not null && input != "0" && double.TryParse(input, out var number) && number > 0 && double.IsFinite(number))
            return number;
        Console.WriteLine("\u0412\u0432\u0435\u0434\u0456\u0442\u044c \u043a\u043e\u0440\u0435\u043a\u0442\u043d\u0435 \u0447\u0438\u0441\u043b\u043e \u0431\u0456\u043b\u044c\u0448\u0435 0; \u043f\u043e\u0432\u0435\u0440\u0442\u0430\u044e\u0441\u044c \u0434\u043e \u043c\u0435\u043d\u044e.");
        return -1;
    }

    private double EnterDiscount(string prompt)
    {
        Console.Write(prompt);
        var input = Console.ReadLine();
        if (input is not null && input != "0" && double.TryParse(input, out var discount) && discount is >= 0 and <= 100)
            return discount;
        Console.WriteLine("\u0412\u0432\u0435\u0434\u0456\u0442\u044c \u0437\u043d\u0438\u0436\u043a\u0443 \u0432\u0456\u0434 0 \u0434\u043e 100; \u043f\u043e\u0432\u0435\u0440\u0442\u0430\u044e\u0441\u044c \u0434\u043e \u043c\u0435\u043d\u044e.");
        return -1;
    }

    private static void Pause()
    {
        Console.WriteLine("\n\u041d\u0430\u0442\u0438\u0441\u043d\u0456\u0442\u044c Enter...");
        Console.ReadLine();
    }

    private void ChoosePayment(Order order)
    {
        Console.WriteLine("\n===== \u041e\u041f\u041b\u0410\u0422\u0410 \u0417\u0410\u041c\u041e\u0412\u041b\u0415\u041d\u041d\u042f =====");
        Console.WriteLine($"\u0421\u0443\u043c\u0430 \u0434\u043e \u0441\u043f\u043b\u0430\u0442\u0438: {order.TotalPrice:F2} \u0433\u0440\u043d");
        Console.WriteLine("1. \u0413\u043e\u0442\u0456\u0432\u043a\u0430");
        Console.WriteLine("2. \u0411\u0430\u043d\u043a\u0456\u0432\u0441\u044c\u043a\u0430 \u043a\u0430\u0440\u0442\u043a\u0430");
        Console.WriteLine("0. \u041e\u043f\u043b\u0430\u0442\u0438\u0442\u0438 \u043f\u0456\u0437\u043d\u0456\u0448\u0435");

        var selection = MenuInput.ReadEnum<PaymentMenuChoice>("\u041e\u0431\u0435\u0440\u0456\u0442\u044c \u0441\u043f\u043e\u0441\u0456\u0431 \u043e\u043f\u043b\u0430\u0442\u0438: ");
        if (selection is null || selection == PaymentMenuChoice.PayLater) return;

        IPaymentMethod? payment = selection switch
        {
            PaymentMenuChoice.Cash => new CashPayment(),
            PaymentMenuChoice.Card => new CardPayment(),
            _ => null
        };
        if (payment is null) return;

        order.SetPaymentMethod(payment);
        order.ProcessPayment();
    }
}
