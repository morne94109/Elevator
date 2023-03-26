using ElevatorSim.Models;

namespace ElevatorSim.Contracts;

public interface IElevatorManager
{
    void SetupElevators();

    Task<IElevatorController> ScheduleElevator(int floor);

    Task<string> GetElevatorStatus();
}