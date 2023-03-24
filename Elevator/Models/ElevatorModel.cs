namespace ElevatorSim.Models;

public class ElevatorModel
{
    public ElevatorModel(int id, int capacity, int totalFloors)
    {
        this.ID = id;
        this.Capacity = capacity;
        this.totalFloors = totalFloors;
    }

    public int ID { get; set; }
    public Status CurrentStatus { get; set; } = Status.IDLE;
    public Direction CurrentDirection { get; set; } = Direction.NONE;
    public int CurrentFloor { get; set; } = 0;
    public List<People> Occupancy { get; set; } =new List<People>();
    public int Capacity { get; init; } = 5;

    public List<Floor> destinationFloors = new List<Floor>();

    public int totalFloors = 0;
}