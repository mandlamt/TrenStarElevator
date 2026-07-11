using TrenStarElevator.Domain.Enums;

namespace TrenStarElevator.Domain.Interfaces;

public interface IElevatorStatus
{
    string Id { get; }
    ElevatorType Type { get; }
    int CurrentFloor { get; }
    Direction CurrentDirection { get; }
    ElevatorOperationalState State { get; }
    int PassengerCount { get; }
    int MaxCapacity { get; }
    IReadOnlyCollection<int> PendingStops { get; }
}