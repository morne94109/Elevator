using ElevatorSim.Models;

namespace ElevatorSim.Contracts;

public interface IElevatorController
{
    void SetupElevators();

    Task CallElevator(int floor);
    
    void AddOccupant(Elevator current, int total);

    void RemoveOccupant(Elevator current, int total);
}