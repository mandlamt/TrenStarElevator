namespace TrenStarElevator.Domain.Interfaces;
/*SOLID OCP  IElevator interface allows adding new elevator types*/
public interface IElevator : 
    IElevatorStatus, 
    IElevatorMovement, 
    IElevatorPassengerHandling, 
    IElevatorEventSource
{
}
