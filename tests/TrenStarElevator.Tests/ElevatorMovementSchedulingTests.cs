using TrenStarElevator.Domain.Exceptions;
using TrenStarElevator.Tests.TestDoubles;
using Xunit;

namespace TrenStarElevator.Tests;

public class ElevatorMovementSchedulingTests
{
    [Fact]
    public void AddStop_WithFloorOutsideBuildingRange_ThrowsInvalidFloorException()
    {
        var elevator = new TestElevator(id: "E-01", minFloor: 0, maxFloor: 10);

        Assert.Throws<InvalidFloorException>(() => elevator.AddStop(11));
    }

    [Fact]
    public void AddStop_WithValidFloor_AppearsInPendingStops()
    {
        var elevator = new TestElevator(id: "E-01", minFloor: 0, maxFloor: 10, startingFloor: 0);

        elevator.AddStop(5);

        Assert.Contains(5, elevator.PendingStops);
    }

    [Fact]
    public void EstimatedFloorsToReach_WhenIdle_ReturnsAbsoluteDistance()
    {
        var elevator = new TestElevator(id: "E-01", minFloor: 0, maxFloor: 10, startingFloor: 4);

        Assert.Equal(6, elevator.EstimatedFloorsToReach(10));
        Assert.Equal(4, elevator.EstimatedFloorsToReach(0));
    }

    [Fact]
    public void StepSimulation_MovesElevatorOneFloorTowardsItsStop()
    {
        var elevator = new TestElevator(id: "E-01", minFloor: 0, maxFloor: 10, startingFloor: 0);
        elevator.AddStop(3);

        elevator.StepSimulation();

        Assert.Equal(1, elevator.CurrentFloor);
    }

    [Fact]
    public void StepSimulation_RepeatedlyCalled_EventuallyReachesDestinationAndClearsStop()
    {
        var elevator = new TestElevator(id: "E-01", minFloor: 0, maxFloor: 10, startingFloor: 0);
        elevator.AddStop(3);

        for (int i = 0; i < 10 && elevator.CurrentFloor != 3; i++)
        {
            elevator.StepSimulation();
        }

        Assert.Equal(3, elevator.CurrentFloor);
        Assert.DoesNotContain(3, elevator.PendingStops);
    }

    [Fact]
    public void StepSimulation_ArrivedAtFloor_RaisesArrivedAtFloorEvent()
    {
        var elevator = new TestElevator(id: "E-01", minFloor: 0, maxFloor: 10, startingFloor: 0);
        elevator.AddStop(1);

        int? arrivedFloor = null;
        elevator.ArrivedAtFloor += floor => arrivedFloor = floor;

        for (int i = 0; i < 5 && arrivedFloor is null; i++)
        {
            elevator.StepSimulation();
        }

        Assert.Equal(1, arrivedFloor);
    }

    [Fact]
    public void TakeOutOfService_MakesEstimatedFloorsToReachReturnMaxValue()
    {
        var elevator = new TestElevator(id: "E-01", minFloor: 0, maxFloor: 10);

        elevator.TakeOutOfService();

        Assert.Equal(int.MaxValue, elevator.EstimatedFloorsToReach(5));
    }
}