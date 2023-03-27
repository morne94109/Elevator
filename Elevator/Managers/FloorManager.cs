using System.Collections.Concurrent;
using ElevatorSim.Contracts;
using ElevatorSim.Models;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ElevatorUnitTests")]

namespace ElevatorSim.Managers;


internal class FloorManager : IFloorManager
{
    private Config _config;
    private IAppLogger _logger;
    public List<Floor> _floors { get; set; } = new List<Floor>();

    public FloorManager(Config config, IAppLogger logger)
    {
        _config = config;
        _logger = logger;
    }

    /// <summary>
    /// Setup the floors
    /// </summary>
    public void SetupFloors()
    {
        _logger.LogMessage($"Building floors.");
        for (int i = 0; i < _config.GetNumFloors(); i++)
        {
            _floors.Add(new Floor(i));
            _logger.LogMessage($"Floor {i} has been completed.");
        }
        _logger.LogMessage("Setup completed of floors for building.");
 }

    /// <summary>
    /// Populate the floors with people based on bool param
    /// </summary>
    /// <param name="autoPopulate"></param>
    public void PopulateFloors(bool autoPopulate)
    {
        // Loop through each floor
        for (int i = 0; i < _floors.Count; i++)
        {
            if (autoPopulate)
            {
                var random = new Random();
                var randomNum = random.Next(0, 6);
                People people = new People();
                // Add random number of people to each floor
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
            else
            {
                Console.WriteLine($"How many people should floor {i} have?");
                string userInput = Console.ReadLine();
                int numPeople;
                while (int.TryParse(userInput, out numPeople) == false )
                {
                    Console.WriteLine("Please enter a valid number.");
                    userInput = Console.ReadLine();
                }

              
                People people = new People();
                for(int c = 0; c < numPeople; c++)
                {
                    people = new People();
                    Console.WriteLine($"What floor should person {c} be going to?");
                    userInput = Console.ReadLine();
                    int nextfloor;
                   
                    while (int.TryParse(userInput, out nextfloor) == false || nextfloor >= _floors.Count)
                    {
                        Console.WriteLine($"Please enter a valid number. Between 0 and {_floors.Count - 1}");
                        userInput = Console.ReadLine();                       
                    }


                    people.DestinationFloor = nextfloor;
                    people.OnElevator = false;
                    _floors[i].Num_People.Add(people);
                }
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


    /// <summary>
    /// Return a string of the floors and the number of people waiting on each floor
    /// </summary>
    /// <returns></returns>
    public Task<string> GetFloorsStatus()
    {
        string message = "Floor status ---------------------";

        foreach (var floor in _floors)
        {
            message += Environment.NewLine + $"Floor {floor.ID,5} | Waiting: {floor.Num_People.Count,3}";
        }

        return Task.FromResult(message);
    }
}