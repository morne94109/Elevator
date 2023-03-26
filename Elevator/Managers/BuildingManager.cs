using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ElevatorSim.Contracts;
using ElevatorSim.Models;
[assembly: InternalsVisibleTo("ElevatorUnitTests")]

namespace ElevatorSim.Managers
{
    internal class BuildingManager : IBuildingManager
    {
        private readonly IElevatorManager _elevatorController;
        private readonly IFloorManager _floorManager;
        private readonly IAppLogger _logger;
        private ConcurrentQueue<int>  _floorQueue = new ConcurrentQueue<int>();

        // Constructor
        public BuildingManager(IElevatorManager elevatorManager,
            IFloorManager floorController,
            IAppLogger logger)
        {
            _elevatorController = elevatorManager;
            _floorManager = floorController;           
            _logger = logger;
        }

        public void CreateBuilding()
        {
            _logger.LogMessage("Staring building construction!");
            _floorManager.SetupFloors();
            _elevatorController.SetupElevators();
        }

        // Method to call an elevator to a specific floor
        public Task CallElevator(int floor)
        {
            if(_floorQueue.Any(x => x == floor))
            {
                _logger.LogMessage($"Floor {floor} already scheduled for pickup. Will not add to queue.");                
            }
            else
            {
                _floorQueue.Enqueue(floor);
                _logger.LogMessage($"Added floor {floor} to queue for pickup.");
            }

            return Task.CompletedTask;
        }

        public async Task<string> GetBuildingStatus()
        {
            string elevatorResponse = await _elevatorController.GetElevatorStatus();
            
            string floorResponse = await _floorManager.GetFloorsStatus();

            return elevatorResponse + Environment.NewLine + floorResponse;
        }

        public async Task ScheduleAndNotifyElevator(int floor)
        {
            IElevatorController result = await _elevatorController.ScheduleElevator(floor);

            if (result != null && result.ElevatorModel.CurrentStatus != Status.MOVING)
            {                      
                _ = result.Notify();
            }
            else if(result == null)
            {
                _logger.LogMessage($"Unable to find elevator available for floor {floor}. Add back into queue.");
                await CallElevator(floor);
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