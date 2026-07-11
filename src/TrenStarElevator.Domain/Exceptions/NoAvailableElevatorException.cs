namespace TrenStarElevator.Domain.Exceptions;

public sealed class NoAvailableElevatorException : ElevatorDomainException
{
    public NoAvailableElevatorException(string message) 
        : base(message)
    {
    }
}