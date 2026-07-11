namespace TrenStarElevator.Domain.Exceptions;

public sealed class InvalidFloorException : ElevatorDomainException
{
    public int RequestedFloor { get; }
    public int MinFloor { get; }
    public int MaxFloor { get; }

    public InvalidFloorException(int requestedFloor, int minFloor, int maxFloor)
        : base($"Floor {requestedFloor} is out of bounds. Valid range is {minFloor} to {maxFloor}.")
    {
        RequestedFloor = requestedFloor;
        MinFloor = minFloor;
        MaxFloor = maxFloor;
    }
}