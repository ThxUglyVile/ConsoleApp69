namespace RestaurantProgram.Services;

/// <summary>Файлове введення та виведення команд і звітів ресторану.</summary>
public interface IFileDataService
{
    string[] ReadLines(string path);
    void WriteAllText(string path, string contents);
}

public sealed class FileDataService : IFileDataService
{
    public string[] ReadLines(string path) => File.ReadAllLines(path);
    public void WriteAllText(string path, string contents) => File.WriteAllText(path, contents);
}
