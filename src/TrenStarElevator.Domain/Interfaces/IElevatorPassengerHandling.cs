namespace TrenStarElevator.Domain.Interfaces;

public interface IElevatorPassengerHandling
{
    bool CanAccommodate(int passengerCount);

    void BoardPassengers(int passengerCount);

    void AlightPassengers(int passengerCount);
}