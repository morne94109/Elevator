namespace ElevatorSim.Contracts;

public interface IBuildingController
{
    void CreateBuilding();
    Task CallElevator(int floor);
}