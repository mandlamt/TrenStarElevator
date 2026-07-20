using TrenStarElevator.Domain.Entities;
using TrenStarElevator.Domain.Interfaces;

namespace TrenStarElevator.Application.Interfaces;

public interface IElevatorDispatchStrategy
{
    IElevator? SelectElevator(IReadOnlyCollection<IElevator> candidates, PassengerRequest request);
}

/* Dispatching
   - Nearest elevator is assigned
   - Full elevators are not assigned
   - Different directions handled correctly

} */
