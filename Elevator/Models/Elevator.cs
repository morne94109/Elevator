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
        public async Task MoveTo(int floor)
        {
            int elevatorID = _elevatorModel.ID;
            if (ElevatorModel.CurrentFloor == floor)
            {
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
                    ElevatorModel.CurrentFloor = i;
                    _logger.LogMessage($"Elevator {elevatorID} is on floor {ElevatorModel.CurrentFloor}" );
                    if (ElevatorModel.CurrentFloor != floor)
                    {
                        await SimulateMoveDelay();
                    }
                }
               
            }
            else if (ElevatorModel.CurrentFloor > floor)
            {
                ElevatorModel.CurrentDirection = Direction.DOWN;
                for (int i = ElevatorModel.CurrentFloor; i >= floor; i--)
                {
                    ElevatorModel.CurrentFloor = i;
                    _logger.LogMessage($"Elevator {elevatorID} is on floor {ElevatorModel.CurrentFloor}");
                    if (ElevatorModel.CurrentFloor != floor)
                    {
                        await SimulateMoveDelay();
                    }
                }
            }
            
            ElevatorModel.CurrentStatus = Status.IDLE;
            ElevatorModel.CurrentDirection = Direction.NONE;
            _logger.LogMessage($"Elevator {elevatorID} reached floor {floor}");
            
        }

        private static async Task SimulateMoveDelay()
        {
            await Task.Delay(2000);
        }

        // Method to add occupants to the elevator
        public bool AddOccupants(int count)
        {
            if (ElevatorModel.Occupancy + count <= ElevatorModel.Capacity)
            {
                ElevatorModel.Occupancy += count;
                return true;
            }
            else
            {
                _logger.LogMessage("Elevator is at maximum capacity.");
                return false;
            }
        }

        // Method to remove occupants from the elevator
        public void RemoveOccupants(int count)
        {
            ElevatorModel.Occupancy -= count;
        }

    }

    public class People
    {
        public int WaitingFloor { get; set; }

        public int DestinationFloor { get; set; }

        public bool OnElevator { get; set; }

    }
}
