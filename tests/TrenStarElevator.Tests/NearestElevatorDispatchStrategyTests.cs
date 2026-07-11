using TrenStarElevator.Application.Strategies;
using TrenStarElevator.Domain.Entities;
using TrenStarElevator.Domain.Interfaces;
using TrenStarElevator.Tests.TestDoubles;
using Xunit;

namespace TrenStarElevator.Tests;

public class NearestElevatorDispatchStrategyTests
{
    [Fact]
    public void SelectElevator_PicksTheClosestIdleElevator()
    {
        var strategy = new NearestElevatorDispatchStrategy();
        var near = new TestElevator(id: "E-01", minFloor: 0, maxFloor: 10, startingFloor: 4);
        var far = new TestElevator(id: "E-02", minFloor: 0, maxFloor: 10, startingFloor: 9);
        var request = new PassengerRequest(originFloor: 5, destinationFloor: 8, passengerCount: 1);

        IElevator? selected = strategy.SelectElevator(new IElevator[] { near, far }, request);

        Assert.Same(near, selected);
    }

    [Fact]
    public void SelectElevator_SkipsElevatorsWithoutCapacity()
    {
        var strategy = new NearestElevatorDispatchStrategy();
        var full = new TestElevator(id: "E-01", minFloor: 0, maxFloor: 10, startingFloor: 5, maxCapacity: 2);
        full.BoardPassengers(2);
        var farButAvailable = new TestElevator(id: "E-02", minFloor: 0, maxFloor: 10, startingFloor: 9, maxCapacity: 4);
        var request = new PassengerRequest(originFloor: 5, destinationFloor: 8, passengerCount: 1);

        IElevator? selected = strategy.SelectElevator(new IElevator[] { full, farButAvailable }, request);

        Assert.Same(farButAvailable, selected);
    }

    [Fact]
    public void SelectElevator_SkipsOutOfServiceElevators()
    {
        var strategy = new NearestElevatorDispatchStrategy();
        var outOfService = new TestElevator(id: "E-01", minFloor: 0, maxFloor: 10, startingFloor: 5);
        outOfService.TakeOutOfService();
        var available = new TestElevator(id: "E-02", minFloor: 0, maxFloor: 10, startingFloor: 9);
        var request = new PassengerRequest(originFloor: 5, destinationFloor: 8, passengerCount: 1);

        IElevator? selected = strategy.SelectElevator(new IElevator[] { outOfService, available }, request);

        Assert.Same(available, selected);
    }

    [Fact]
    public void SelectElevator_WhenNoCandidateCanService_ReturnsNull()
    {
        var strategy = new NearestElevatorDispatchStrategy();
        var full = new TestElevator(id: "E-01", minFloor: 0, maxFloor: 10, maxCapacity: 1);
        full.BoardPassengers(1);
        var request = new PassengerRequest(originFloor: 5, destinationFloor: 8, passengerCount: 1);

        IElevator? selected = strategy.SelectElevator(new IElevator[] { full }, request);

        Assert.Null(selected);
    }
}