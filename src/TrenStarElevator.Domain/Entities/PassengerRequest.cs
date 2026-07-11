using System.Threading;
using TrenStarElevator.Domain.Enums;

namespace TrenStarElevator.Domain.Entities;

public sealed class PassengerRequest
{
    private static long _sequence;

    public long Id { get; }
    public int OriginFloor { get; }
    public int DestinationFloor { get; }
    public int PassengerCount { get; }
    public DateTime RequestedAtUtc { get; }

    public Direction RequestedDirection => DestinationFloor switch
    {
        var d when d > OriginFloor => Direction.Up,
        var d when d < OriginFloor => Direction.Down,
        _ => Direction.Idle
    };

    public PassengerRequest(int originFloor, int destinationFloor, int passengerCount)
    {
        if (originFloor == destinationFloor)
        {
            throw new ArgumentException("Origin and destination floors cannot be identical.", nameof(destinationFloor));
        }

        if (passengerCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(passengerCount), "Passenger count must be at least 1.");
        }

        Id = Interlocked.Increment(ref _sequence);
        OriginFloor = originFloor;
        DestinationFloor = destinationFloor;
        PassengerCount = passengerCount;
        RequestedAtUtc = DateTime.UtcNow;
    }

    public override string ToString() =>
        $"Request #{Id}: {PassengerCount} pax from floor {OriginFloor} -> {DestinationFloor} ({RequestedDirection})";
}