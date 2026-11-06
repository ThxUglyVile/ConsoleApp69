namespace RestaurantProgram.Services;

/// <summary>File access for input commands and generated reports.</summary>
public interface IFileDataService
{
    string[] ReadLines(string path);
    void WriteAllText(string path, string contents);
    RestaurantData ReadRestaurantData(string path);
}

public sealed class FileDataService : IFileDataService
{
    public string[] ReadLines(string path) => File.ReadAllLines(path);
    public void WriteAllText(string path, string contents) => File.WriteAllText(path, contents);

    public RestaurantData ReadRestaurantData(string path)
    {
        var json = File.ReadAllText(path);
        return System.Text.Json.JsonSerializer.Deserialize<RestaurantData>(json,
                   new System.Text.Json.JsonSerializerOptions
                   {
                       PropertyNameCaseInsensitive = true
                   })
               ?? throw new InvalidDataException("The data file is empty or contains invalid JSON.");
    }
}

/// <summary>Initial restaurant data imported from JSON.</summary>
public sealed class RestaurantData
{
    public List<DishData> Dishes { get; set; } = [];
    public List<CustomerData> Customers { get; set; } = [];
    public List<TableData> Tables { get; set; } = [];
}

public sealed class DishData
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public double Price { get; set; }
    public int Weight { get; set; }
}

public sealed class CustomerData
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Age { get; set; }
    public string Phone { get; set; } = "";
}

public sealed class TableData
{
    public int Number { get; set; }
    public int Seats { get; set; }
}
