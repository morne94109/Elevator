namespace ElevatorSim.Contracts;

public interface IBuildingManager
{
    void CreateBuilding();
    Task<bool> AddFloorToQueue(int floor);

    Task<string> GetBuildingStatus();

    Task ScheduleAndNotifyElevator(int floor);
    Task<int> GetNextFromQueue();
}