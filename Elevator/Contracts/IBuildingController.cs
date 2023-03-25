namespace ElevatorSim.Contracts;

public interface IBuildingController
{
    void CreateBuilding();
    Task CallElevator(int floor);

    Task<string> GetBuildingStatus();

    Task ScheduleAndNotifyElevator(int floor);
    Task<int> GetNextFromQueue();
}