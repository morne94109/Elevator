using ElevatorSim.Extensions;
using ElevatorSim.Contracts;
using ElevatorSim.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ElevatorUnitTests")]
namespace ElevatorSim.Managers;

internal class ElevatorManager : IElevatorManager
{
    private Config _config;
    private IAppLogger _logger;
    private readonly IFloorManager _floorController;
    private readonly IServiceProvider _serviceProvider;

    internal List<IElevatorController> Elevators = new List<IElevatorController>();
    public int FloorCount { get; set; }


    /// <summary>
    /// Setup the elevators
    /// </summary>
    public void SetupElevators()
    {
        FloorCount = _floorController.GetFloorCount();
        _logger.LogMessage($"Building elevators.");
        for (int i = 0; i < _config.GetNumElevators(); i++)
        {
            var elevator = _serviceProvider.GetRequiredService<IElevatorController>();
            Elevators.Add(elevator);

            _logger.LogMessage($"Elevator {elevator.ElevatorModel.ID} has been installed.");
        }

        _logger.LogMessage("Installation of all elevators completed.");

    }

    public ElevatorManager(Config config, IAppLogger logger, IFloorManager floorController, IServiceProvider serviceProvider)
    {
        _config = config;
        _logger = logger;
        _floorController = floorController;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Find and return the nearest elevator to the floor
    /// </summary>
    /// <param name="floor"></param>
    /// <returns></returns>
    public Task<IElevatorController> ScheduleElevator(int floor)
    {
        IElevatorController nearestElevator = null;
        Floor destinationFloor = _floorController.GetFloor(floor);
        int shortestDistance = FloorCount + 1;
        int totalWaiting = destinationFloor.GetPeopleOnFloor();

        int distanceLeeway = 3;
        //Loop until nearest elevator is found.
        // Increase leeway by 1 each time to allow for elevator to be found;
        while (nearestElevator == null && distanceLeeway <= _config.GetNumFloors())
        {
            
            //Shuffle list to avoid all floors scheduled on the same elevator
            Elevators.Shuffle();

            // Find the nearest available elevator
            foreach (IElevatorController elevator in Elevators)
            {
                int distance = Math.Abs(floor - elevator.ElevatorModel.CurrentFloor);
                ElevatorModel elevatorModel = elevator.ElevatorModel;
                int totalAlreadyWaiting = 0;
                elevatorModel.destinationFloors.ForEach(f =>
                {
                    totalAlreadyWaiting += f.Num_People.Count;
                });

                int tempOccupancy = elevatorModel.Occupancy.Count + totalAlreadyWaiting;


                // if elevator is idle and the elevator is not full, and the distance is 0
                if (distance == 0
                    && elevatorModel.Occupancy.Count + totalWaiting <= elevatorModel.Capacity
                    && nearestElevator == null)
                {
                    nearestElevator = elevator;
                    shortestDistance = distance;
                }
                // if elevator is moving and going up, and the elevator is not full, and the distance is less than the distance leeway
                else if (elevatorModel is { CurrentStatus: Status.MOVING, CurrentDirection: Direction.UP }
                        && (tempOccupancy + totalWaiting) <= elevatorModel.Capacity
                        && distance <= distanceLeeway)
                {
                    if (distance < shortestDistance)
                    {
                        if (elevatorModel.Occupancy.Count + totalAlreadyWaiting <= elevatorModel.Capacity)
                        {
                            nearestElevator = elevator;
                            shortestDistance = distance;
                        }
                    }
                }
                //if elevator is moving and going down, and the elevator is not full, and the distance is less than the distance leeway
                else if (elevatorModel is { CurrentStatus: Status.MOVING, CurrentDirection: Direction.DOWN }
                         && (tempOccupancy + totalWaiting) <= elevatorModel.Capacity
                         && distance <= distanceLeeway)
                {
                    // if the elevator is not full, and the distance is less than the distance leeway
                    if (elevatorModel.Occupancy.Count + totalAlreadyWaiting <= elevatorModel.Capacity)
                    {
                        if (distance < shortestDistance)
                        {
                            nearestElevator = elevator;
                            shortestDistance = distance;
                        }
                    }                    
                }
                else if (elevatorModel is { CurrentStatus: Status.IDLE, CurrentDirection: Direction.NONE }
                         && tempOccupancy + totalWaiting <= elevatorModel.Capacity)
                {
                    if (distance < shortestDistance)
                    {
                        nearestElevator = elevator;
                        shortestDistance = distance;
                        
                    }
                }
            }
            if (nearestElevator == null)
            {
                distanceLeeway++;
                _logger.LogMessage($"No elevator availible. Increasing distance to look for by 1. Distance lee way = {distanceLeeway}");
            }

        }

        // Notify the nearest available elevator to the requested floor
        if (nearestElevator != null)
        {
            Floor tempFloor =
                nearestElevator.ElevatorModel.destinationFloors.FirstOrDefault(x => x.ID == destinationFloor.ID);

            if (tempFloor == null)
            {
                nearestElevator.ElevatorModel.destinationFloors.Add(destinationFloor);
            }
            else if (tempFloor.Num_People.Count == 0)
            {
                tempFloor.Num_People = destinationFloor.Num_People;
            }

            return Task.FromResult(nearestElevator);
        }
        else
        {
            Console.WriteLine("No available elevators.");
            return Task.FromResult(nearestElevator);
        }
    }

    /// <summary>
    /// Return current elevator status
    /// </summary>
    /// <returns></returns>
    public Task<string> GetElevatorStatus()
    {
        string message = "Elevator status ---------------------";

        foreach (var elevator in Elevators)
        {

            message += Environment.NewLine
                        + $"Elevator: {elevator.ElevatorModel.ID,5}"
                        + $" | Floor: {elevator.ElevatorModel.CurrentFloor,5}"
                        + $" | Status: {elevator.ElevatorModel.CurrentStatus,5}"
                        + $" | Direction: {elevator.ElevatorModel.CurrentDirection,5}"
                        + $" | Occupancy: {elevator.ElevatorModel.Occupancy.Count,5}"
                        + $" | Total Floors scheduled: {string.Join(",", elevator.ElevatorModel.destinationFloors.Select(x => x.ID)),10}";
        }

        return Task.FromResult(message);
    }
}