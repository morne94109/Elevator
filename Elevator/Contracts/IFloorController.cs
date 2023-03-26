using Elevator.Models;
using ElevatorSim.Models;

namespace ElevatorSim.Contracts;

public interface IFloorController
{
    void SetupFloors(bool setupPeople = true);

    int GetFloorCount();

    Floor GetFloor(int floor);

    List<People> GetPeopleWaiting(int floor);
    
    bool HasPeopleWaiting(int floor);

    void RemovePeople(int floor);
    void RemovePeople(int floor, List<People> peopleList);

    Task<string> GetFloorsStatus();

}