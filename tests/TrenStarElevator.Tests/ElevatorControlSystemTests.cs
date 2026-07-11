using System.Threading;
using TrenStarElevator.Application.Services;
using TrenStarElevator.Application.Strategies;
using TrenStarElevator.Domain.Entities;
using TrenStarElevator.Domain.Exceptions;
using TrenStarElevator.Tests.TestDoubles;
using Xunit;

namespace TrenStarElevator.Tests;

public class ElevatorControlSystemTests
{
    private static Building CreateBuilding(params TestElevator[] elevators)
    {
        var building = new Building("Test Tower", minFloor: 0, maxFloor: 10);
        foreach (var elevator in elevators)
        {
            building.RegisterElevator(elevator);
        }
        return building;
    }

    [Fact]
    public void RegisterRequest_WithFloorOutsideBuildingRange_ThrowsInvalidFloorException()
    {
        var elevator = new TestElevator(id: "E-01", minFloor: 0, maxFloor: 10);
        var building = CreateBuilding(elevator);
        var logger = new FakeElevatorLogger();
        var controlSystem = new ElevatorControlSystem(building, new NearestElevatorDispatchStrategy(), logger);

        Assert.Throws<InvalidFloorException>(() =>
            controlSystem.RegisterRequest(new PassengerRequest(originFloor: 0, destinationFloor: 99, passengerCount: 1)));
    }

    [Fact]
    public void RegisterRequest_AssignsElevatorAndSchedulesOriginStop()
    {
        var elevator = new TestElevator(id: "E-01", minFloor: 0, maxFloor: 10, startingFloor: 0);
        var building = CreateBuilding(elevator);
        var logger = new FakeElevatorLogger();
        var controlSystem = new ElevatorControlSystem(building, new NearestElevatorDispatchStrategy(), logger);

        controlSystem.RegisterRequest(new PassengerRequest(originFloor: 4, destinationFloor: 8, passengerCount: 2));

        Assert.Contains(4, elevator.PendingStops);
        Assert.Contains(logger.InfoMessages, m => m.Contains("Assigned"));
    }

    [Fact]
    public async Task ArrivalAtOriginFloor_BoardsPassengersAndSchedulesDestinationStop()
    {
        var elevator = new TestElevator(id: "E-01", minFloor: 0, maxFloor: 10, startingFloor: 0, maxCapacity: 4);
        var building = CreateBuilding(elevator);
        var logger = new FakeElevatorLogger();
        var controlSystem = new ElevatorControlSystem(building, new NearestElevatorDispatchStrategy(), logger);

        controlSystem.RegisterRequest(new PassengerRequest(originFloor: 2, destinationFloor: 5, passengerCount: 2));

        for (int i = 0; i < 20 && elevator.CurrentFloor != 2; i++)
        {
            await elevator.StepAsync(CancellationToken.None);
        }

        Assert.Equal(2, elevator.CurrentFloor);
        Assert.Equal(2, elevator.PassengerCount);
        Assert.Contains(5, elevator.PendingStops);
    }

    [Fact]
    public async Task ArrivalAtDestinationFloor_AlightsPassengers()
    {
        var elevator = new TestElevator(id: "E-01", minFloor: 0, maxFloor: 10, startingFloor: 0, maxCapacity: 4);
        var building = CreateBuilding(elevator);
        var logger = new FakeElevatorLogger();
        var controlSystem = new ElevatorControlSystem(building, new NearestElevatorDispatchStrategy(), logger);

        controlSystem.RegisterRequest(new PassengerRequest(originFloor: 1, destinationFloor: 3, passengerCount: 2));

        for (int i = 0; i < 40 && elevator.CurrentFloor != 3; i++)
        {
            await elevator.StepAsync(CancellationToken.None);
        }

        Assert.Equal(3, elevator.CurrentFloor);
        Assert.Equal(0, elevator.PassengerCount);
    }

    [Fact]
    public void RegisterRequest_WhenNoElevatorHasCapacity_QueuesRequestAndLogsWarning()
    {
        var elevator = new TestElevator(id: "E-01", minFloor: 0, maxFloor: 10, startingFloor: 0, maxCapacity: 1);
        elevator.BoardPassengers(1);
        var building = CreateBuilding(elevator);
        var logger = new FakeElevatorLogger();
        var controlSystem = new ElevatorControlSystem(building, new NearestElevatorDispatchStrategy(), logger);

        controlSystem.RegisterRequest(new PassengerRequest(originFloor: 4, destinationFloor: 8, passengerCount: 1));

        Assert.DoesNotContain(4, elevator.PendingStops);
        Assert.Contains(logger.WarningMessages, m => m.Contains("Queued for retry"));
    }

    [Fact]
    public void RetryUnassignedRequests_AfterCapacityFrees_AssignsQueuedRequest()
    {
        var elevator = new TestElevator(id: "E-01", minFloor: 0, maxFloor: 10, startingFloor: 0, maxCapacity: 1);
        elevator.BoardPassengers(1);
        var building = CreateBuilding(elevator);
        var logger = new FakeElevatorLogger();
        var controlSystem = new ElevatorControlSystem(building, new NearestElevatorDispatchStrategy(), logger);

        controlSystem.RegisterRequest(new PassengerRequest(originFloor: 4, destinationFloor: 8, passengerCount: 1));
        Assert.DoesNotContain(4, elevator.PendingStops);

        elevator.AlightPassengers(1); // Free up capacity
        controlSystem.RetryUnassignedRequests();

        Assert.Contains(4, elevator.PendingStops);
    }
}