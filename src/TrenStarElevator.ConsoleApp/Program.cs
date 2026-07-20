using TrenStarElevator.Application.Interfaces;
using TrenStarElevator.Application.Services;
using TrenStarElevator.Application.Strategies;
using TrenStarElevator.ConsoleApp.UI;
using TrenStarElevator.Domain.Elevators;
using TrenStarElevator.Domain.Entities;
using TrenStarElevator.Infrastructure.Logging;

namespace TrenStarElevator.ConsoleApp;
/*.Presentation

Contains:Console UI,Input validation,Display logic,It simply calls Application services.  */
internal static class Program
{
    private static async Task Main()
    {
        var logFilePath = Path.Combine(AppContext.BaseDirectory, "logs", "elevator-simulation.log");
        var logger = new CompositeElevatorLogger(new ConsoleElevatorLogger(), new FileElevatorLogger(logFilePath));

        var building = BuildDefaultBuilding(logger);
        var controlSystem = new ElevatorControlSystem(building, new NearestElevatorDispatchStrategy(), logger);
        var engine = new ElevatorSimulationEngine(building, controlSystem, logger);
        var display = new ConsoleDisplayService(building);

        engine.Start();

        var running = true;
        while (running)
        {
            running = await RunMenuAsync(building, controlSystem, display);
        }

        await engine.StopAsync();
        Console.WriteLine("Simulation stopped. Goodbye.");
    }

    private static Building BuildDefaultBuilding(IElevatorLogger logger)
    {
        var building = new Building("TrenStar Tower", minFloor: 1, maxFloor: 15);

        ElevatorBase[] elevators =
        [
            new PassengerElevator("P-01", 1, 15, startingFloor: 1),
            new PassengerElevator("P-02", 1, 15, startingFloor: 8),
            new HighSpeedElevator("H-01", 1, 15, startingFloor: 1),
            new GlassElevator("G-01", 1, 15, startingFloor: 1),
            new FreightElevator("F-01", 1, 15, startingFloor: 1)
        ];

        foreach (var elevator in elevators)
        {
            elevator.Subscribe(new ElevatorEventLoggerAdapter(logger));
            building.RegisterElevator(elevator);
        }

        return building;
    }

    private static async Task<bool> RunMenuAsync(Building building, ElevatorControlSystem controlSystem, ConsoleDisplayService display)
    {
        Console.Clear();
        Console.WriteLine("======================================");
        Console.WriteLine(" TrenStar Elevator Control System");
        Console.WriteLine("======================================");
        Console.WriteLine("1. Call an elevator");
        Console.WriteLine("2. Watch live status board");
        Console.WriteLine("3. Show status snapshot");
        Console.WriteLine("4. Exit");
        Console.Write("Choose an option: ");

        switch (Console.ReadLine()?.Trim())
        {
            case "1":
                CallElevator(building, controlSystem);
                Pause();
                return true;
            case "2":
                await WatchLiveStatusAsync(display);
                return true;
            case "3":
                display.RenderSnapshot();
                Pause();
                return true;
            case "4":
                return false;
            default:
                Console.WriteLine("Unrecognized option.");
                Pause();
                return true;
        }
    }

    private static void CallElevator(Building building, ElevatorControlSystem controlSystem)
    {
        var origin = ReadFloor("Origin floor", building);
        var destination = ReadFloor("Destination floor", building);
        var passengers = ReadPassengerCount();

        try
        {
            controlSystem.RegisterRequest(new PassengerRequest(origin, destination, passengers));
            Console.WriteLine("Request registered.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not register request: {ex.Message}");
        }
    }

    private static int ReadFloor(string prompt, Building building)
    {
        while (true)
        {
            Console.Write($"{prompt} ({building.MinFloor}-{building.MaxFloor}): ");
            if (int.TryParse(Console.ReadLine(), out var floor) && floor >= building.MinFloor && floor <= building.MaxFloor)
            {
                return floor;
            }

            Console.WriteLine("Enter a valid floor number.");
        }
    }

    private static int ReadPassengerCount()
    {
        while (true)
        {
            Console.Write("Passenger count: ");
            if (int.TryParse(Console.ReadLine(), out var count) && count > 0)
            {
                return count;
            }

            Console.WriteLine("Enter a positive number.");
        }
    }

    private static async Task WatchLiveStatusAsync(ConsoleDisplayService display)
    {
        Console.CursorVisible = false;
        Console.Clear();
        Console.WriteLine("Press any key to return to the menu.");

        while (!Console.KeyAvailable)
        {
            display.RenderLive();
            await Task.Delay(250);
        }

        Console.ReadKey(true);
        Console.CursorVisible = true;
    }

    private static void Pause()
    {
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);
    }
}
