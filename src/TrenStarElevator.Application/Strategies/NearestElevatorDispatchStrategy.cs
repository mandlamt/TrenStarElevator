using System;
using System.Collections.Generic;
using TrenStarElevator.Domain.Exceptions;
using TrenStarElevator.Application.Interfaces;
using TrenStarElevator.Domain.Entities;
using TrenStarElevator.Domain.Enums;
using TrenStarElevator.Domain.Interfaces;

namespace TrenStarElevator.Application.Strategies;

public class NearestElevatorDispatchStrategy : IElevatorDispatchStrategy
{
    public IElevator? SelectElevator(IReadOnlyCollection<IElevator> candidates, PassengerRequest request)
    {
        IElevator? bestCandidate = null;
        int bestScore = int.MaxValue;

        foreach (var elevator in candidates)
        {
            // Skip elevators that can't accommodate the incoming passenger count
            if (!elevator.CanAccommodate(request.PassengerCount))
            {
                continue;
            }

            int distance = Math.Abs(elevator.CurrentFloor - request.OriginFloor);
            int score = distance * 10;

            // Determine if the request direction aligns with the elevator
            Direction requestDir = request.DestinationFloor > request.OriginFloor ? Direction.Up : Direction.Down;

            // Penalize elevators moving in the opposite direction
            if (elevator.CurrentDirection != Direction.Idle && elevator.CurrentDirection != requestDir)
            {
                score += 15;
            }

            // Reward idle elevators slightly for faster response
            if (elevator.CurrentDirection == Direction.Idle)
            {
                score -= 2;
            }

            if (score < bestScore)
            {
                bestScore = score;
                bestCandidate = elevator;
            }
        }

        return bestCandidate;
    }
}