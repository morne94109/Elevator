namespace ElevatorSim.Contracts;

public interface IBuildingManager
{
    void CreateBuilding();
    Task<bool> CallElevator(int floor);

    Task<string> GetBuildingStatus();

    Task ScheduleAndNotifyElevator(int floor);
    Task<int> GetNextFromQueue();
}