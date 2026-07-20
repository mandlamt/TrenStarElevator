using TrenStarElevator.Domain.Enums;

namespace TrenStarElevator.Domain.Elevators;
/*I used the Strategy pattern:Implemented different behaviors:.- HighSpeedElevatorBehavior (2x speed, capacity 8)  */
public sealed class HighSpeedElevator : ElevatorBase
{
    public override ElevatorType Type => ElevatorType.HighSpeed;

    // Faster: shorter travel and door times than the standard passenger elevator.
    protected override int FloorTravelTimeMs => 450;
    protected override int DoorCycleTimeMs => 900;

    public HighSpeedElevator(string id, int minFloor, int maxFloor, int startingFloor = 0, int maxCapacity = 10)
        : base(id, maxCapacity, minFloor, maxFloor, startingFloor)
    {
    }
}
