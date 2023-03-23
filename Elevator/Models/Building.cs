using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSim.Models
{
    internal class Building
    {
        public int FloorCount { get; set; }
        public List<Elevator> Elevators { get; set; }

        // Constructor
        public Building(int floorCount, int elevatorCount, int elevatorCapacity)
        {
            FloorCount = floorCount;
            Elevators = new List<Elevator>();
            for (int i = 0; i < elevatorCount; i++)
            {
                Elevators.Add(new Elevator(elevatorCapacity));
            }
        }

        // Method to call an elevator to a specific floor
        public void CallElevator(int floor)
        {
            Elevator nearestElevator = null;
            int shortestDistance = FloorCount + 1;

            // Find the nearest available elevator
            foreach (Elevator elevator in Elevators)
            {
                if (elevator.CurrentStatus == Status.IDLE && elevator.CurrentDirection == Direction.NONE)
                {
                    int distance = Math.Abs(elevator.CurrentFloor - floor);
                    if (distance < shortestDistance)
                    {
                        nearestElevator = elevator;
                        shortestDistance = distance;
                    }
                }
            }

            // Move the nearest available elevator to the requested floor
            if (nearestElevator != null)
            {
                nearestElevator.MoveTo(floor);
            }
            else
            {
                Console.WriteLine("No available elevators.");
            }
        }
    }
}
