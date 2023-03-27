using Xunit;
using ElevatorSim.Managers;
using ElevatorSim.Models;
using ElevatorSim.Contracts;
using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ElevatorUnitTests
{
    public class BuildingManager_Tests
    {
        [InlineData(1,false)]
        [InlineData(2,true)]
        [Theory]
        public async Task CallElevator(int floor, bool AddTwice)
        {
            
           
            IAppLogger logger = Mock.Of<IAppLogger>();
            var serviceProvider = new Mock<IServiceProvider>();
            Config config = new Config(10, 3, 10);
            FloorManager floorController = new FloorManager(config, logger);
            ElevatorManager elevatorManager = new ElevatorManager(config, logger, floorController, serviceProvider.Object);
            
            BuildingManager buildingManager = new BuildingManager(elevatorManager, floorController,logger,config);


            serviceProvider
                .Setup(x => x.GetService(typeof(IElevatorController)))
                .Returns(new ElevatorController(config, floorController, buildingManager));

            if (AddTwice)
            {
                await buildingManager.CallElevator(floor);
            }
            var result = await buildingManager.CallElevator(floor);

            Assert.True(buildingManager.GetNextFromQueue().Result == floor);
            Assert.True(result == true);
        }
        
        
        [InlineData(15)]
        [Theory]
        public void Neg_CallElevator(int floor)
        {
            
           
            IAppLogger logger = Mock.Of<IAppLogger>();
            var serviceProvider = new Mock<IServiceProvider>();
            Config config = new Config(10, 3, 10);
            FloorManager floorController = new FloorManager(config, logger);
            ElevatorManager elevatorManager = new ElevatorManager(config, logger, floorController, serviceProvider.Object);
            
            BuildingManager buildingManager = new BuildingManager(elevatorManager, floorController,logger,config);


            serviceProvider
                .Setup(x => x.GetService(typeof(IElevatorController)))
                .Returns(new ElevatorController(config, floorController, buildingManager));

            var result = buildingManager.CallElevator(floor).Result;

            Assert.True(result == false);
        }
        
        [InlineData(5)]
        [Theory]
        public async Task Neg_GetNextFromQueue(int floor)
        {
            
           
            IAppLogger logger = Mock.Of<IAppLogger>();
            var serviceProvider = new Mock<IServiceProvider>();
            Config config = new Config(10, 3, 10);
            FloorManager floorController = new FloorManager(config, logger);
            ElevatorManager elevatorManager = new ElevatorManager(config, logger, floorController, serviceProvider.Object);
            
            BuildingManager buildingManager = new BuildingManager(elevatorManager, floorController,logger,config);


            serviceProvider
                .Setup(x => x.GetService(typeof(IElevatorController)))
                .Returns(new ElevatorController(config, floorController, buildingManager));

            var result = await buildingManager.GetNextFromQueue();

            Assert.True(result == -1);
        }
    }
}