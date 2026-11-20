using System.Text;

namespace RestaurantProgram.Services;

public sealed class RestaurantReportBuilder : IReportBuilder
{
    public string Build(RestaurantState state)
    {
        var report = new StringBuilder();
        report.AppendLine("\u0417\u0412\u0406\u0422 LA PIAZZA");
        report.AppendLine($"\u0421\u0442\u0440\u0430\u0432 \u0443 \u043c\u0435\u043d\u044e: {state.Menu.Dishes.Count}");
        report.AppendLine($"\u041a\u043b\u0456\u0454\u043d\u0442\u0456\u0432: {state.Customers.Count}");
        report.AppendLine($"\u0417\u0430\u043c\u043e\u0432\u043b\u0435\u043d\u044c: {state.Orders.Count}");
        report.AppendLine($"\u041e\u043f\u043b\u0430\u0447\u0435\u043d\u043e: {state.Orders.Count(order => order.IsPaid)}");
        report.AppendLine($"\u041e\u0447\u0456\u043a\u0443\u044e\u0442\u044c \u043e\u043f\u043b\u0430\u0442\u0438: {state.Orders.Count(order => !order.IsPaid)}");
        report.AppendLine($"\u0414\u043e\u0445\u0456\u0434 (\u043e\u043f\u043b\u0430\u0447\u0435\u043d\u0456 \u0437\u0430\u043c\u043e\u0432\u043b\u0435\u043d\u043d\u044f): {state.Orders.Where(order => order.IsPaid).Sum(order => order.TotalPrice):F2} \u0433\u0440\u043d");
        report.AppendLine($"\u0421\u0442\u043e\u043b\u0438\u043a\u0456\u0432: {state.Tables.Count}");
        report.AppendLine("\u0421\u0442\u043e\u043b\u0438\u043a\u0438:");
        foreach (var table in state.Tables)
            report.AppendLine($"{table.Number};{table.Seats};{(table.IsFree ? "\u0412\u0456\u043b\u044c\u043d\u0438\u0439" : table.ReservedBy)}");
        report.AppendLine("\u0421\u0442\u0440\u0430\u0432\u0438:");
        foreach (var dish in state.Menu.Dishes)
            report.AppendLine($"{dish.Id};{dish.Name};{dish.Category};{dish.Price};{dish.Weight}");
        report.AppendLine("\u041a\u043b\u0456\u0454\u043d\u0442\u0438:");
        foreach (var customer in state.Customers)
            report.AppendLine($"{customer.Id};{customer.Name};{customer.Age};{customer.Phone}");
        report.AppendLine("\u0417\u0430\u043c\u043e\u0432\u043b\u0435\u043d\u043d\u044f:");
        foreach (var order in state.Orders)
            report.AppendLine($"{order.Id};{order.CustomerName};{order.DishName};{order.Quantity};{order.Discount};{order.TotalPrice};{(order.IsPaid ? "Paid" : "Pending")};{order.PaymentType}");
        return report.ToString();
    }
}
