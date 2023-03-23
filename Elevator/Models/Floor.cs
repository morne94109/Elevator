namespace ElevatorSim.Models;

public class Floor
{
    public int Num_People { get; set; }

    public Floor(int numPeople)
    {
        Num_People = numPeople;
    }
}