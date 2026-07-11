namespace TrenStarElevator.Domain.Interfaces;

public interface IElevator : 
    IElevatorStatus, 
    IElevatorMovement, 
    IElevatorPassengerHandling, 
    IElevatorEventSource
{
}