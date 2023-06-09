﻿using System;
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
        private readonly IElevatorManager _elevatorManager;
        private readonly IFloorManager _floorManager;
        private readonly IAppLogger _logger;
        private readonly Config _config;
        private ConcurrentQueue<int>  _floorQueue = new ConcurrentQueue<int>();

        // Constructor
        public BuildingManager(IElevatorManager elevatorManager,
            IFloorManager floorController,
            IAppLogger logger,
            Config config)
        {
            _elevatorManager = elevatorManager;
            _floorManager = floorController;           
            _logger = logger;
            _config = config;
        }

        /// <summary>
        /// Create the building
        /// </summary>
        public void CreateBuilding()
        {
            _logger.LogMessage("Staring building construction!");
            _floorManager.SetupFloors();
            _elevatorManager.SetupElevators();
        }

        /// <summary>
        /// Add a floor to the queue for pickup
        /// </summary>
        /// <param name="floor"></param>
        /// <returns></returns>
        public Task<bool> AddFloorToQueue(int floor)
        {
            if (0 <= floor && floor <= _config.GetNumFloors())
            {
                if (_floorQueue.Any(x => x == floor))
                {
                    _logger.LogMessage($"Floor {floor} already scheduled for pickup. Will not add to queue.");
                }
                else
                {
                    _floorQueue.Enqueue(floor);
                    _logger.LogMessage($"Added floor {floor} to queue for pickup.");
                }
            }
            else
            {
                _logger.LogMessage($"Floor {floor} is not a valid floor. Please try again.");
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        /// <summary>
        /// Get the status of the building
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetBuildingStatus()
        {
            string elevatorResponse = await _elevatorManager.GetElevatorStatus();
            
            string floorResponse = await _floorManager.GetFloorsStatus();

            return elevatorResponse + Environment.NewLine + floorResponse;
        }

        /// <summary>
        /// Find an elevator and schedule it to pick up the next floor in the queue
        /// </summary>
        /// <param name="floor"></param>
        /// <returns></returns>
        public async Task ScheduleAndNotifyElevator(int floor)
        {
            
                IElevatorController result = await _elevatorManager.ScheduleElevator(floor);

                if (result != null && result.ElevatorModel.CurrentStatus != Status.MOVING)
                {                      
                    _ = result.Notify();
                }
                else if(result == null)
                {
                    _logger.LogMessage($"Unable to find elevator available for floor {floor}. Add back into queue.");
                    await AddFloorToQueue(floor);
                }
            
        }

        /// <summary>
        /// Return the next floor from the queue
        /// </summary>
        /// <returns></returns>
        public Task<int> GetNextFromQueue()
        {
            if (_floorQueue.IsEmpty == false && _floorQueue.TryDequeue(out int floor))
            {
                return Task.FromResult(floor);
            }

            return Task.FromResult(-1);
        }
    }
}