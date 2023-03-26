using ElevatorSim.Models;

namespace ElevatorSim.Contracts
{
    public interface IElevatorController
    {
        ElevatorModel ElevatorModel { get; }

        Task Notify();
    }
}