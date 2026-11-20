using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RestaurantProgram.Services;

namespace RestaurantProgram.Web;

/// <summary>HTTP endpoints that use the application's existing domain models.</summary>
public static class RestaurantApi
{
    public static void Map(
        WebApplication app,
        RestaurantState state,
        ILogger logger,
        IReportBuilder reportBuilder,
        IRestaurantOrderService orderService)
    {
        app.MapGet("/api/restaurant", () => Results.Ok(new
        {
            name = state.Restaurant.Name,
            address = state.Restaurant.Address,
            cuisine = state.Restaurant.Cuisine,
            dishes = state.Menu.GetDishCount(),
            customers = state.Customers.Count,
            orders = state.Orders.Count,
            paidOrders = state.Orders.Count(order => order.IsPaid),
            pendingOrders = state.Orders.Count(order => !order.IsPaid),
            revenue = state.Orders.Where(order => order.IsPaid).Sum(order => order.TotalPrice),
            averageDishPrice = state.Menu.GetAveragePrice(),
            mostExpensiveDish = state.Menu.GetMostExpensiveDish()?.Name,
            tables = state.Tables.Count,
            availableTables = state.Tables.Count(table => table.IsFree)
        }));

        app.MapGet("/api/dishes", (string? search, string? category, string? sort) =>
        {
            IEnumerable<Dish> dishes = string.IsNullOrWhiteSpace(category)
                ? state.Menu.Dishes
                : state.Menu.GetByCategory(category.Trim());
            if (!string.IsNullOrWhiteSpace(search))
                dishes = dishes.Where(dish => dish.Name.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase) ||
                    dish.Category.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase));
            dishes = sort?.ToLowerInvariant() switch
            {
                "price-asc" => dishes.OrderBy(dish => dish.Price),
                "price-desc" => dishes.OrderByDescending(dish => dish.Price),
                "name" => dishes.OrderBy(dish => dish.Name),
                _ => dishes.OrderBy(dish => dish.Id)
            };
            return Results.Ok(dishes.Select(ToDishView));
        });
        app.MapGet("/api/dishes/{id:int}", (int id) =>
        {
            var dish = state.Menu.FindDish(id);
            return dish is null ? Results.NotFound(Error("Страву не знайдено.")) : Results.Ok(ToDishView(dish));
        });
        app.MapPost("/api/dishes", (DishRequest request) =>
        {
            var validation = ValidateDish(request);
            if (validation is not null) return Results.BadRequest(Error(validation));
            if (state.Menu.Dishes.Any(item => item.Name.Equals(request.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
                return Results.Conflict(Error("Страва з такою назвою вже існує."));
            var id = request.Id > 0 ? request.Id : NextId(state.Menu.Dishes.Select(dish => dish.Id));
            if (state.Menu.ContainsDish(id)) return Results.Conflict(Error("Страва з таким ID вже існує."));

            var dish = new Dish(id, request.Name, request.Category, request.Price, request.Weight);
            state.Menu.AddDish(dish);
            logger.LogInformation("Dish {DishId} added through web UI", dish.Id);
            return Results.Created($"/api/dishes/{dish.Id}", ToDishView(dish));
        });
        app.MapPut("/api/dishes/{id:int}", (int id, DishRequest request) =>
        {
            var dish = state.Menu.FindDish(id);
            if (dish is null) return Results.NotFound(Error("Страву не знайдено."));
            var validation = ValidateDish(request);
            if (validation is not null) return Results.BadRequest(Error(validation));
            if (state.Menu.Dishes.Any(item => item.Id != id && item.Name.Equals(request.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
                return Results.Conflict(Error("Страва з такою назвою вже існує."));

            dish.Name = request.Name;
            dish.Category = request.Category;
            dish.Price = request.Price;
            dish.Weight = request.Weight;
            logger.LogInformation("Dish {DishId} updated through web UI", id);
            return Results.Ok(ToDishView(dish));
        });
        app.MapDelete("/api/dishes/{id:int}", (int id) =>
        {
            var dish = state.Menu.FindDish(id);
            if (dish is null) return Results.NotFound(Error("Страву не знайдено."));
            if (state.Orders.Any(order => !order.IsPaid && order.DishName == dish.Name))
                return Results.Conflict(Error("Не можна видалити страву з неоплаченим замовленням."));
            state.Menu.RemoveDish(id);
            state.Cart.RemoveAll(item => item.DishId == id);
            logger.LogInformation("Dish {DishId} deleted through web UI", id);
            return Results.NoContent();
        });

        app.MapGet("/api/customers", () => Results.Ok(state.Customers));
        app.MapPost("/api/customers", (CustomerRequest request) =>
        {
            var validation = ValidateCustomer(request);
            if (validation is not null) return Results.BadRequest(Error(validation));
            if (state.Customers.Any(item => item.Phone == request.Phone.Trim()))
                return Results.Conflict(Error("Клієнт із таким номером телефону вже існує."));

            var customer = new Customer(NextId(state.Customers.Select(item => item.Id)),
                request.Name, request.Age, request.Phone);
            state.Customers.Add(customer);
            logger.LogInformation("Customer {CustomerId} added through web UI", customer.Id);
            return Results.Created($"/api/customers/{customer.Id}", customer);
        });
        app.MapPut("/api/customers/{id:int}", (int id, CustomerRequest request) =>
        {
            var customer = state.Customers.FirstOrDefault(item => item.Id == id);
            if (customer is null) return Results.NotFound(Error("Клієнта не знайдено."));
            var validation = ValidateCustomer(request);
            if (validation is not null) return Results.BadRequest(Error(validation));
            if (state.Customers.Any(item => item.Id != id && item.Phone == request.Phone.Trim()))
                return Results.Conflict(Error("Клієнт із таким номером телефону вже існує."));

            var oldName = customer.Name;
            customer.Name = request.Name;
            customer.Age = request.Age;
            customer.Phone = request.Phone;
            foreach (var table in state.Tables.Where(table => !table.IsFree && table.ReservedBy == oldName))
                table.ReservedBy = customer.Name;
            logger.LogInformation("Customer {CustomerId} updated through web UI", id);
            return Results.Ok(customer);
        });
        app.MapDelete("/api/customers/{id:int}", (int id) =>
        {
            var customer = state.Customers.FirstOrDefault(item => item.Id == id);
            if (customer is null) return Results.NotFound(Error("Клієнта не знайдено."));
            if (state.Orders.Any(order => order.CustomerId == customer.Id || order.CustomerName == customer.Name) ||
                state.Tables.Any(table => !table.IsFree && table.ReservedBy == customer.Name))
                return Results.Conflict(Error("Клієнта не можна видалити: він має замовлення або бронювання."));
            state.Customers.Remove(customer);
            logger.LogInformation("Customer {CustomerId} deleted through web UI", id);
            return Results.NoContent();
        });

        app.MapGet("/api/orders", () => Results.Ok(state.Orders.Select(ToOrderView)));
        app.MapGet("/api/orders/{id:int}", (int id) =>
        {
            var order = state.Orders.FirstOrDefault(item => item.Id == id);
            return order is null ? Results.NotFound(Error("Замовлення не знайдено.")) : Results.Ok(ToOrderView(order));
        });
        app.MapPost("/api/orders", (OrderRequest request) => CreateOrder(state, request, logger, orderService));
        app.MapPost("/api/orders/{id:int}/discount", (int id, DiscountRequest request) =>
        {
            var order = state.Orders.FirstOrDefault(item => item.Id == id);
            if (order is null) return Results.NotFound(Error("Замовлення не знайдено."));
            if (order.IsPaid) return Results.Conflict(Error("Не можна змінити знижку в оплаченому замовленні."));
            if (!double.IsFinite(request.Percent) || request.Percent is < 0 or > 100)
                return Results.BadRequest(Error("Знижка має бути від 0 до 100%."));
            order.ApplyDiscount(request.Percent);
            logger.LogInformation("Discount {Discount}% applied to order {OrderId}", request.Percent, id);
            return Results.Ok(ToOrderView(order));
        });
        app.MapPost("/api/orders/{id:int}/payment", (int id, PaymentRequest request) =>
        {
            var order = state.Orders.FirstOrDefault(item => item.Id == id);
            if (order is null) return Results.NotFound(Error("Замовлення не знайдено."));
            if (order.IsPaid) return Results.Conflict(Error("Замовлення вже оплачене."));
            if (order.TotalPrice <= 0) return Results.BadRequest(Error("Сума замовлення має бути більшою за нуль."));

            IPaymentMethod? method = request.Method?.Trim().ToLowerInvariant() switch
            {
                "cash" => new CashPayment(),
                "card-test" => new CardPayment(),
                _ => null
            };
            if (method is null) return Results.BadRequest(Error("Оберіть cash або card-test."));

            order.SetPaymentMethod(method);
            if (!order.ProcessPayment()) return Results.Problem("Тестовий платіж не пройшов.", statusCode: 409);
            logger.LogInformation("Test payment completed for order {OrderId} using {PaymentType}", id, method.GetPaymentInfo());
            return Results.Ok(new { order = ToOrderView(order), mode = "test", message = "Тестовий платіж: реальні кошти не списувалися." });
        });
        app.MapDelete("/api/orders/{id:int}", (int id) =>
        {
            var order = state.Orders.FirstOrDefault(item => item.Id == id);
            if (order is null) return Results.NotFound(Error("Замовлення не знайдено."));
            if (order.IsPaid) return Results.Conflict(Error("Оплачене замовлення не можна скасувати без процедури повернення."));
            state.Orders.Remove(order);
            logger.LogInformation("Order {OrderId} cancelled through web UI", id);
            return Results.NoContent();
        });

        app.MapGet("/api/cart", () => CartView(state));
        app.MapPost("/api/cart/items", (CartItemRequest request) =>
        {
            if (request.Quantity is < 1 or > 50) return Results.BadRequest(Error("Кількість має бути від 1 до 50."));
            var dish = state.Menu.FindDish(request.DishId);
            if (dish is null) return Results.NotFound(Error("Страву не знайдено."));
            var item = state.Cart.FirstOrDefault(line => line.DishId == request.DishId);
            if (item is not null && item.Quantity + request.Quantity > 50)
                return Results.BadRequest(Error("У кошику можна замовити не більше 50 одиниць однієї страви."));
            if (item is null) state.Cart.Add(new CartItem(request.DishId, request.Quantity));
            else item.Quantity += request.Quantity;
            logger.LogInformation("Dish {DishId} added to cart", request.DishId);
            return CartView(state);
        });
        app.MapPut("/api/cart/items/{dishId:int}", (int dishId, CartItemRequest request) =>
        {
            if (request.Quantity is < 1 or > 50) return Results.BadRequest(Error("Кількість має бути від 1 до 50."));
            var item = state.Cart.FirstOrDefault(line => line.DishId == dishId);
            if (item is null) return Results.NotFound(Error("Страви немає в кошику."));
            item.Quantity = request.Quantity;
            return CartView(state);
        });
        app.MapDelete("/api/cart/items/{dishId:int}", (int dishId) =>
        {
            var removed = state.Cart.RemoveAll(item => item.DishId == dishId);
            return removed == 0 ? Results.NotFound(Error("Страви немає в кошику.")) : CartView(state);
        });
        app.MapDelete("/api/cart", () =>
        {
            state.Cart.Clear();
            return CartView(state);
        });
        app.MapPost("/api/cart/checkout", (CheckoutRequest request) =>
        {
            var customer = state.Customers.FirstOrDefault(item => item.Id == request.CustomerId);
            if (customer is null) return Results.BadRequest(Error("Клієнта не знайдено."));
            if (state.Cart.Count == 0) return Results.BadRequest(Error("Кошик порожній."));

            var lines = state.Cart.Select(line => (Line: line, Dish: state.Menu.FindDish(line.DishId)))
                .ToList();
            if (lines.Any(entry => entry.Dish is null))
                return Results.Conflict(Error("Одна зі страв більше недоступна. Оновіть кошик."));

            var created = new List<Order>();
            try
            {
                foreach (var entry in lines)
                    created.Add(orderService.Create(state, customer.Id, entry.Dish!.Id, entry.Line.Quantity));
            }
            catch (ArgumentException error)
            {
                foreach (var order in created) state.Orders.Remove(order);
                return Results.BadRequest(Error(error.Message));
            }
            catch (KeyNotFoundException error)
            {
                foreach (var order in created) state.Orders.Remove(order);
                return Results.Conflict(Error(error.Message));
            }
            state.Cart.Clear();
            logger.LogInformation("Checkout created {OrderCount} orders for customer {CustomerId}", created.Count, customer.Id);
            return Results.Created("/api/orders", new
            {
                orders = created.Select(ToOrderView),
                total = created.Sum(order => order.TotalPrice),
                paymentMode = "test"
            });
        });

        app.MapGet("/api/tables", () => Results.Ok(state.Tables));
        app.MapPost("/api/tables/{number:int}/reservation", (int number, ReservationRequest request) =>
        {
            var table = state.Tables.FirstOrDefault(item => item.Number == number);
            if (table is null) return Results.NotFound(Error("Столик не знайдено."));
            var customer = state.Customers.FirstOrDefault(item => item.Id == request.CustomerId);
            if (customer is null) return Results.BadRequest(Error("Клієнта не знайдено."));
            if (!table.Reserve(customer.Name, request.Guests))
                return Results.Conflict(Error("Столик зайнятий або не вміщує стільки гостей."));
            logger.LogInformation("Table {TableNumber} reserved through web UI", number);
            return Results.Ok(table);
        });
        app.MapDelete("/api/tables/{number:int}/reservation", (int number) =>
        {
            var table = state.Tables.FirstOrDefault(item => item.Number == number);
            if (table is null) return Results.NotFound(Error("Столик не знайдено."));
            table.CancelReservation();
            logger.LogInformation("Reservation for table {TableNumber} cancelled through web UI", number);
            return Results.NoContent();
        });
        app.MapGet("/api/reports/current", () => Results.File(
            System.Text.Encoding.UTF8.GetBytes(reportBuilder.Build(state)), "text/plain; charset=utf-8", "restaurant-report.txt"));
    }

    private static IResult CreateOrder(
        RestaurantState state,
        OrderRequest request,
        ILogger logger,
        IRestaurantOrderService orderService)
    {
        if (request.Quantity is < 1 or > 50) return Results.BadRequest(Error("Кількість має бути від 1 до 50."));
        try
        {
            var order = orderService.Create(state, request.CustomerId, request.DishId, request.Quantity);
            logger.LogInformation("Order {OrderId} created through web UI", order.Id);
            return Results.Created($"/api/orders/{order.Id}", order);
        }
        catch (KeyNotFoundException error) { return Results.BadRequest(Error(error.Message)); }
        catch (ArgumentException error) { return Results.BadRequest(Error(error.Message)); }
    }

    private static IResult CartView(RestaurantState state)
    {
        var items = state.Cart.Select(line =>
        {
            var dish = state.Menu.FindDish(line.DishId);
            return dish is null ? null : new
            {
                dishId = dish.Id,
                name = dish.Name,
                category = dish.Category,
                price = dish.Price,
                quantity = line.Quantity,
                lineTotal = dish.Price * line.Quantity
            };
        }).Where(item => item is not null).ToList();
        return Results.Ok(new { items, total = items.Sum(item => item!.lineTotal) });
    }

    private static object ToDishView(Dish dish) => new
    {
        dish.Id,
        dish.Name,
        dish.Category,
        dish.Price,
        dish.Weight,
        description = dish.GetDescription(),
        isExpensive = dish.IsExpensive(300),
        promotionalPrice = dish.GetPriceWithDiscount(10)
    };

    private static object ToOrderView(Order order) => new
    {
        order.Id,
        order.CustomerId,
        order.CustomerName,
        order.DishName,
        order.Quantity,
        order.OriginalTotalPrice,
        order.TotalPrice,
        order.Discount,
        order.IsPaid,
        order.PaymentType,
        savedMoney = order.GetSavedMoney(),
        isLargeOrder = order.IsLargeOrder(1000)
    };

    private static string? ValidateDish(DishRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Trim().Length > 80)
            return "Назва страви обов'язкова (до 80 символів).";
        if (string.IsNullOrWhiteSpace(request.Category) || request.Category.Trim().Length > 40)
            return "Категорія обов'язкова (до 40 символів).";
        if (!double.IsFinite(request.Price) || request.Price < 0) return "Ціна має бути коректним невід'ємним числом.";
        if (request.Weight <= 0) return "Вага має бути більшою за нуль.";
        return null;
    }

    private static string? ValidateCustomer(CustomerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Trim().Length > 80)
            return "Ім'я клієнта обов'язкове (до 80 символів).";
        if (request.Age is < 1 or > 120) return "Вік має бути від 1 до 120.";
        var phone = request.Phone?.Trim() ?? "";
        if (phone.Length is < 10 or > 15 || phone.Any(character => !char.IsDigit(character)))
            return "Телефон має містити 10–15 цифр.";
        return null;
    }

    private static object Error(string message) => new { message };
    private static int NextId(IEnumerable<int> ids) => ids.DefaultIfEmpty(0).Max() + 1;

    public sealed record DishRequest(int Id, string Name, string Category, double Price, int Weight);
    public sealed record CustomerRequest(string Name, int Age, string Phone);
    public sealed record OrderRequest(int CustomerId, int DishId, int Quantity);
    public sealed record DiscountRequest(double Percent);
    public sealed record PaymentRequest(string Method);
    public sealed record CartItemRequest(int DishId, int Quantity);
    public sealed record CheckoutRequest(int CustomerId);
    public sealed record ReservationRequest(int CustomerId, int Guests);
}
