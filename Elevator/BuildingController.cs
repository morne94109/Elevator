using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElevatorSim.Contracts;

namespace ElevatorSim.Models
{
    internal class BuildingController : IBuildingController
    {
        private readonly IElevatorController _elevatorController;
        private readonly IFloorController _floorController;
        private readonly Config _config;
        private readonly IAppLogger _logger;
        private ConcurrentQueue<int>  _floorQueue = new ConcurrentQueue<int>();

        public List<Elevator> Elevators { get; set; }

        // Constructor
        public BuildingController(IElevatorController elevatorController, IFloorController floorController,
            Config config, IAppLogger logger)
        {
            _elevatorController = elevatorController;
            _floorController = floorController;
            _config = config;
            _logger = logger;
        }

        public void CreateBuilding()
        {
            _logger.LogMessage("Staring building construction!");
            _floorController.SetupFloors();
            _elevatorController.SetupElevators();
        }

        // Method to call an elevator to a specific floor
        public async Task CallElevator(int floor)
        {
            if(_floorQueue.Any(x => x == floor))
            {
                _logger.LogMessage("Floor already scheduled for pickup. Will not add to queue.");
                
            }
            else
            {
                _floorQueue.Enqueue(floor);
            }
        }

        public async Task<string> GetBuildingStatus()
        {
            string elevatorResponse = await _elevatorController.GetElevatorStatus();
            
            string floorResponse = await _floorController.GetFloorsStatus();

            return elevatorResponse + Environment.NewLine + floorResponse;
        }

        public async Task ScheduleAndNotifyElevator(int floor)
        {
            Elevator result = await _elevatorController.ScheduleElevator(floor);

            if (result != null && result.ElevatorModel.CurrentStatus != Status.MOVING)
            {                      
                _ = result.Notify();
            }
        }

        public Task<int> GetNextFromQueue()
        {
            if (_floorQueue.IsEmpty == false)
            {
                if (_floorQueue.TryDequeue(out int floor))
                {
                    return Task.FromResult(floor);
                }
            }

            return Task.FromResult(-1);
        }
    }
}