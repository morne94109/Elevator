﻿using System;
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
        
        public List<Elevator> Elevators { get; set; }

        // Constructor
        public BuildingController(IElevatorController elevatorController, IFloorController floorController, Config config, IAppLogger logger)
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
            await _elevatorController.CallElevator(floor);
        }
    }
}
