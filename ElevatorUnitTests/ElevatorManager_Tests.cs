using ElevatorSim.Contracts;
using ElevatorSim.Managers;
using ElevatorSim.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace ElevatorUnitTests
{
   
    public class ElevatorManager_Tests 
    {
   

        [InlineData(10, 3, 10)]
        [Theory]
        public void SetupElevators(int numFloors, int numElevators, int capacity)
        {
            IAppLogger logger = Mock.Of<IAppLogger>();
            var serviceProvider = new Mock<IServiceProvider>();
            Config config = new Config(numFloors, numElevators, capacity, 0, 0, 0);

            FloorManager floorController = new FloorManager(config, logger);
            ElevatorManager elevatorManager = new ElevatorManager(config, logger, floorController, serviceProvider.Object);

            BuildingManager buildingManager = new BuildingManager(elevatorManager, floorController, logger, config);
            ElevatorController elevatorController = new ElevatorController(config, floorController, buildingManager);

            serviceProvider
               .Setup(x => x.GetService(typeof(IElevatorController)))
               .Returns(elevatorController);


            elevatorManager.SetupElevators();

            Assert.True(elevatorManager.Elevators.Count == numElevators);
        }

        [InlineData(10, 3, 10, 5, 0, 2, 4, 4)]
        [InlineData(10, 3, 10, 7, 0, 9, 2, 9)]
        [InlineData(10, 3, 10, 7, 5, 2, 8, 8)]
        [InlineData(10, 3, 10, 1, 7, 5, 9, 5)]
        [Theory]
        public async Task ScheduleElevator(int numFloors,
            int numElevators,
            int capacity,
            int requestedFloor,
            int startingFloor1,
            int startingFloor2,
            int startingFloor3,
            int expectedFloor)
        {
            
            IAppLogger logger = Mock.Of<IAppLogger>();
            var serviceProvider = new Mock<IServiceProvider>();
            Config config = new Config(numFloors, numElevators, capacity, 0, 0, 0);

            FloorManager floorController = new FloorManager(config, logger);
            ElevatorManager elevatorManager = new ElevatorManager(config, logger, floorController, serviceProvider.Object);

            BuildingManager buildingManager = new BuildingManager(elevatorManager, floorController, logger, config);
            

            serviceProvider
               .Setup(x => x.GetService(typeof(IElevatorController)))
               .Returns(new ElevatorController(config, floorController, buildingManager));

            buildingManager.CreateBuilding();

            var list = elevatorManager.Elevators;
             for (int i = 0; i < numElevators; i++)
            {
                list[i] = new ElevatorController(config, floorController, buildingManager);
            }
                
                
            list[0].ElevatorModel.CurrentFloor = startingFloor1;
            list[1].ElevatorModel.CurrentFloor = startingFloor2;
            list[2].ElevatorModel.CurrentFloor = startingFloor3;

            var excpectedElevatorID = list.First(x => x.ElevatorModel.CurrentFloor == expectedFloor).ElevatorModel.ID;

            var result = await elevatorManager.ScheduleElevator(requestedFloor);

            var floor = result.ElevatorModel.destinationFloors.FirstOrDefault(x => x.ID == requestedFloor);
            Assert.NotNull(floor);
            Assert.True(floor.ID == requestedFloor);
            Assert.Equal(excpectedElevatorID, result.ElevatorModel.ID);
        }

        [InlineData(10, 3, 10, 5, 0, 2, 4,Status.MOVING,Direction.UP, 4)]
        [InlineData(10, 3, 10, 7, 0, 8, 2, Status.MOVING, Direction.UP, 8)]
        [InlineData(10, 3, 10, 7, 5, 2, 8, Status.MOVING, Direction.UP, 8)]
        [InlineData(10, 3, 10, 1, 7, 5, 9, Status.MOVING, Direction.UP, 9)]

        [InlineData(10, 3, 10, 5, 0, 2, 4, Status.MOVING, Direction.DOWN, 4)]
        [InlineData(10, 3, 10, 7, 0, 8, 2, Status.MOVING, Direction.DOWN, 8)]
        [InlineData(10, 3, 10, 7, 5, 2, 8, Status.MOVING, Direction.DOWN, 8)]
        [InlineData(10, 3, 10, 1, 7, 5, 9, Status.MOVING, Direction.DOWN, 9)]
        [InlineData(10, 3, 10, 9, 7, 5, 9, Status.MOVING, Direction.DOWN, 9)]
        [Theory]
        public async Task ScheduleElevatorMoving(int numFloors,
           int numElevators,
           int capacity,
           int requestedFloor,
           int startingFloor1,
           int startingFloor2,
           int startingFloor3,
           Status status,
           Direction direction,
           int expectedFloor)
        {

            IAppLogger logger = Mock.Of<IAppLogger>();
            var serviceProvider = new Mock<IServiceProvider>();
            Config config = new Config(numFloors, numElevators, capacity, 0, 0, 0)   ;

            FloorManager floorController = new FloorManager(config, logger);
            ElevatorManager elevatorManager = new ElevatorManager(config, logger, floorController, serviceProvider.Object);

            BuildingManager buildingManager = new BuildingManager(elevatorManager, floorController, logger, config);
            ElevatorController elevatorController = new ElevatorController(config, floorController, buildingManager);

            serviceProvider
               .Setup(x => x.GetService(typeof(IElevatorController)))
               .Returns(elevatorController);

            buildingManager.CreateBuilding();
            var list = elevatorManager.Elevators;
            for (int i = 0; i < numElevators; i++)
            {
                list[i] = new ElevatorController(config, floorController, buildingManager);
            }
            list[0].ElevatorModel.CurrentFloor = startingFloor1;
            list[0].ElevatorModel.CurrentDirection = direction;
            list[0].ElevatorModel.CurrentStatus = status;
            list[1].ElevatorModel.CurrentFloor = startingFloor2;
            list[1].ElevatorModel.CurrentDirection = direction;
            list[1].ElevatorModel.CurrentStatus = status;
            list[2].ElevatorModel.CurrentFloor = startingFloor3;
            list[2].ElevatorModel.CurrentDirection = Direction.NONE;
            list[2].ElevatorModel.CurrentStatus = Status.IDLE;

            var excpectedElevatorID = list.First(x => x.ElevatorModel.CurrentFloor == expectedFloor).ElevatorModel.ID;

            var result = await elevatorManager.ScheduleElevator(requestedFloor);

            var floor = result.ElevatorModel.destinationFloors.FirstOrDefault(x => x.ID == requestedFloor);
            Assert.NotNull(floor);
            Assert.True(floor.ID == requestedFloor);
            Assert.Equal(excpectedElevatorID, result.ElevatorModel.ID);
        }



        [InlineData(10, 3, 10)]
        [Theory]
        public async Task GetElevatorStatus(int numFloors, int numElevators, int capacity)
        {
            IAppLogger logger = Mock.Of<IAppLogger>();
            var serviceProvider = new Mock<IServiceProvider>();
            Config config = new Config(numFloors, numElevators, capacity, 0, 0, 0);

            FloorManager floorController = new FloorManager(config, logger);
            ElevatorManager elevatorManager = new ElevatorManager(config, logger, floorController, serviceProvider.Object);

            BuildingManager buildingManager = new BuildingManager(elevatorManager, floorController, logger, config);
            ElevatorController elevatorController = new ElevatorController(config, floorController, buildingManager);

            serviceProvider
               .Setup(x => x.GetService(typeof(IElevatorController)))
               .Returns(elevatorController);

            elevatorManager.SetupElevators();



            var result = await elevatorManager.GetElevatorStatus();
            foreach (var elevatorItem in elevatorManager.Elevators)
            {
                Assert.Contains($"Elevator: {elevatorItem.ElevatorModel.ID,5}", result);
                Assert.Contains($"Floor: {elevatorItem.ElevatorModel.CurrentFloor,5}", result);
                Assert.Contains($"Status: {elevatorItem.ElevatorModel.CurrentStatus,5}", result);
                Assert.Contains($"Direction: {elevatorItem.ElevatorModel.CurrentDirection,5}", result);
                Assert.Contains($"Occupancy: {elevatorItem.ElevatorModel.Occupancy.Count,5}", result);
                Assert.Contains($"Total Floors scheduled: {string.Join(",", elevatorItem.ElevatorModel.destinationFloors.Select(x => x.ID)),10}", result);
                Assert.Contains($"Elevator status ---------------------", result);
            }
        }
    }

}