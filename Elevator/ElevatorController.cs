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

        // Find the nearest available elevator
        foreach (Elevator elevator in Elevators)
        {
            int distance = Math.Abs(floor - elevator.ElevatorModel.CurrentFloor);

            /*if (distance < 0) //
            {
                if (elevator.ElevatorModel.CurrentStatus == Status.IDLE
                      && elevator.ElevatorModel.CurrentDirection == Direction.NONE
                     && elevator.ElevatorModel.Occupancy.Count + totalWaiting <= elevator.ElevatorModel.Capacity)
                {
                    if (distance < shortestDistance)
                    {
                        nearestElevator = elevator;
                        shortestDistance = distance;
                    }
                }
            }*/
            if (distance == 0
                && elevator.ElevatorModel.Occupancy.Count + totalWaiting <= elevator.ElevatorModel.Capacity
                && nearestElevator == null)
            {
                nearestElevator = elevator;
                shortestDistance = distance;
            }
            else
            {
                //Go up
                if (elevator.ElevatorModel is { CurrentStatus: Status.MOVING, CurrentDirection: Direction.UP }
                    && (elevator.ElevatorModel.Occupancy.Count + totalWaiting) <= elevator.ElevatorModel.Capacity
                    && distance <= 3)
                {
                    // elevator.ElevatorModel.destinationFloors.Add(_floorController.GetFloor(floor));
                    nearestElevator = elevator;
                    break;
                }
                else if (elevator.ElevatorModel is { CurrentStatus: Status.MOVING, CurrentDirection: Direction.DOWN }
                         && (elevator.ElevatorModel.Occupancy.Count + totalWaiting) <= elevator.ElevatorModel.Capacity
                         && distance <= 3)
                {
                    // elevator.ElevatorModel.destinationFloors.Add(_floorController.GetFloor(floor));
                    nearestElevator = elevator;
                    break;
                }
                else if (elevator.ElevatorModel is { CurrentStatus: Status.IDLE, CurrentDirection: Direction.NONE }
                         && elevator.ElevatorModel.Occupancy.Count + totalWaiting <= elevator.ElevatorModel.Capacity)
                {
                    if (distance < shortestDistance)
                    {
                        nearestElevator = elevator;
                        shortestDistance = distance;
                    }
                }
            }

            //if (elevator.ElevatorModel.CurrentStatus == Status.IDLE 
            //    && elevator.ElevatorModel.CurrentDirection == Direction.NONE
            //    && elevator.ElevatorModel.Occupancy + totalWaiting <= elevator.ElevatorModel.Capacity)
            //{

            //    if (distance < shortestDistance)
            //    {
            //        nearestElevator = elevator;
            //        shortestDistance = distance;
            //    }
            //}           
        }

        // Notify the nearest available elevator to the requested floor
        if (nearestElevator != null)
        {
            nearestElevator.ElevatorModel.destinationFloors.Add(destinationFloor);
            //var peopleAtFloor = _floorController.GetPeopleWaiting(floor);
            //nearestElevator.AddOccupants(peopleAtFloor);
            //_floorController.RemovePeople(floor, peopleAtFloor);

            //await nearestElevator.MoveTo(floor);
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
                        + $" | Total Floors scheduled: {string.Join(",",elevator.ElevatorModel.destinationFloors.Select(x => x.ID)),10}";
        }

        return message;
    }
}