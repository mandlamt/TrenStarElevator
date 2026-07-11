using TrenStarElevator.Domain.Elevators;
using TrenStarElevator.Domain.Enums;

namespace TrenStarElevator.Tests.TestDoubles;

/// <summary>
/// A minimal concrete elevator used purely for exercising <see cref="ElevatorBase"/> logic in unit tests.
/// Simulation intervals are minimal so the test suite runs instantly.
/// </summary>
internal sealed class TestElevator : ElevatorBase
{
    public override ElevatorType Type => ElevatorType.Passenger;

    protected override int FloorTravelTimeMs => 1;

    protected override int DoorCycleTimeMs => 1;

    public TestElevator(
        string id, 
        int minFloor, 
        int maxFloor, 
        int startingFloor = 0, 
        int maxCapacity = 4)
        : base(id, maxCapacity, minFloor, maxFloor, startingFloor)
    {
    }
}