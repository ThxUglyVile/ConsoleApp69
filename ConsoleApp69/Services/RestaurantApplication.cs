using Microsoft.Extensions.Logging;

namespace RestaurantProgram.Services;

/// <summary>Coordinates console startup and receives dependencies through DI.</summary>
public sealed class RestaurantApplication(
    ILogger<RestaurantApplication> logger,
    IFileDataService fileData) : IRestaurantApplication
{
    public void Run(string[] args)
    {
        var originalInput = Console.In;
        try
        {
            var workspaceRoot = Program.GetWorkspaceRoot();
            var dataArgument = GetArgument(args, "--data");
            var inputArgument = GetArgument(args, "--input");
            var dataPath = dataArgument is null ? null : ResolvePath(dataArgument, workspaceRoot);
            var inputPath = inputArgument is null ? null : ResolvePath(inputArgument, workspaceRoot);
            var outputArgument = GetArgument(args, "--output");
            var outputPath = outputArgument is null
                ? Path.Combine(workspaceRoot, "report.txt")
                : ResolvePath(outputArgument, workspaceRoot);
            var initialData = dataPath is null ? null : fileData.ReadRestaurantData(dataPath);
            if (dataPath is not null)
                logger.LogInformation("Restaurant data loaded from {DataPath}", dataPath);

            if (inputPath is not null)
                Console.SetIn(new StringReader(string.Join(Environment.NewLine, fileData.ReadLines(inputPath))));

            Program.RunApplication(logger, initialData);

            fileData.WriteAllText(outputPath, Program.BuildReport());
            logger.LogInformation("Report saved to {OutputPath}", outputPath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Application execution failed");
            Console.Error.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            Console.SetIn(originalInput);
        }
    }

    private static string? GetArgument(string[] args, string option)
    {
        var index = Array.IndexOf(args, option);
        if (index < 0) return null;
        if (index + 1 >= args.Length || args[index + 1].StartsWith("--", StringComparison.Ordinal))
            throw new ArgumentException($"Option {option} requires a file path.");
        return args[index + 1];
    }

    private static string ResolvePath(string path, string workspaceRoot)
    {
        if (Path.IsPathRooted(path)) return path;
        return Path.GetFullPath(Path.Combine(workspaceRoot, path));
    }
}
