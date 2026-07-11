using TrenStarElevator.Application.Interfaces;

namespace TrenStarElevator.Infrastructure.Logging;

public sealed class FileElevatorLogger : IElevatorLogger, IDisposable
{
    private readonly StreamWriter _writer;
    private readonly object _lock = new();

    public FileElevatorLogger(string filePath)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        _writer = new StreamWriter(filePath, append: true) { AutoFlush = true };
    }

    public void LogInfo(string message) 
        => Write("INFO", message);

    public void LogWarning(string message) 
        => Write("WARN", message);

    public void LogError(string message, Exception? exception = null)
    {
        var msg = exception is null ? message : $"{message} (Exception: {exception.Message})";
        Write("ERROR", msg);
    }

    private void Write(string level, string message)
    {
        lock (_lock)
        {
            _writer.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}");
        }
    }

    public void Dispose()
    {
        _writer?.Dispose();
    }
}