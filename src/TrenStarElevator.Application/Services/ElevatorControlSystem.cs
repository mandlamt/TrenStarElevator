using System.Collections.Concurrent;
using TrenStarElevator.Application.Interfaces;
using TrenStarElevator.Domain.Entities;
using TrenStarElevator.Domain.Interfaces;

namespace TrenStarElevator.Application.Services;

public sealed class ElevatorControlSystem
{
    private sealed class AssignedLeg
    {
        public required PassengerRequest Request { get; init; }
        public bool HasBoarded { get; set; }
    }

    private readonly Building _building;
    private readonly IElevatorDispatchStrategy _dispatchStrategy;
    private readonly IElevatorLogger _logger;
    private readonly ConcurrentDictionary<string, List<AssignedLeg>> _assignments = new();
    private readonly ConcurrentQueue<PassengerRequest> _unassignedRequests = new();

    public IReadOnlyList<IElevator> Elevators => _building.Elevators;
    public Building Building => _building;

    public ElevatorControlSystem(Building building, IElevatorDispatchStrategy dispatchStrategy, IElevatorLogger logger)
    {
        _building = building ?? throw new ArgumentNullException(nameof(building));
        _dispatchStrategy = dispatchStrategy ?? throw new ArgumentNullException(nameof(dispatchStrategy));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        foreach (var elevator in building.Elevators)
        {
            _assignments[elevator.Id] = new List<AssignedLeg>();
            elevator.ArrivedAtFloor += floor => OnElevatorArrived(elevator, floor);
        }
    }

    public void RegisterRequest(PassengerRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        _building.ValidateFloor(request.OriginFloor);
        _building.ValidateFloor(request.DestinationFloor);

        TryAssign(request);
    }

    public void RetryUnassignedRequests()
    {
        int snapshotCount = _unassignedRequests.Count;

        for (int i = 0; i < snapshotCount; i++)
        {
            if (_unassignedRequests.TryDequeue(out var request))
            {
                TryAssign(request);
            }
        }
    }

    private void TryAssign(PassengerRequest request)
    {
        var elevator = _dispatchStrategy.SelectElevator(_building.Elevators, request);

        if (elevator is null)
        {
            _unassignedRequests.Enqueue(request);
            _logger.LogWarning($"No available elevator could immediately service {request}. Queued for retry.");
            return;
        }

        var legs = _assignments[elevator.Id];
        lock (legs)
        {
            legs.Add(new AssignedLeg { Request = request });
        }

        elevator.AddStop(request.OriginFloor);
        _logger.LogInfo($"Assigned {request} to elevator {elevator.Id}.");
    }

    private void OnElevatorArrived(IElevator elevator, int floor)
    {
        var legs = _assignments[elevator.Id];
        List<AssignedLeg> toBoard;
        List<AssignedLeg> toAlight;

        lock (legs)
        {
            toBoard = legs.Where(l => !l.HasBoarded && l.Request.OriginFloor == floor).ToList();
            toAlight = legs.Where(l => l.HasBoarded && l.Request.DestinationFloor == floor).ToList();
        }

        foreach (var leg in toBoard)
        {
            if (!elevator.CanAccommodate(leg.Request.PassengerCount))
            {
                _logger.LogWarning($"Elevator {elevator.Id} at floor {floor} cannot fit {leg.Request.PassengerCount} passenger(s). Remaining queued.");
                continue;
            }

            elevator.BoardPassengers(leg.Request.PassengerCount);
            elevator.AddStop(leg.Request.DestinationFloor);
            leg.HasBoarded = true;

            _logger.LogInfo($"Elevator {elevator.Id}: boarded {leg.Request.PassengerCount} passenger(s) bound for floor {leg.Request.DestinationFloor}.");
        }

        if (toAlight.Count == 0) return;

        lock (legs)
        {
            foreach (var leg in toAlight)
            {
                elevator.AlightPassengers(leg.Request.PassengerCount);
                legs.Remove(leg);
                _logger.LogInfo($"Elevator {elevator.Id}: {leg.Request.PassengerCount} passenger(s) arrived at floor {floor}.");
            }
        }
    }
}