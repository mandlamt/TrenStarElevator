using TrenStarElevator.Domain.Enums;

namespace TrenStarElevator.Domain.Elevators;

public sealed class PassengerElevator : ElevatorBase
{
    public override ElevatorType Type => ElevatorType.Passenger;

    public PassengerElevator(string id, int minFloor, int maxFloor, int startingFloor = 0, int maxCapacity = 8)
        : base(id, maxCapacity, minFloor, maxFloor, startingFloor)
    {
    }
}
