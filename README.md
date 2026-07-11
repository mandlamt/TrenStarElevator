# TrenStar Elevator Simulation — TEAMX Elevator Challenge

A real-time, multi-elevator dispatch simulation console application for a large building,
built in C# / .NET 8 following Clean Architecture and SOLID principles.

## Overview

The application models a building's central computer coordinating several elevator cars of
different types (standard passenger, high-speed, glass/panoramic and freight). Users interact
through the console to call an elevator to a floor and specify how many passengers are waiting;
the central computer dispatches the nearest suitable car, and every elevator moves independently
and concurrently in real time, respecting passenger capacity limits.

## Features

- **Real-time elevator status** — current floor, direction, motion/idle state and passenger load
  for every car, refreshed live.
- **Interactive elevator control** — call a car to a floor, specify passenger count and
  destination from the console.
- **Multiple floors and elevators** — configurable building floor range and any number of cars.
- **Efficient dispatching** — a direction-aware "nearest car" algorithm (an elevator already
  sweeping towards a request is preferred over sending a farther idle car).
- **Passenger limit handling** — capacity is enforced per car; requests that can't currently be
  serviced are queued and automatically retried as capacity frees up.
- **Multiple elevator types** — `PassengerElevator`, `HighSpeedElevator`, `GlassElevator` and
  `FreightElevator`, each with different speed/capacity/door-timing characteristics, all sharing
  one base implementation and pluggable into the system via a common abstraction.
- **Real-time operation** — every elevator runs on its own asynchronous loop, so cars move and
  respond independently and concurrently, just like a real building's control system.

## Architecture

The solution follows a Clean Architecture layering, with dependencies pointing inwards:

```
TrenStarElevator.ConsoleApp        (Presentation / composition root)
        │
        ▼
TrenStarElevator.Infrastructure    (Logging sinks: console, file)
        │
        ▼
TrenStarElevator.Application       (Use cases: dispatch strategy, control system, simulation loop)
        │
        ▼
TrenStarElevator.Domain            (Entities, enums, exceptions, elevator behaviour - no dependencies)
```

- **Domain** — `IElevator` and its segregated sub-interfaces (`IElevatorStatus`,
  `IElevatorMovement`, `IElevatorPassengerHandling`, `IElevatorEventSource`), the `ElevatorBase`
  abstract class implementing shared movement/capacity logic, the four concrete elevator types,
  `Building`, `Floor`, `PassengerRequest`, and the domain exceptions. Has no dependency on any
  other project.
- **Application** — `IElevatorDispatchStrategy` abstraction and its `NearestElevatorDispatchStrategy`
  implementation, `ElevatorControlSystem` (the building's "central computer"), and
  `ElevatorSimulationEngine`, which drives the real-time per-elevator loops. Depends only on Domain.
- **Infrastructure** — `IElevatorLogger` implementations (`ConsoleElevatorLogger`,
  `FileElevatorLogger`, `CompositeElevatorLogger`) and an adapter bridging Domain elevator events
  onto the logger abstraction. Depends on Application/Domain, never the other way round.
- **ConsoleApp** — composition root (`Program.cs`) wiring everything together, plus the console
  menu and live status board renderer. Depends on all three layers below it.

### SOLID

- **SRP** — dispatching (`NearestElevatorDispatchStrategy`), coordination (`ElevatorControlSystem`),
  movement simulation (`ElevatorBase`) and presentation (`ConsoleDisplayService`) are all separate
  classes with one reason to change each.
- **OCP** — new elevator types are added by deriving from `ElevatorBase` (see `HighSpeedElevator`,
  `GlassElevator`, `FreightElevator`); new dispatch algorithms are added by implementing
  `IElevatorDispatchStrategy`; new logging sinks by implementing `IElevatorLogger` — none of this
  requires modifying existing classes.
- **LSP** — every elevator type can be used anywhere an `IElevator` is expected; the control
  system and dispatch strategy never check for a concrete type.
- **ISP** — `IElevator` is composed from four narrow interfaces so that, for example, the
  console status board depends only on `IElevatorStatus`, not the full movement/passenger API.
- **DIP** — `ElevatorControlSystem` and `ElevatorSimulationEngine` depend only on `IElevator`,
  `IElevatorDispatchStrategy` and `IElevatorLogger` abstractions, never on concrete elevator,
  strategy or logging classes.

## Dispatch algorithm

`NearestElevatorDispatchStrategy` scores each elevator with spare capacity by
`EstimatedFloorsToReach`, which accounts for the car's current direction and its pending stop
queue (a SCAN/"elevator algorithm" style sweep) rather than simple absolute distance, so a car
already heading the right way is preferred over sending a nearer-but-wrong-direction car past the
caller. Requests that cannot be serviced immediately (e.g. every car is full) are queued and
retried automatically once a second by the simulation engine.

## Getting started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Build

```bash
dotnet build
```

### Run

```bash
dotnet run --project src/TrenStarElevator.ConsoleApp
```

### Test

```bash
dotnet test
```

## Using the application

On launch, the menu offers:

1. **Call an elevator** — enter the floor passengers are waiting on, their destination floor,
   and how many passengers are waiting. The central computer dispatches the best available car.
2. **Watch live status board** — an auto-refreshing view of every elevator's floor, direction,
   state, passenger load and pending stops; press any key to return to the menu.
3. **Show status snapshot** — a single, non-refreshing view of current elevator status.
4. **Exit** — stops the simulation and closes the application.

Elevator activity (arrivals, boarding, dispatch decisions, warnings) is logged to the console and
appended to `logs/elevator-simulation.log` next to the executable.

## Project layout

```
TrenStarElevator/
├── src/
│   ├── TrenStarElevator.Domain/
│   ├── TrenStarElevator.Application/
│   ├── TrenStarElevator.Infrastructure/
│   └── TrenStarElevator.ConsoleApp/
├── tests/
│   └── TrenStarElevator.Tests/
├── TrenStarElevator.sln
└── README.md
```

## Known limitations / possible extensions

- The building configuration (floor range, number and type of elevators) is currently defined in
  `Program.BuildDefaultBuilding()`; externalising this to a config file would be a natural next step.
- Elevators are dispatched per-request rather than batched; a destination-dispatch panel (where
  passengers select their destination before entering, rather than a simple up/down hall call)
  could be layered on top of the existing `PassengerRequest` model without changing the Domain.
- `TakeOutOfService` / `ReturnToService` exist on every elevator to support maintenance scenarios
  and are exercised in the test suite, but are not yet wired into the console menu.
