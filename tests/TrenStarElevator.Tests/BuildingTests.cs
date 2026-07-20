using TrenStarElevator.Domain.Entities;
using TrenStarElevator.Domain.Exceptions;
using TrenStarElevator.Tests.TestDoubles;
using Xunit;

namespace TrenStarElevator.Tests;
/* unit tests:

Tested:
1. Elevator movement
   - Move up from floor 0 to 5 → reaches floor 5
   - Cannot move to invalid floor (> max floor)

2. Dispatching
   - Nearest elevator is assigned
   - Full elevators are not assigned
   - Different directions handled correctly

3. Capacity
   - Cannot exceed max passengers
   - Request is queued if all elevators full

4. Edge Cases
   - No elevators available
   - Invalid floor numbers
   - Multiple simultaneous requests */

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
