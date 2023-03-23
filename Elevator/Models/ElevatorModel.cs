namespace ElevatorSim.Models;

public class ElevatorModel
{
    public ElevatorModel(int id, int capacity)
    {
        this.ID = id;
        this.Capacity = capacity;
    }

    public int ID { get; set; }
    public Status CurrentStatus { get; set; } = Status.IDLE;
    public Direction CurrentDirection { get; set; } = Direction.NONE;
    public int CurrentFloor { get; set; } = 0;
    public int Occupancy { get; set; } = 0;
    public int Capacity { get; init; } = 5;
}