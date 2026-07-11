using System.Threading;

namespace TrenStarElevator.Domain.Interfaces;

public interface IElevatorMovement
{
    event Action<int>? ArrivedAtFloor;

    void AddStop(int floor);

    int EstimatedFloorsToReach(int floor);

    void StepSimulation();

    Task StepAsync(CancellationToken cancellationToken);

    void TakeOutOfService();

    void ReturnToService();
}