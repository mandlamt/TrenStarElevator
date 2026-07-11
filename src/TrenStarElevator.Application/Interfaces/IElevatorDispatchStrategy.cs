using TrenStarElevator.Domain.Entities;
using TrenStarElevator.Domain.Interfaces;

namespace TrenStarElevator.Application.Interfaces;

public interface IElevatorDispatchStrategy
{
    IElevator? SelectElevator(IReadOnlyCollection<IElevator> candidates, PassengerRequest request);
}