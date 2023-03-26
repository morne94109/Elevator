using Xunit;
using ElevatorSim.Managers;
using ElevatorSim.Models;
using ElevatorSim.Contracts;
using Moq;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace ElevatorUnitTests
{
    public class BuildingManager_Tests
    {
        [InlineData(1)]
        [InlineData(2)]
        [Theory]
        public void CallElevator(int floor)
        {
            
           
            IAppLogger logger = Mock.Of<IAppLogger>();
            var serviceProvider = new Mock<IServiceProvider>();
            Config config = new Config(10, 3, 10);
            FloorManager floorController = new FloorManager(config, logger);
            ElevatorManager elevatorManager = new ElevatorManager(config, logger, floorController, serviceProvider.Object);
           





            BuildingManager buildingManager = new BuildingManager(elevatorManager, floorController,logger);


            serviceProvider
                .Setup(x => x.GetService(typeof(IElevatorController)))
                .Returns(new ElevatorController(config, floorController, buildingManager));

            buildingManager.CallElevator(floor);

            Assert.True(buildingManager.GetNextFromQueue().Result == floor);
        }
    }
}