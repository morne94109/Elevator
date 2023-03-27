namespace ElevatorSim.Models;

public class Floor
{
    public List<People> Num_People { get; set; } = new List<People>();
    public int ID { get; set; }

    public Floor(int id)
    {
        this.ID = id;
    }

    public Floor(Floor floor)
    {
        this.ID = floor.ID;
        this.Num_People = floor.Num_People;
    }
}

public static class FloorExtensions
{
    public static int GetPeopleOnFloor(this Floor floor)
    {
        return floor.Num_People.Count;
    }
}