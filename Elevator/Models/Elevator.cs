using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElevatorSim.Contracts;

namespace ElevatorSim.Models
{

    public class Elevator
    {
        private readonly ElevatorModel _elevatorModel;
        private readonly IAppLogger _logger;

        public Elevator(int id, int capacity, IAppLogger logger)
        {
            _elevatorModel = new ElevatorModel(id, capacity);
            _logger = logger;
        }

        public ElevatorModel ElevatorModel
        {
            get { return _elevatorModel; }
        }

        // Method to move the elevator
        public async Task Move()
        {
            List<Floor> floorList = new List<Floor>(_elevatorModel.destinationFloors);
            foreach (var floorModel in floorList)
            {
                var floor = floorModel.ID;
                
                int elevatorID = _elevatorModel.ID;
                if (ElevatorModel.CurrentFloor == floor)
                {
                    _logger.LogMessage($"Elevator {elevatorID} is already on floor {floor}");
                    ElevatorModel.CurrentStatus = Status.IDLE;
                    ElevatorModel.CurrentDirection = Direction.NONE;                   
                    return;
                }

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
                            foreach(var people in _elevatorModel.Occupancy)
                            {
                                people.OnElevator = true;
                            }
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
                            foreach (var people in _elevatorModel.Occupancy)
                            {
                                people.OnElevator = true;
                            }
                        }
                    }
                }

                ElevatorModel.CurrentStatus = Status.IDLE;
                ElevatorModel.CurrentDirection = Direction.NONE;
                _logger.LogMessage($"Elevator {elevatorID} reached floor {floor}");
                ElevatorModel.destinationFloors.Remove(floorModel);
            }            
        }

        private void DepartFromElevator(Floor floorModel, int currentFloor)
        {
            //If people are on elevator and reach the destination floor they can leave
            if (_elevatorModel.Occupancy.Any(x => x.DestinationFloor == currentFloor && x.OnElevator == true))
            {
                var peoples = _elevatorModel.Occupancy.Where(x => x.DestinationFloor == currentFloor && x.OnElevator == true).ToList();
                RemoveOccupants(peoples);
            }
        }

        private static async Task SimulateMoveDelay()
        {
            await Task.Delay(2000);
        }

        // Method to add occupants to the elevator
        public bool AddOccupants(List<People> listOfPeople)
        {
            if (ElevatorModel.Occupancy.Count + listOfPeople.Count <= ElevatorModel.Capacity)
            {
                ElevatorModel.Occupancy.AddRange(listOfPeople);
                return true;
            }
            else
            {
                _logger.LogMessage("Elevator is at maximum capacity.");
                return false;
            }
        }

        // Method to remove occupants from the elevator
        public void RemoveOccupants(List<People> listOfPeople)
        {
            foreach (var people in listOfPeople)
            {
                ElevatorModel.Occupancy.Remove(people);
            }
            
        }

    }

    public class People
    {
       
        public int DestinationFloor { get; set; }

        public bool OnElevator { get; set; }

    }
}
