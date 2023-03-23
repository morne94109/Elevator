using System.Collections.Concurrent;
using ElevatorSim.Contracts;
using ElevatorSim.Models;
using System.Linq;

namespace ElevatorSim;

public class FloorController : IFloorController
{
    private Config _config;
    private IAppLogger _logger;
    private List<Floor> _floors = new List<Floor>();
    
    public FloorController(Config config, IAppLogger logger)
    {
        _config = config;
        _logger = logger;
    }

    public void SetupFloors()
    {
        _logger.LogMessage($"Building floors.");
        for(int i = 0;i < _config.GetNumFloors(); i++)
        {
            _floors.Add(new Floor(_config.GetCapacity()));
            _logger.LogMessage($"Floor {i} has been completed.");
        }
        _logger.LogMessage("Setup completed of floors for building.");
    }

    public int GetFloorCount()
    {
        return _floors.Count;
    }

    public int GetPeopleWaiting(int floor)
    {
        return _floors[floor].Num_People;
    }
    
    public bool HasPeopleWaiting(int floor)
    {
        return _floors[floor].Num_People > 0;
    }

    public void RemovePeople(int floor, int total)
    {
        _floors[floor].Num_People -= total;
    }
    

    
}