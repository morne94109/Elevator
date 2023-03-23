using ElevatorSim.Contracts;
using ElevatorSim.Models;

namespace ElevatorSim;

public class ElevatorController : IElevatorController
{
    private Config _config;
    private IAppLogger _logger;
    private readonly IFloorController _floorController;
    List<Elevator> Elevators = new List<Elevator>();
    public int FloorCount { get; set; }

    public void SetupElevators()
    {
        _logger.LogMessage($"Building elevators.");
        for (int i = 0; i < _config.GetNumElevators(); i++)
        {
            Elevators.Add(new Elevator(i + 1, _config.GetCapacity(), _logger));
            _logger.LogMessage($"Elevator {i + 1} has been installed.");
        }
        _logger.LogMessage("Installation of all elevators completed.");
        FloorCount = _floorController.GetFloorCount();
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

    public async Task<bool> ScheduleElevator(int floor, int totalWaiting)
    {
        Elevator nearestElevator = null;
        int shortestDistance = FloorCount + 1;

        // Find the nearest available elevator
        foreach (Elevator elevator in Elevators)
        {
            int distance = floor - elevator.ElevatorModel.CurrentFloor;

            if (distance < 0) //
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
            }
            else if (distance == 0)
            {
                nearestElevator = elevator;
                shortestDistance = distance;
            }
            else
            {
                //Go up
                if (elevator.ElevatorModel.CurrentStatus == Status.MOVING
                    && elevator.ElevatorModel.CurrentDirection == Direction.UP
                    && (elevator.ElevatorModel.Occupancy.Count + totalWaiting) <= elevator.ElevatorModel.Capacity
                    && distance <= 2)
                {
                    // elevator.ElevatorModel.destinationFloors.Add(_floorController.GetFloor(floor));
                    nearestElevator = elevator;
                    break;
                }
                else if (elevator.ElevatorModel.CurrentStatus == Status.IDLE
                     && elevator.ElevatorModel.CurrentDirection == Direction.NONE
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

        // Move the nearest available elevator to the requested floor
        if (nearestElevator != null)
        {
            nearestElevator.ElevatorModel.destinationFloors.Add(_floorController.GetFloor(floor));
            var peopleAtFloor = _floorController.GetPeopleWaiting(floor);
            nearestElevator.AddOccupants(peopleAtFloor);
            _floorController.RemovePeople(floor, peopleAtFloor);

            //await nearestElevator.MoveTo(floor);
            return true;
        }
        else
        {
            Console.WriteLine("No available elevators.");
            return false;
        }
    }

    public async Task SendElevator()
    {
        foreach (var elevator in Elevators)
        {
            await elevator.Move();
        }
    }0
}