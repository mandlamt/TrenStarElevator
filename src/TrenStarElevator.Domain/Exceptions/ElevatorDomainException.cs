namespace TrenStarElevator.Domain.Exceptions;

public abstract class ElevatorDomainException : Exception
{
    protected ElevatorDomainException(string message) 
        : base(message)
    {
    }
}