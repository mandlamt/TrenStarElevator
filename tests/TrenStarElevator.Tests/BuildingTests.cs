using TrenStarElevator.Domain.Entities;
using TrenStarElevator.Domain.Exceptions;
using TrenStarElevator.Tests.TestDoubles;
using Xunit;

namespace TrenStarElevator.Tests;

public class BuildingTests
{
    [Fact]
    public void Constructor_WithInvalidFloorRange_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Building("Test", minFloor: 5, maxFloor: 5));
    }

    [Fact]
    public void RegisterElevator_WithDuplicateId_ThrowsArgumentException()
    {
        var building = new Building("Test", minFloor: 0, maxFloor: 10);
        building.RegisterElevator(new TestElevator(id: "E-01", minFloor: 0, maxFloor: 10));

        Assert.Throws<ArgumentException>(() =>
            building.RegisterElevator(new TestElevator(id: "E-01", minFloor: 0, maxFloor: 10)));
    }

    [Fact]
    public void ValidateFloor_WithFloorOutsideRange_ThrowsInvalidFloorException()
    {
        var building = new Building("Test", minFloor: 0, maxFloor: 10);

        Assert.Throws<InvalidFloorException>(() => building.ValidateFloor(11));
    }

    [Fact]
    public void ValidateFloor_WithFloorWithinRange_DoesNotThrow()
    {
        var building = new Building("Test", minFloor: 0, maxFloor: 10);

        var exception = Record.Exception(() => building.ValidateFloor(5));

        Assert.Null(exception);
    }
}