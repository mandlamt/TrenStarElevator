namespace TrenStarElevator.Domain.Exceptions;

public sealed class CapacityExceededException : ElevatorDomainException
{
    public string ElevatorId { get; }
    public int AttemptedPassengers { get; }
    public int MaxCapacity { get; }

    public CapacityExceededException(string elevatorId, int attemptedPassengers, int maxCapacity)
        : base($"Elevator {elevatorId} cannot accept {attemptedPassengers} passengers. Max capacity is {maxCapacity}.")
    {
        ElevatorId = elevatorId;
        AttemptedPassengers = attemptedPassengers;
        MaxCapacity = maxCapacity;
    }
}