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
        for(int i = 0;i < _config.GetNumElevators(); i++)
        {
            Elevators.Add(new Elevator(i + 1,_config.GetCapacity(), _logger));
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
        current.AddOccupants(total);
    }

    public void RemoveOccupant(Elevator current, int total)
    {
        current.RemoveOccupants(total);
    }

    public async Task CallElevator(int floor)
    {
        Elevator nearestElevator = null;
        int shortestDistance = FloorCount + 1;

        // Find the nearest available elevator
        foreach (Elevator elevator in Elevators)
        {
            if (elevator.ElevatorModel.CurrentStatus == Status.IDLE && elevator.ElevatorModel.CurrentDirection == Direction.NONE)
            {
                int distance = Math.Abs(elevator.ElevatorModel.CurrentFloor - floor);
                if (distance < shortestDistance)
                {
                    nearestElevator = elevator;
                    shortestDistance = distance;
                }
            }
        }

        // Move the nearest available elevator to the requested floor
        if (nearestElevator != null)
        {
            await nearestElevator.MoveTo(floor);
        }
        else
        {
            Console.WriteLine("No available elevators.");
        }
    }
    
}