 TrenStarElevator System

A robust, modular simulation of a building elevator management system using C#, structured with Domain-Driven Design (DDD) principles.

 Project Overview

The TrenStarElevator project models a sophisticated building environment with multiple elevator shafts. It manages passenger requests, handles movement, simulates load, and outputs status updates. It is designed to be extensible, allowing for different types of elevators and dispatch strategies.

 Architecture

The solution follows a clean architecture pattern:

src/TrenStarElevator.Domain`: The core of the system. Contains fundamental business rules, entities (`Building`, `PassengerRequest`), value objects, enums, interfaces, and the base logic for all elevators (`ElevatorBase`). It has no external dependencies.
`src/TrenStarElevator.Application`: Coordinates the domain logic to fulfill system use cases. Contains interfaces for application services, the primary control logic (`ElevatorControlSystem`), the simulation orchestration (`ElevatorSimulationEngine`), and dispatch algorithms.
`src/TrenStarElevator.Infrastructure`: Implementation details that support the system but are not part of the core domain. Currently handles logging mechanisms (Console, File).
`src/TrenStarElevator.ConsoleApp`: The user interface layer. A simple command-line interface to visualize the state of the building and its elevators.
`tests/TrenStarElevator.Tests`: Automated test suite for validating the logic across different layers.

Key Components

`ElevatorControlSystem`: The main brain that accepts requests, uses a strategy to assign them, and responds to elevator events.
`IElevatorDispatchStrategy`: An interface allowing different algorithms for picking the best elevator for a request.
`ElevatorBase`: A robust base class implementing core physics, capacity handling, and event raising.

Getting Started

Prerequisites

.NET 8.0 SDK or later

Building and Running

1.  Open a terminal in the root directory (`TrenStarElevator/`).
2.  Build the solution:
    bash
    dotnet build
    
3.  Run the Console application:
    bash
    dotnet run --project src/TrenStarElevator.ConsoleApp
    

Running Tests

1.  Open a terminal in the root directory.
2.  Run all tests:
    bash
    dotnet test
    
