namespace TrenStarElevator.Domain.Interfaces;

public sealed record ElevatorEvent(string ElevatorId, DateTime TimestampUtc, string Message);

public interface IElevatorEventListener
{
    void OnElevatorEvent(ElevatorEvent elevatorEvent);
}

public interface IElevatorEventSource
{
    void Subscribe(IElevatorEventListener listener);
}