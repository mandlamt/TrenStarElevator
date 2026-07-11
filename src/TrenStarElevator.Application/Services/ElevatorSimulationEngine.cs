using System;
using System.Collections.Generic;
using System.Threading;
using TrenStarElevator.Application.Strategies;
using TrenStarElevator.Application.Interfaces;
using TrenStarElevator.Domain.Entities;
using TrenStarElevator.Domain.Enums;

namespace TrenStarElevator.Application.Services;

public class ElevatorSimulationEngine
{
    private readonly Building _building;
    private readonly ElevatorControlSystem _controlSystem;
    private readonly IElevatorLogger _logger;
    private readonly Random _random;
    private CancellationTokenSource? _cts;
    private Task? _runTask;

    public ElevatorSimulationEngine(Building building, ElevatorControlSystem controlSystem, IElevatorLogger logger)
    {
        _building = building;
        _controlSystem = controlSystem;
        _logger = logger;
        _random = new Random();
    }

    public void Start()
    {
        if (_runTask is { IsCompleted: false })
        {
            return;
        }

        _cts = new CancellationTokenSource();
        _runTask = Task.Run(() => RunSimulationAsync(_cts.Token));
        _logger.LogInfo("Elevator simulation engine started.");
    }

    public async Task StopAsync()
    {
        if (_cts is null || _runTask is null)
        {
            return;
        }

        _logger.LogInfo("Stopping elevator simulation engine...");
        await _cts.CancelAsync();

        try
        {
            await _runTask;
        }
        catch (OperationCanceledException)
        {
            // Expected: Task.Delay throws when the token is cancelled during shutdown.
        }
        finally
        {
            _cts.Dispose();
            _cts = null;
            _runTask = null;
        }
    }

    private async Task RunSimulationAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            // Roughly 30% of ticks generate a new passenger request.
            if (_random.Next(0, 10) < 3)
            {
                GenerateRandomTraffic();
            }

            foreach (var elevator in _building.Elevators)
            {
                elevator.StepSimulation();
            }

            await Task.Delay(800, token);
        }
    }

    private void GenerateRandomTraffic()
    {
        int fromFloor = _random.Next(_building.MinFloor, _building.MaxFloor + 1);
        int toFloor;

        do
        {
            toFloor = _random.Next(_building.MinFloor, _building.MaxFloor + 1);
        } while (toFloor == fromFloor);

        int passengerCount = _random.Next(1, 5);
        var request = new PassengerRequest(fromFloor, toFloor, passengerCount);

        try
        {
            _controlSystem.RegisterRequest(request);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to register random simulation request: {ex.Message}", ex);
        }
    }
}