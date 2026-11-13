namespace RestaurantProgram.UI;

internal static class MenuInput
{
    public static T? ReadEnum<T>(string prompt) where T : struct, Enum
    {
        Console.Write(prompt);
        var input = Console.ReadLine();
        if (input is not null && int.TryParse(input, out var value) && Enum.IsDefined(typeof(T), value))
            return (T)Enum.ToObject(typeof(T), value);

        Console.WriteLine("Invalid selection. Returning to the main menu.");
        return null;
    }
}
