using System.Text;
using TrenStarElevator.Domain.Entities;
using TrenStarElevator.Domain.Enums;

namespace TrenStarElevator.ConsoleApp.UI;

public sealed class ConsoleDisplayService
{
    private readonly Building _building;

    public ConsoleDisplayService(Building building)
    {
        _building = building;
    }

    /// <summary>Redraws the status board in place, for use in a refreshing display loop.</summary>
    public void RenderLive()
    {
        Console.SetCursorPosition(0, 0);
        Console.Write(BuildStatusReport());
    }

    /// <summary>Prints a single, non-refreshing snapshot of the current elevator status.</summary>
    public void RenderSnapshot() => Console.WriteLine(BuildStatusReport());

    private string BuildStatusReport()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"{_building.Name} \u2014 floors {_building.MinFloor} to {_building.MaxFloor}");
        sb.AppendLine(new string('=', 78));
        sb.AppendLine($"{"ID",-8}{"Floor",-8}{"Direction",-12}{"State",-14}{"Passengers",-13}Pending Stops");
        sb.AppendLine(new string('-', 78));

        foreach (var elevator in _building.Elevators)
        {
            var direction = elevator.CurrentDirection == Direction.Idle
                ? "IDLE"
                : elevator.CurrentDirection.ToString().ToUpperInvariant();
            var load = $"{elevator.PassengerCount}/{elevator.MaxCapacity}";
            var stops = elevator.PendingStops.Count == 0 ? "-" : string.Join(",", elevator.PendingStops);

            sb.AppendLine($"{elevator.Id,-8}{elevator.CurrentFloor,-8}{direction,-12}{elevator.State,-14}{load,-13}{stops}");
        }

        sb.AppendLine(new string('=', 78));
        return sb.ToString();
    }
}
