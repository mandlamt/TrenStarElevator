using TrenStarElevator.Domain.Enums;

namespace TrenStarElevator.Domain.Elevators;
/*This means adding a new type = just add new behavior class
   - GlassElevatorBehavior (slow, high visibility) */
public sealed class GlassElevator : ElevatorBase
{
    public override ElevatorType Type => ElevatorType.Glass;

    // Holds doors open a bit longer to suit boarding.
    protected override int DoorCycleTimeMs => 1600;

    public GlassElevator(string id, int minFloor, int maxFloor, int startingFloor = 0, int maxCapacity = 8)
        : base(id, maxCapacity, minFloor, maxFloor, startingFloor)
    {
    }

    protected override void OnArrived(int floor)
    {
    }
}
