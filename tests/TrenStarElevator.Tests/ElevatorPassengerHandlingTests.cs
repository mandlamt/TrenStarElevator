using TrenStarElevator.Domain.Exceptions;
using TrenStarElevator.Tests.TestDoubles;
using Xunit;

namespace TrenStarElevator.Tests;

public class ElevatorPassengerHandlingTests
{
    [Fact]
    public void CanAccommodate_WhenUnderCapacity_ReturnsTrue()
    {
        var elevator = new TestElevator(id: "E-01", minFloor: 0, maxFloor: 10, maxCapacity: 4);

        Assert.True(elevator.CanAccommodate(4));
    }

    [Fact]
    public void CanAccommodate_WhenOverCapacity_ReturnsFalse()
    {
        var elevator = new TestElevator(id: "E-01", minFloor: 0, maxFloor: 10, maxCapacity: 4);

        Assert.False(elevator.CanAccommodate(5));
    }

    [Fact]
    public void BoardPassengers_WithinCapacity_IncreasesPassengerCount()
    {
        var elevator = new TestElevator(id: "E-01", minFloor: 0, maxFloor: 10, maxCapacity: 4);

        elevator.BoardPassengers(3);

        Assert.Equal(3, elevator.PassengerCount);
    }

    [Fact]
    public void BoardPassengers_ExceedingCapacity_ThrowsCapacityExceededException()
    {
        var elevator = new TestElevator(id: "E-01", minFloor: 0, maxFloor: 10, maxCapacity: 4);
        elevator.BoardPassengers(3);

        var ex = Assert.Throws<CapacityExceededException>(() => elevator.BoardPassengers(2));

        Assert.Equal("E-01", ex.ElevatorId);
        Assert.Equal(4, ex.MaxCapacity);
        Assert.Equal(3, elevator.PassengerCount); // Verifies state remained unchanged
    }

    [Fact]
    public void AlightPassengers_ReducesPassengerCount_AndNeverGoesNegative()
    {
        var elevator = new TestElevator(id: "E-01", minFloor: 0, maxFloor: 10, maxCapacity: 4);
        elevator.BoardPassengers(2);

        elevator.AlightPassengers(5);

        Assert.Equal(0, elevator.PassengerCount);
    }
}