namespace TrenStarElevator.Domain.Interfaces;
/*SOLID OCP  IElevator interface allows adding new elevator types
Can add FreightElevator, HighSpeedElevator without changing core code
*/
public interface IElevator : 
    IElevatorStatus, 
    IElevatorMovement, 
    IElevatorPassengerHandling, 
    IElevatorEventSource
{
}
