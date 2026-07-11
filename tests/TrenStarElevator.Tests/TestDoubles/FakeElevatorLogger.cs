using System.Collections.Concurrent;
using TrenStarElevator.Application.Interfaces;

namespace TrenStarElevator.Tests.TestDoubles;

internal sealed class FakeElevatorLogger : IElevatorLogger
{
    public ConcurrentQueue<string> InfoMessages { get; } = new();
    public ConcurrentQueue<string> WarningMessages { get; } = new();
    public ConcurrentQueue<string> ErrorMessages { get; } = new();

    public void LogInfo(string message) => InfoMessages.Enqueue(message);

    public void LogWarning(string message) => WarningMessages.Enqueue(message);

    public void LogError(string message, Exception? exception = null)
    {
        var logEntry = exception is null ? message : $"{message} | Exception: {exception.Message}";
        ErrorMessages.Enqueue(logEntry);
    }
}