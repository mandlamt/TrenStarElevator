using TrenStarElevator.Domain.Enums;

namespace TrenStarElevator.Domain.Elevators;
/*LSP (Liskov Substitution):
- Any elevator type can replace the base Elevator class
- All subclasses maintain base behavior
ISP (Interface Segregation):
- IPassengerTransport (for passenger elevators)
*/

public sealed class PassengerElevator : ElevatorBase
{
    public override ElevatorType Type => ElevatorType.Passenger;

    public PassengerElevator(string id, int minFloor, int maxFloor, int startingFloor = 0, int maxCapacity = 8)
        : base(id, maxCapacity, minFloor, maxFloor, startingFloor)
    {
    }
}
