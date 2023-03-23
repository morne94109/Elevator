using ElevatorSim.Models;

namespace ElevatorSim.Contracts;

public interface IElevatorController
{
    void SetupElevators();

    Task<bool> ScheduleElevator(int floor, int totalWaiting);

    Task SendElevator();


    void AddOccupant(Elevator current, int total);

    void RemoveOccupant(Elevator current, int total);
}