namespace ElevatorSim.Models;

public class Config
{
    private int number_floors { get; set; }
    private int number_elevators { get; set; }
    
    private int capacity { get; set; }

    private int move_delay { get; set; }

    private int boarding_delay { get; set; }

    private int polling_delay { get; set; }

    public Config(int numFloors, int numElevators, int capacity, int polling_delay, int moveDelay,  int boardingDelay)
    {
        this.number_floors = numFloors;
        this.number_elevators = numElevators;
        this.capacity = capacity;
        this.boarding_delay = boardingDelay;
        this.move_delay = moveDelay;
        this.polling_delay = polling_delay;
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

    public int GetBoardingDelay()
    {
        return this.boarding_delay;
    }

    public int GetMoveDelay()
    {
        return this.move_delay;
    }

    public int GetPollingDelay()
    {
        return this.polling_delay;
    }
}