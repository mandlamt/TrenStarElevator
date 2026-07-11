using TrenStarElevator.Application.Interfaces;
using TrenStarElevator.Domain.Interfaces;

namespace TrenStarElevator.Infrastructure.Logging;

public sealed class ElevatorEventLoggerAdapter : IElevatorEventListener
{
    private readonly IElevatorLogger _logger;

    public ElevatorEventLoggerAdapter(IElevatorLogger logger)
    {
        _logger = logger;
    }

    public void OnElevatorEvent(ElevatorEvent elevatorEvent)
    {
        _logger.LogInfo($"[Elevator {elevatorEvent.ElevatorId}] {elevatorEvent.Message}");
    }
}