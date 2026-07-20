namespace TrenStarElevator.Domain.Enums;
/*LSP (Liskov Substitution):
- Any elevator type can replace the base Elevator class
- All subclasses maintain base behavior*/

public enum ElevatorType
{
    Passenger = 0,
    HighSpeed = 1,
    Glass = 2,
    Freight = 3
}
