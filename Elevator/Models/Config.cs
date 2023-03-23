namespace ElevatorSim.Models;

public class Config
{
    private int number_floors { get; set; }
    private int number_elevators { get; set; }
    
    private int capacity { get; set; }

    public Config(int numFloors, int numElevators, int capacity)
    {
        this.number_floors = numFloors;
        this.number_elevators = numElevators;
        this.capacity = capacity;
    }

    public int GetNumFloors()
    {
        return this.number_floors;
    }
    
    public int GetNumElevators()
    {
        return this.number_elevators;
    }
    
    public int GetCapacity()
    {
        return this.capacity;
    }
}