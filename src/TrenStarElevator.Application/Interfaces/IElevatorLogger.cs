namespace TrenStarElevator.Application.Interfaces;

public interface IElevatorLogger
{
    void LogInfo(string message);
    void LogWarning(string message);
    void LogError(string message, Exception? exception = null);
}