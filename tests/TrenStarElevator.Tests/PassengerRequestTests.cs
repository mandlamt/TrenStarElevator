using TrenStarElevator.Domain.Entities;
using TrenStarElevator.Domain.Enums;
using Xunit;

namespace TrenStarElevator.Tests;

public class PassengerRequestTests
{
    [Fact]
    public void Constructor_WithSameOriginAndDestination_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => 
            new PassengerRequest(originFloor: 3, destinationFloor: 3, passengerCount: 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithNonPositivePassengerCount_ThrowsArgumentOutOfRangeException(int invalidCount)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => 
            new PassengerRequest(originFloor: 0, destinationFloor: 3, passengerCount: invalidCount));
    }

    [Theory]
    [InlineData(0, 5, Direction.Up)]
    [InlineData(5, 0, Direction.Down)]
    public void RequestedDirection_IsDerivedFromOriginAndDestination(int origin, int destination, Direction expected)
    {
        var request = new PassengerRequest(origin, destination, passengerCount: 1);

        Assert.Equal(expected, request.RequestedDirection);
    }

    [Fact]
    public void EachRequest_ReceivesAUniqueIncrementingId()
    {
        var first = new PassengerRequest(0, 5, 1);
        var second = new PassengerRequest(0, 5, 1);

        Assert.True(second.Id > first.Id);
    }
}