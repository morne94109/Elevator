namespace ElevatorSim.Contracts;

public interface IFloorController
{
    void SetupFloors();

    int GetFloorCount();

    int GetPeopleWaiting(int floor);
    
    bool HasPeopleWaiting(int floor);
    
    void RemovePeople(int floor, int total);

}