using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elevator.Models;
using ElevatorSim.Contracts;
using ElevatorSim.Logger;
using Serilog;

namespace ElevatorSim.Models
{

    internal class ElevatorController : IElevatorController
    {
        private readonly ElevatorModel _elevatorModel;
        private readonly IFloorController _floorController;
        private readonly IBuildingManager _buildingManager;
        private readonly ILogger _logger;

        public ElevatorController(Config config,
            IFloorController floorController,
            IBuildingManager buildingManager)
        {
            _elevatorModel = new ElevatorModel(config.GetCapacity(), config.GetNumFloors());
            _floorController = floorController;
            _buildingManager = buildingManager;
            _logger = SetupLogger(_elevatorModel.ID);
            _logger.LogMessage($"Installation completed @ {DateTimeOffset.Now}!");
        }

        public ElevatorModel ElevatorModel => _elevatorModel;

        public ILogger SetupLogger(string id)
        {

            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File($"logs/elevator-{id}-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
        // Method to move the elevator
        public async Task Notify()
        {
            List<Floor> floorList = new List<Floor>(_elevatorModel.destinationFloors);
            while (floorList.Count != 0)
            {
                foreach (var floorModel in floorList)
                {
                    var floor = floorModel.ID;

                    string elevatorID = _elevatorModel.ID;
                    if (ElevatorModel.CurrentFloor == floor)
                    {
                        _logger.LogMessage($"Elevator {elevatorID} is already on floor {floor}");
                        // ElevatorModel.CurrentStatus = Status.IDLE;
                        // ElevatorModel.CurrentDirection = Direction.NONE;

                        List<People> waitingPeople = new List<People>(floorModel.Num_People);
                        foreach (var people in waitingPeople)
                        {
                            people.OnElevator = true;
                        }
                        AddOccupants(waitingPeople);
                        await SimulateBoardingDelay();

                    }
                    else
                    {
                        _logger.LogMessage($"Elevator {elevatorID} is moving to floor {floor}");
                        ElevatorModel.CurrentStatus = Status.MOVING;

                        if (ElevatorModel.CurrentFloor < floor)
                        {
                            ElevatorModel.CurrentDirection = Direction.UP;
                            for (int i = ElevatorModel.CurrentFloor; i <= floor; i++)
                            {

                                DepartFromElevator(floorModel, i);
                                ElevatorModel.CurrentFloor = i;
                                _logger.LogMessage($"Elevator {elevatorID} is on floor {ElevatorModel.CurrentFloor}");
                                if (ElevatorModel.CurrentFloor != floor)
                                {
                                    await SimulateMoveDelay();
                                }
                                else
                                {
                                    List<People> waitingPeople = new List<People>(floorModel.Num_People);
                                    foreach (var people in waitingPeople)
                                    {
                                        people.OnElevator = true;
                                    }

                                    AddOccupants(waitingPeople);
                                }
                            }

                        }
                        else if (ElevatorModel.CurrentFloor > floor)
                        {
                            ElevatorModel.CurrentDirection = Direction.DOWN;
                            for (int i = ElevatorModel.CurrentFloor; i >= floor; i--)
                            {
                                DepartFromElevator(floorModel, i);
                                ElevatorModel.CurrentFloor = i;
                                _logger.LogMessage($"Elevator {elevatorID} is on floor {ElevatorModel.CurrentFloor}");
                                if (ElevatorModel.CurrentFloor != floor)
                                {
                                    await SimulateMoveDelay();
                                }
                                else
                                {
                                    List<People> waitingPeople = new List<People>(floorModel.Num_People);
                                    foreach (var people in waitingPeople)
                                    {
                                        people.OnElevator = true;
                                    }

                                    AddOccupants(waitingPeople);
                                }
                            }
                        }
                    }

                    ElevatorModel.CurrentStatus = Status.IDLE;
                    ElevatorModel.CurrentDirection = Direction.NONE;
                    _logger.LogMessage($"Elevator {elevatorID} reached floor {floor}");
                    ElevatorModel.destinationFloors.Remove(floorModel);
                }
                floorList = new List<Floor>(_elevatorModel.destinationFloors);
            }
        }

        private async Task SimulateBoardingDelay()
        {
            await Task.Delay(50);
        }

        private void DepartFromElevator(Floor floorModel, int currentFloor)
        {
            //If people are on elevator and reach the destination floor they can leave
            if (_elevatorModel.Occupancy.Any(x => x.DestinationFloor == currentFloor && x.OnElevator == true))
            {
                var peoples = _elevatorModel.Occupancy.Where(x => x.DestinationFloor == currentFloor && x.OnElevator == true).ToList();
                RemoveOccupants(peoples);
                _logger.LogMessage($"{peoples.Count} departed from elevator.");
            }
        }

        private static async Task SimulateMoveDelay()
        {
            await Task.Delay(5000);
        }

        // Method to add occupants to the elevator
        private bool AddOccupants(List<People> listOfPeople)
        {
            if (ElevatorModel.Occupancy.Count + listOfPeople.Count <= ElevatorModel.Capacity)
            {
                ElevatorModel.Occupancy.AddRange(listOfPeople);
                _floorController.RemovePeople(_elevatorModel.CurrentFloor, listOfPeople);
                foreach (var people in listOfPeople)
                {
                    if (_elevatorModel.destinationFloors.Exists(x => x.ID == people.DestinationFloor) == false)
                    {
                        _elevatorModel.destinationFloors.Add(new Floor(people.DestinationFloor));
                    }
                }

                if (_elevatorModel.CurrentFloor >= (_elevatorModel.totalFloors / 2))
                {
                    _elevatorModel.destinationFloors =
                        _elevatorModel.destinationFloors.OrderByDescending(x => x.ID).ToList();
                }
                else
                {
                    _elevatorModel.destinationFloors =
                        _elevatorModel.destinationFloors.OrderBy(x => x.ID).ToList();
                }
                return true;
            }
            else
            {
                _logger.LogMessage("Elevator is at maximum capacity.");
                _logger.LogMessage("Scheduling another elevator for pickup.");
                _buildingManager.CallElevator(_elevatorModel.CurrentFloor);
                return false;
            }
        }

        // Method to remove occupants from the elevator
        private void RemoveOccupants(List<People> listOfPeople)
        {
            foreach (var people in listOfPeople)
            {
                ElevatorModel.Occupancy.Remove(people);
            }

        }
    }
}
