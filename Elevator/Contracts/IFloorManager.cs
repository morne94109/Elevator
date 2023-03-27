using ElevatorSim.Models;

namespace ElevatorSim.Contracts;

public interface IFloorManager
{
    void SetupFloors();

    void PopulateFloors(bool autoPopulate);

    int GetFloorCount();

    Floor GetFloor(int floor);

    List<People> GetPeopleWaiting(int floor);
    
    bool HasPeopleWaiting(int floor);
        
    void RemovePeople(int floor, List<People> peopleList);

    Task<string> GetFloorsStatus();

}