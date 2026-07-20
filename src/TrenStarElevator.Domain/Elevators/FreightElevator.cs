using TrenStarElevator.Domain.Enums;

namespace TrenStarElevator.Domain.Elevators;
/*.I used the Strategy pattern:Implemented different behaviors FreightElevatorBehavior (normal speed, capacity 2000kg)*/
public sealed class FreightElevator : ElevatorBase
{
    public override ElevatorType Type => ElevatorType.Freight;

    // Cargo elevators move slower and hold their doors open longer for loading.
    protected override int FloorTravelTimeMs => 1800;
    protected override int DoorCycleTimeMs => 2500;

    public FreightElevator(string id, int minFloor, int maxFloor, int startingFloor = 0, int maxCapacity = 25)
        : base(id, maxCapacity, minFloor, maxFloor, startingFloor)
    {
    }
}
