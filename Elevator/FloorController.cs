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

    public void SetupFloors(bool setupPeople = true)
    {
        _logger.LogMessage($"Building floors.");
        for (int i = 0; i < _config.GetNumFloors(); i++)
        {
            _floors.Add(new Floor(i));
            _logger.LogMessage($"Floor {i} has been completed.");
        }
        _logger.LogMessage("Setup completed of floors for building.");
        if (setupPeople)
        {
            PopulteFloors();
        }
    }

    public void PopulteFloors()
    {
        for (int i = 0; i < _floors.Count; i++)
        {
            var random = new Random();
            var randomNum = random.Next(0, 4);
            People people = new People();
            for (int z = 0; z < randomNum; z++)
            {
                people = new People();
                int nextfloor = 0;
                do
                {
                    nextfloor = random.Next(0, _floors.Count);
                }
                while (nextfloor == i);

                people.DestinationFloor = nextfloor;
                people.OnElevator = false;
                _floors[i].Num_People.Add(people);
            }

        }
    }

    public int GetFloorCount()
    {
        return _floors.Count;
    }

    public Floor GetFloor(int floor)
    {
        return new Floor(_floors[floor]);
    }

    public List<People> GetPeopleWaiting(int floor)
    {
        return new List<People>(_floors[floor].Num_People);
    }

    public bool HasPeopleWaiting(int floor)
    {
        return _floors[floor].Num_People.Count > 0;
    }

    public void RemovePeople(int floor, List<People> peopleList)
    {
        foreach (var people in peopleList)
        {
            _floors[floor].Num_People.Remove(people);
        }
    }

    public void RemovePeople(int floor)
    {

        _floors[floor].Num_People = new List<People>();
        
    }

    public async Task<string> GetFloorsStatus()
    {
        string message = "Floor status ---------------------";

        foreach (var floor in _floors)
        {
            message += Environment.NewLine + $"Floor {floor.ID,5} | Waiting: {floor.Num_People.Count,3}";
        }

        return message;
    }


}