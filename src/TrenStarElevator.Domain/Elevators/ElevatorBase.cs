using System.Threading;
using TrenStarElevator.Domain.Enums;
using TrenStarElevator.Domain.Exceptions;
using TrenStarElevator.Domain.Interfaces;

namespace TrenStarElevator.Domain.Elevators;
/*This class does X, it follows SRP because Y”
Domain

Contains:Elevator entity,Floor information,Business rules
Domain models,No external dependencies.

SOLID SRP:Elevator class: Only handles movement, doors, passengers
*/
public abstract class ElevatorBase : IElevator
{
    private readonly int _minFloor;
    private readonly int _maxFloor;
    private readonly List<int> _pendingStops = [];
    private readonly List<IElevatorEventListener> _listeners = [];
    private ElevatorOperationalState _state = ElevatorOperationalState.Idle;

    public string Id { get; }
    public abstract ElevatorType Type { get; }
    public int CurrentFloor { get; private set; }
    public int PassengerCount { get; private set; }
    public int MaxCapacity { get; }
    public IReadOnlyCollection<int> PendingStops => _pendingStops;

    public ElevatorOperationalState State => _state;

    public Direction CurrentDirection
    {
        get
        {
            if (_state == ElevatorOperationalState.OutOfService || _pendingStops.Count == 0)
            {
                return Direction.Idle;
            }

            var target = NearestStop();
            return target switch
            {
                _ when target > CurrentFloor => Direction.Up,
                _ when target < CurrentFloor => Direction.Down,
                _ => Direction.Idle
            };
        }
    }

    public event Action<int>? ArrivedAtFloor;

    protected virtual int FloorTravelTimeMs => 1000;
    protected virtual int DoorCycleTimeMs => 1200;

    protected ElevatorBase(string id, int maxCapacity, int minFloor, int maxFloor, int startingFloor = 0)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Elevator id cannot be empty.", nameof(id));
        }

        if (maxCapacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxCapacity), "Maximum capacity must be at least 1.");
        }

        if (minFloor >= maxFloor)
        {
            throw new ArgumentException("Minimum floor must be strictly less than maximum floor.", nameof(minFloor));
        }

        if (startingFloor < minFloor || startingFloor > maxFloor)
        {
            throw new InvalidFloorException(startingFloor, minFloor, maxFloor);
        }

        Id = id;
        MaxCapacity = maxCapacity;
        _minFloor = minFloor;
        _maxFloor = maxFloor;
        CurrentFloor = startingFloor;
    }

    public void AddStop(int floor)
    {
        if (floor < _minFloor || floor > _maxFloor)
        {
            throw new InvalidFloorException(floor, _minFloor, _maxFloor);
        }

        if (!_pendingStops.Contains(floor))
        {
            _pendingStops.Add(floor);
        }
    }

    public int EstimatedFloorsToReach(int floor) =>
        _state == ElevatorOperationalState.OutOfService ? int.MaxValue : Math.Abs(CurrentFloor - floor);

    public void StepSimulation()
    {
        if (_state == ElevatorOperationalState.OutOfService)
        {
            return;
        }

        if (_pendingStops.Count == 0)
        {
            _state = ElevatorOperationalState.Idle;
            return;
        }

        var target = NearestStop();

        if (CurrentFloor < target)
        {
            CurrentFloor++;
        }
        else if (CurrentFloor > target)
        {
            CurrentFloor--;
        }

        if (CurrentFloor == target)
        {
            _pendingStops.Remove(target);
            OnArrived(CurrentFloor);
            ArrivedAtFloor?.Invoke(CurrentFloor);
            NotifyListeners(CurrentFloor);
        }

        _state = _pendingStops.Count == 0 ? ElevatorOperationalState.Idle : ElevatorOperationalState.Moving;
    }

    public async Task StepAsync(CancellationToken cancellationToken)
    {
        if (_state != ElevatorOperationalState.OutOfService && _pendingStops.Count > 0)
        {
            await Task.Delay(FloorTravelTimeMs, cancellationToken);
        }

        StepSimulation();
    }

    public void TakeOutOfService() => _state = ElevatorOperationalState.OutOfService;

    public void ReturnToService() =>
        _state = _pendingStops.Count == 0 ? ElevatorOperationalState.Idle : ElevatorOperationalState.Moving;

    public bool CanAccommodate(int passengerCount) =>
        _state != ElevatorOperationalState.OutOfService && PassengerCount + passengerCount <= MaxCapacity;

    public void BoardPassengers(int passengerCount)
    {
        var prospectiveTotal = PassengerCount + passengerCount;
        if (prospectiveTotal > MaxCapacity)
        {
            throw new CapacityExceededException(Id, prospectiveTotal, MaxCapacity);
        }

        PassengerCount = prospectiveTotal;
    }

    public void AlightPassengers(int passengerCount) => PassengerCount = Math.Max(0, PassengerCount - passengerCount);

    public void Subscribe(IElevatorEventListener listener) => _listeners.Add(listener);

    protected virtual void OnArrived(int floor)
    {
    }

    private int NearestStop() => _pendingStops.OrderBy(floor => Math.Abs(floor - CurrentFloor)).First();

    private void NotifyListeners(int floor)
    {
        if (_listeners.Count == 0)
        {
            return;
        }

        var elevatorEvent = new ElevatorEvent(Id, DateTime.UtcNow, $"Arrived at floor {floor}.");
        foreach (var listener in _listeners)
        {
            listener.OnElevatorEvent(elevatorEvent);
        }
    }
}
