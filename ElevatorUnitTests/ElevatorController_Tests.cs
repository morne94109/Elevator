using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elevator.Models;
using ElevatorSim.Contracts;
using ElevatorSim.Managers;
using ElevatorSim.Models;
using Moq;
using Xunit;

namespace ElevatorUnitTests;

public class ElevatorController_Tests
{
    [InlineData(10,3,10,3,0)]
    [InlineData(10,3,10,5,5)]
    [InlineData(10,3,10,2,8)]
    [Theory]
    public async Task Notify(int numFloors,
        int numElevators,
        int capacity,
        int destinationFloorNum,
        int currentFloorNum)
    {
        IAppLogger logger = Mock.Of<IAppLogger>();
        var serviceProvider = new Mock<IServiceProvider>();
        Config config = new Config(numFloors, numElevators, capacity);
        
        FloorManager floorController = new FloorManager(config, logger);
        ElevatorManager elevatorManager = new ElevatorManager(config, logger, floorController, serviceProvider.Object);
        
        BuildingManager buildingManager = new BuildingManager(elevatorManager, floorController,logger,config);
        
        ElevatorController elevatorController = new ElevatorController(config, floorController, buildingManager);

        var random = new Random();
        
        for (int i = 0; i < numFloors; i++)
        {
            Floor newFloor = new Floor(i)
            {
                Num_People = new List<People>()
                {
                    new People()
                    {
                        DestinationFloor = random.Next(0, numFloors)
                    },
                    new People()
                    {
                        DestinationFloor = random.Next(0, numFloors)
                    }
                }
            };
            floorController._floors.Add(newFloor);
        }

        var destinationFloor = floorController.GetFloor(destinationFloorNum);
        elevatorController.ElevatorModel.destinationFloors.Add(destinationFloor);
        elevatorController.ElevatorModel.CurrentFloor = currentFloorNum;
        
        await elevatorController.Notify();
        
        Assert.True(elevatorController.ElevatorModel.destinationFloors.Count == 0);
        Assert.True(elevatorController.ElevatorModel.CurrentDirection == Direction.NONE);
        Assert.True(elevatorController.ElevatorModel.CurrentStatus == Status.IDLE);
        Assert.True(floorController.GetFloor(destinationFloorNum).Num_People.Count == 0);
        
        
        
    }

    public void SetElevatorFloor(int floor)
    {
        
    }
}