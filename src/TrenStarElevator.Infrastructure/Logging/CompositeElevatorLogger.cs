using TrenStarElevator.Application.Interfaces;

namespace TrenStarElevator.Infrastructure.Logging;
/*. Contains:Logging,Database implementations,External services,File access,Future SQL repositories

Implements interfaces defined by Application.*/
/// <summary>
/// Combines several <see cref="IElevatorLogger"/> implementations behind a single instance
/// for example console + file, so the Application layer can log once while Infrastructure decides
/// how many, and which, sinks actually receive the message.
/// </summary>
public sealed class CompositeElevatorLogger : IElevatorLogger
{
    private readonly IReadOnlyList<IElevatorLogger> _loggers;

    public CompositeElevatorLogger(params IElevatorLogger[] loggers)
    {
        _loggers = loggers;
    }

    public void LogInfo(string message)
    {
        foreach (var logger in _loggers) logger.LogInfo(message);
    }

    public void LogWarning(string message)
    {
        foreach (var logger in _loggers) logger.LogWarning(message);
    }

    public void LogError(string message, Exception? exception = null)
    {
        foreach (var logger in _loggers) logger.LogError(message, exception);
    }
}
