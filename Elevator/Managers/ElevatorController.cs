using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Elevator.Implementation;
using ElevatorSim.Contracts;
using ElevatorSim.Models;
using Serilog;

[assembly: InternalsVisibleTo("ElevatorUnitTests")]
namespace ElevatorSim.Managers
{

    internal class ElevatorController : IElevatorController
    {
        private readonly ElevatorModel _elevatorModel;
        private readonly Config _config;
        private readonly IFloorManager _floorController;
        private readonly IBuildingManager _buildingManager;
        private readonly ILogger _logger;

        public ElevatorController(Config config,
            IFloorManager floorController,
            IBuildingManager buildingManager)
        {

            _elevatorModel = new ElevatorModel(config.GetCapacity(), config.GetNumFloors());
            this._config = config;
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
        
        /// <summary>
        /// Method to simulate elevator movement.
        /// </summary>
        /// <returns></returns>
        public async Task Notify()
        {
            List<Floor> floorList = new List<Floor>(ElevatorModel.destinationFloors);
            // Loop through the list of floors until empty
            while (floorList.Count != 0)
            {
                foreach (var floorModel in floorList)
                {
                    var floor = floorModel.ID;

                    string elevatorID = ElevatorModel.ID;
                    // if floor is same as elevator current floor pickup people
                    if (ElevatorModel.CurrentFloor == floor)
                    {
                        _logger.LogMessage($"Elevator {elevatorID} is already on floor {floor}");

                        List<People> waitingPeople = new List<People>(floorModel.Num_People);
                        foreach (var people in waitingPeople)
                        {
                            people.OnElevator = true;
                        }
                        AddOccupants(waitingPeople);
                        _logger.LogMessage($"New occupants");
                        await SimulateBoardingDelay();

                    }
                    else
                    {
                        _logger.LogMessage($"Elevator {elevatorID} is moving to floor {floor}");
                        ElevatorModel.CurrentStatus = Status.MOVING;

                        // if elevator current floor is lower than destination floor go UP
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
                                    //People are boarding
                                    foreach (var people in waitingPeople)
                                    {
                                        people.OnElevator = true;
                                    }

                                    AddOccupants(waitingPeople);
                                    _logger.LogMessage($"New occupants");
                                }
                            }

                        }
                        // if elevator current floor is higher than destination floor go DOWN
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
                                    _logger.LogMessage($"SimulateMoveDelay");
                                    await SimulateMoveDelay();
                                }
                                else
                                {
                                    List<People> waitingPeople = new List<People>(floorModel.Num_People);
                                    //People are boarding
                                    foreach (var people in waitingPeople)
                                    {
                                        people.OnElevator = true;
                                    }

                                    
                                    AddOccupants(waitingPeople);
                                    _logger.LogMessage($"New occupants");
                                }
                            }
                        }
                    }

                    ElevatorModel.CurrentStatus = Status.IDLE;
                    ElevatorModel.CurrentDirection = Direction.NONE;
                    _logger.LogMessage($"Elevator {elevatorID} reached floor {floor}");
                    ElevatorModel.destinationFloors.Remove(floorModel);
                }
                floorList = new List<Floor>(ElevatorModel.destinationFloors);
            }
        }

        /// <summary>
        /// Simulate boarding delay
        /// </summary>
        /// <returns></returns>
        private async Task SimulateBoardingDelay()
        {
            await Task.Delay(_config.GetBoardingDelay());
        }

        /// <summary>
        /// Depart from elevator
        /// </summary>
        /// <param name="floorModel"></param>
        /// <param name="currentFloor"></param>
        private void DepartFromElevator(Floor floorModel, int currentFloor)
        {
            //If people are on elevator and reach the destination floor they can leave
            if (ElevatorModel.Occupancy.Any(x => x.DestinationFloor == currentFloor && x.OnElevator == true))
            {
                var peoples = ElevatorModel.Occupancy.Where(x => x.DestinationFloor == currentFloor && x.OnElevator == true).ToList();
                RemoveOccupants(peoples);
                _logger.LogMessage($"{peoples.Count} departed from elevator.");
            }
        }

        /// <summary>
        /// Simulate move delay
        /// </summary>
        /// <returns></returns>

        private async Task SimulateMoveDelay()
        {
            await Task.Delay(_config.GetMoveDelay());
        }

        /// <summary>
        /// Add occupants to elevator
        /// </summary>
        /// <param name="listOfPeople"></param>
        /// <returns></returns>
        internal bool AddOccupants(List<People> listOfPeople)
        {
            if (ElevatorModel.Occupancy.Count + listOfPeople.Count <= ElevatorModel.Capacity)
            {
                _logger.LogMessage("1");
                ElevatorModel.Occupancy.AddRange(listOfPeople);
                _floorController.RemovePeople(ElevatorModel.CurrentFloor, listOfPeople);
                foreach (var people in listOfPeople)
                {
                    if (ElevatorModel.destinationFloors.Exists(x => x.ID == people.DestinationFloor) == false)
                    {
                        //Only add as drop off, not pickup as floor wasn't requested for pickup.
                        ElevatorModel.destinationFloors.Add(new Floor(people.DestinationFloor));
                    }
                }

                if (ElevatorModel.CurrentFloor >= (ElevatorModel.totalFloors / 2))
                {
                    ElevatorModel.destinationFloors =
                        ElevatorModel.destinationFloors.OrderByDescending(x => x.ID).ToList();
                }
                else
                {
                    ElevatorModel.destinationFloors =
                        ElevatorModel.destinationFloors.OrderBy(x => x.ID).ToList();
                }
                return true;
            }
            else
            {
                _logger.LogMessage("Elevator is at maximum capacity.");
                _logger.LogMessage("Scheduling another elevator for pickup.");
                _buildingManager.AddFloorToQueue(ElevatorModel.CurrentFloor);
                return false;
            }
        }

        /// <summary>
        /// Remove occupants from elevator
        /// </summary>
        /// <param name="listOfPeople"></param>
        private void RemoveOccupants(List<People> listOfPeople)
        {
            foreach (var people in listOfPeople)
            {
                ElevatorModel.Occupancy.Remove(people);
            }

        }
    }
}
