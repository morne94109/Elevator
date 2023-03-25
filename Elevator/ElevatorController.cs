using ElevatorSim.Contracts;
using ElevatorSim.Models;

namespace ElevatorSim;

public class ElevatorController : IElevatorController
{
    private Config _config;
    private IAppLogger _logger;
    private readonly IFloorController _floorController;
    private readonly IServiceProvider _serviceProvider;
    List<Elevator> Elevators = new List<Elevator>();
    public int FloorCount { get; set; }

    public void SetupElevators()
    {
        FloorCount = _floorController.GetFloorCount();
        _logger.LogMessage($"Building elevators.");
        for (int i = 0; i < _config.GetNumElevators(); i++)
        {
            Elevators.Add(new Elevator(i + 1, _config.GetCapacity(), FloorCount, _floorController));
            _logger.LogMessage($"Elevator {i + 1} has been installed.");
        }

        _logger.LogMessage("Installation of all elevators completed.");

    }

    public ElevatorController(Config config, IAppLogger logger, IFloorController floorController)
    {
        _config = config;
        _logger = logger;
        _floorController = floorController;
    }

    public void AddOccupant(Elevator current, int total)
    {
        //current.AddOccupants(total);
    }

    public void RemoveOccupant(Elevator current, int total)
    {
        // current.RemoveOccupants(total);
    }

    public async Task<Elevator> ScheduleElevator(int floor)
    {
        Elevator nearestElevator = null;
        Floor destinationFloor = _floorController.GetFloor(floor);
        int shortestDistance = FloorCount + 1;
        int totalWaiting = destinationFloor.GetPeopleOnFloor();

        int distanceLeeway = 3;
        while (nearestElevator == null)
        {
            // Find the nearest available elevator
            foreach (Elevator elevator in Elevators)
            {
                int distance = Math.Abs(floor - elevator.ElevatorModel.CurrentFloor);
                ElevatorModel elevatorModel = elevator.ElevatorModel;
                int totalAlreadyWaitding = 0;
                elevatorModel.destinationFloors.ForEach(f =>
                {
                    totalAlreadyWaitding += f.Num_People.Count;
                });

                int tempOccupancy = elevatorModel.Occupancy.Count + totalAlreadyWaitding;


                if (distance == 0
                    && elevatorModel.Occupancy.Count + totalWaiting <= elevatorModel.Capacity
                    && nearestElevator == null)
                {
                    nearestElevator = elevator;
                    shortestDistance = distance;
                }
                else if (elevatorModel is { CurrentStatus: Status.MOVING, CurrentDirection: Direction.UP }
                        && (tempOccupancy + totalWaiting) <= elevatorModel.Capacity
                        && distance <= distanceLeeway)
                {

                    if (elevatorModel.Occupancy.Count + totalAlreadyWaitding <= elevatorModel.Capacity)
                    {
                        nearestElevator = elevator;
                    }

                    break;
                }
                else if (elevatorModel is { CurrentStatus: Status.MOVING, CurrentDirection: Direction.DOWN }
                         && (tempOccupancy + totalWaiting) <= elevatorModel.Capacity
                         && distance <= distanceLeeway)
                {
                    nearestElevator = elevator;
                    break;
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
            if (nearestElevator.ElevatorModel.destinationFloors.FirstOrDefault(x => x.ID == destinationFloor.ID) == null)
            {
                nearestElevator.ElevatorModel.destinationFloors.Add(destinationFloor);
            }
           
            return nearestElevator;
        }
        else
        {
            Console.WriteLine("No available elevators.");
            return null;
        }
    }

    public async Task SendElevator()
    {
        foreach (var elevator in Elevators)
        {
            await elevator.Notify();
        }
    }

    public async Task<string> GetElevatorStatus()
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

        return message;
    }
}