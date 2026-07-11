using TrenStarElevator.Domain.Exceptions;
using TrenStarElevator.Tests.TestDoubles;
using Xunit;

namespace TrenStarElevator.Tests;

public class ElevatorConstructionTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidId_ThrowsArgumentException(string? invalidId)
    {
        Assert.Throws<ArgumentException>(() => 
            new TestElevator(id: invalidId!, minFloor: 0, maxFloor: 10));
    }

    [Fact]
    public void Constructor_WithNonPositiveCapacity_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => 
            new TestElevator(id: "E-01", minFloor: 0, maxFloor: 10, maxCapacity: 0));
    }

    [Fact]
    public void Constructor_WithInvalidFloorRange_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => 
            new TestElevator(id: "E-01", minFloor: 5, maxFloor: 5));
    }

    [Fact]
    public void Constructor_WithStartingFloorOutsideRange_ThrowsInvalidFloorException()
    {
        Assert.Throws<InvalidFloorException>(() => 
            new TestElevator(id: "E-01", minFloor: 0, maxFloor: 10, startingFloor: 20));
    }

    [Fact]
    public void Constructor_WithValidArguments_SetsInitialState()
    {
        var elevator = new TestElevator(id: "E-07", minFloor: 0, maxFloor: 10, startingFloor: 3, maxCapacity: 6);

        Assert.Equal("E-07", elevator.Id);
        Assert.Equal(3, elevator.CurrentFloor);
        Assert.Equal(6, elevator.MaxCapacity);
        Assert.Equal(0, elevator.PassengerCount);
        Assert.Empty(elevator.PendingStops);
    }
}