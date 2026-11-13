using RestaurantProgram.Enums;

namespace RestaurantProgram.UI;

public sealed class MainMenu : IMainMenu
{
    public MenuChoice? ReadChoice()
    {
        if (!Console.IsOutputRedirected)
            Console.Clear();

        Console.WriteLine("==========================");
        Console.WriteLine("       LA PIAZZA");
        Console.WriteLine("==========================");
        Console.WriteLine("1. \u0406\u043d\u0444\u043e\u0440\u043c\u0430\u0446\u0456\u044f \u043f\u0440\u043e \u0440\u0435\u0441\u0442\u043e\u0440\u0430\u043d");
        Console.WriteLine("2. \u041f\u043e\u043a\u0430\u0437\u0430\u0442\u0438 \u043c\u0435\u043d\u044e");
        Console.WriteLine("3. \u0414\u043e\u0434\u0430\u0442\u0438 \u0441\u0442\u0440\u0430\u0432\u0443");
        Console.WriteLine("4. \u0412\u0438\u0434\u0430\u043b\u0438\u0442\u0438 \u0441\u0442\u0440\u0430\u0432\u0443");
        Console.WriteLine("5. \u0414\u043e\u0434\u0430\u0442\u0438 \u043a\u043b\u0456\u0454\u043d\u0442\u0430");
        Console.WriteLine("6. \u041f\u043e\u043a\u0430\u0437\u0430\u0442\u0438 \u043a\u043b\u0456\u0454\u043d\u0442\u0456\u0432");
        Console.WriteLine("7. \u0421\u0442\u0432\u043e\u0440\u0438\u0442\u0438 \u0437\u0430\u043c\u043e\u0432\u043b\u0435\u043d\u043d\u044f");
        Console.WriteLine("8. \u041f\u043e\u043a\u0430\u0437\u0430\u0442\u0438 \u0437\u0430\u043c\u043e\u0432\u043b\u0435\u043d\u043d\u044f");
        Console.WriteLine("9. \u0421\u043a\u0430\u0441\u0443\u0432\u0430\u0442\u0438 \u0437\u0430\u043c\u043e\u0432\u043b\u0435\u043d\u043d\u044f");
        Console.WriteLine("10. \u0417\u0430\u0431\u0440\u043e\u043d\u044e\u0432\u0430\u0442\u0438 \u0441\u0442\u043e\u043b\u0438\u043a");
        Console.WriteLine("11. \u041f\u043e\u043a\u0430\u0437\u0430\u0442\u0438 \u0441\u0442\u043e\u043b\u0438\u043a\u0438");
        Console.WriteLine("12. \u0421\u043a\u0430\u0441\u0443\u0432\u0430\u0442\u0438 \u0431\u0440\u043e\u043d\u044e\u0432\u0430\u043d\u043d\u044f");
        Console.WriteLine("13. \u041a\u0435\u0440\u0443\u0432\u0430\u043d\u043d\u044f \u0437\u043d\u0438\u0436\u043a\u043e\u044e");
        Console.WriteLine("0. \u0412\u0438\u0445\u0456\u0434");
        Console.WriteLine("==========================");
        Console.Write("\u041e\u0431\u0435\u0440\u0456\u0442\u044c \u0434\u0456\u044e: ");

        var input = Console.ReadLine();
        if (input is null)
            return null;

        if (int.TryParse(input, out var value) && Enum.IsDefined(typeof(MenuChoice), value))
            return (MenuChoice)value;

        Console.WriteLine("\u041d\u0435\u0432\u0456\u0440\u043d\u0438\u0439 \u0432\u0438\u0431\u0456\u0440. \u041f\u043e\u0432\u0435\u0440\u0442\u0430\u044e\u0441\u044c \u0434\u043e \u0433\u043e\u043b\u043e\u0432\u043d\u043e\u0433\u043e \u043c\u0435\u043d\u044e.");
        return MenuChoice.Invalid;
    }
}
