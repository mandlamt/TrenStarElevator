using TrenStarElevator.Application.Interfaces;

namespace TrenStarElevator.Infrastructure.Logging;

public sealed class ConsoleElevatorLogger : IElevatorLogger
{
    private readonly object _lock = new();

    public void LogInfo(string message) 
        => Write(ConsoleColor.Gray, "INFO", message);

    public void LogWarning(string message) 
        => Write(ConsoleColor.Yellow, "WARN", message);

    public void LogError(string message, Exception? exception = null)
    {
        var msg = exception is null ? message : $"{message} (Exception: {exception.Message})";
        Write(ConsoleColor.Red, "ERROR", msg);
    }

    private void Write(ConsoleColor color, string level, string message)
    {
        lock (_lock)
        {
            var originalColor = Console.ForegroundColor;
            try
            {
                Console.ForegroundColor = color;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{level}] {message}");
            }
            finally
            {
                Console.ForegroundColor = originalColor;
            }
        }
    }
}