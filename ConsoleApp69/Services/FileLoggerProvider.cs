using Microsoft.Extensions.Logging;

namespace RestaurantProgram.Services;

/// <summary>Провайдер ILogger, який записує події в окремий файл.</summary>
public sealed class FileLoggerProvider(string path) : ILoggerProvider
{
    private readonly object gate = new();
    public ILogger CreateLogger(string categoryName) => new FileLogger(categoryName, path, gate);
    public void Dispose() { }

    private sealed class FileLogger(string category, string filePath, object sync) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;
            var line = $"{DateTimeOffset.Now:O} [{logLevel}] {category}: {formatter(state, exception)}" +
                       (exception is null ? "" : Environment.NewLine + exception);
            lock (sync)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(filePath))!);
                File.AppendAllText(filePath, line + Environment.NewLine);
            }
        }
    }
}
