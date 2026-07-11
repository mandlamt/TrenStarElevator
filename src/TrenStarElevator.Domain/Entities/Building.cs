using TrenStarElevator.Domain.Exceptions;
using TrenStarElevator.Domain.Interfaces;

namespace TrenStarElevator.Domain.Entities;

public sealed class Building
{
    private readonly List<IElevator> _elevators = [];

    public string Name { get; }
    public int MinFloor { get; }
    public int MaxFloor { get; }
    public int TotalFloors => MaxFloor - MinFloor + 1;
    public IReadOnlyList<Floor> Floors { get; }

    public IReadOnlyList<IElevator> Elevators => _elevators;

    public Building(string name, int minFloor, int maxFloor)
    {
        if (minFloor >= maxFloor)
        {
            throw new ArgumentException("Minimum floor must be strictly less than maximum floor.");
        }

        Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Building name cannot be empty.", nameof(name)) : name;
        MinFloor = minFloor;
        MaxFloor = maxFloor;
        Floors = Enumerable.Range(MinFloor, TotalFloors).Select(number => new Floor(number)).ToList();
    }

    public void RegisterElevator(IElevator elevator)
    {
        ArgumentNullException.ThrowIfNull(elevator);

        if (_elevators.Any(e => e.Id == elevator.Id))
        {
            throw new ArgumentException($"Elevator ID {elevator.Id} is already registered to this building.", nameof(elevator));
        }

        _elevators.Add(elevator);
    }

    public void ValidateFloor(int floor)
    {
        if (floor < MinFloor || floor > MaxFloor)
        {
            throw new InvalidFloorException(floor, MinFloor, MaxFloor);
        }
    }
}