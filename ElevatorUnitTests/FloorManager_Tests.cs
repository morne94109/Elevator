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

public class FloorManager_Tests
{
    [InlineData(15,false)]
    [InlineData(8,true)]
    [Theory]
    public async Task SetupFloors(int floorCount, bool populate)
    {
            
           
        IAppLogger logger = Mock.Of<IAppLogger>();
        var serviceProvider = new Mock<IServiceProvider>();
        Config config = new Config(floorCount, 3, 10);
        FloorManager floorManager= new FloorManager(config, logger);
        
        floorManager.SetupFloors(true);
        
        Assert.True(floorManager.GetFloorCount() == config.GetNumFloors());
        
    }
    
    [InlineData(2)]
    [InlineData(4)]
    [Theory]
    public async Task GetFloor(int floor)
    {
            
           
        IAppLogger logger = Mock.Of<IAppLogger>();
        var serviceProvider = new Mock<IServiceProvider>();
        Config config = new Config(5, 3, 10);
        FloorManager floorManager= new FloorManager(config, logger);
        
        floorManager.SetupFloors(true);
        
        Assert.True(floorManager.GetFloorCount() == config.GetNumFloors());

        var floorModel = floorManager.GetFloor(floor);
        
        Assert.NotNull(floorModel);
        Assert.True(floorModel.ID == floor);

    }
    
    [InlineData(1)]
    [InlineData(4)]
    [Theory]
    public async Task GetPeopleWaiting(int totalWaiting)
    {
            
           
        IAppLogger logger = Mock.Of<IAppLogger>();
        var serviceProvider = new Mock<IServiceProvider>();

        var peopleList = new List<People>();
        for (int i = 0; i < totalWaiting; i++)
        {
            var peopleItem = new People();
            peopleItem.DestinationFloor = 1;
            peopleItem.OnElevator = false;
            peopleList.Add(peopleItem);
        }
       
       
        var newFloor = new Floor(0);
        newFloor.Num_People = peopleList;

        var newFloorList = new List<Floor>();
        newFloorList.Add(new Floor(newFloor));

       

        Config config = new Config(5, 3, 10);
        FloorManager floorManager= new FloorManager(config, logger);
        
        floorManager._floors = newFloorList;
        
        var floorModel = floorManager.GetPeopleWaiting(0);
        
        Assert.NotNull(floorModel);
        Assert.True(floorModel.Count == totalWaiting);

    }
    
    [InlineData(1,true)]
    [InlineData(4,false)]
    [Theory]
    public async Task HasPeopleWaiting(int totalWaiting, bool shouldHavePeopleWaiting)
    {
            
           
        IAppLogger logger = Mock.Of<IAppLogger>();
        var serviceProvider = new Mock<IServiceProvider>();

        
        var peopleList = new List<People>();
        var newFloor = new Floor(0);

        if (shouldHavePeopleWaiting)
        {
            for (int i = 0; i < totalWaiting; i++)
            {
                var peopleItem = new People();
                peopleItem.DestinationFloor = 1;
                peopleItem.OnElevator = false;
                peopleList.Add(peopleItem);
            }


            newFloor.Num_People = peopleList;
        }

        var newFloorList = new List<Floor>();
        newFloorList.Add(new Floor(newFloor));

       

        Config config = new Config(5, 3, 10);
        FloorManager floorManager= new FloorManager(config, logger);
        
        floorManager._floors = newFloorList;
        
        var floorModel = floorManager.HasPeopleWaiting(0);
        
        
        Assert.True(floorModel == shouldHavePeopleWaiting);

    }
}