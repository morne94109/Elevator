using ElevatorSim.Models;

namespace ElevatorSim.Contracts;

public interface IElevatorController
{
    void SetupElevators();

    Task<Elevator> ScheduleElevator(int floor);

    Task SendElevator();


    void AddOccupant(Elevator current, int total);

    void RemoveOccupant(Elevator current, int total);
}