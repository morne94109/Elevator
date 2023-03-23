using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSim.Models
{
    public class Elevator
    {
        public Elevator(int capacity)
        {
            Capacity = capacity;
        }

        public Status CurrentStatus { get; set; } = Status.IDLE;

        public Direction CurrentDirection { get; set; } = Direction.NONE;

        public int CurrentFloor { get; set; } = 0;

        public int Occupancy { get; set; } = 0;

        public int Capacity { get; init; } = 5;

        // Method to move the elevator
        public void MoveTo(int floor)
        {
            Console.WriteLine("Elevator is moving to floor " + floor);
            CurrentStatus = Status.MOVING;
            if (CurrentFloor < floor)
            {
                CurrentDirection = Direction.UP;
                for (int i = CurrentFloor; i <= floor; i++)
                {
                    CurrentFloor = i;
                    Console.WriteLine("Elevator is on floor " + CurrentFloor);
                }
            }
            else if (CurrentFloor > floor)
            {
                CurrentDirection = Direction.DOWN;
                for (int i = CurrentFloor; i >= floor; i--)
                {
                    CurrentFloor = i;
                    Console.WriteLine("Elevator is on floor " + CurrentFloor);
                }
            }
            CurrentStatus = Status.IDLE;
            CurrentDirection = Direction.NONE;
        }

        // Method to add occupants to the elevator
        public bool AddOccupants(int count)
        {
            if (Occupancy + count <= Capacity)
            {
                Occupancy += count;
                return true;
            }
            else
            {
                Console.WriteLine("Elevator is at maximum capacity.");
                return false;
            }
        }

        // Method to remove occupants from the elevator
        public void RemoveOccupants(int count)
        {
            Occupancy -= count;
        }

    }

    public class People
    {
        public int WaitingFloor { get; set; }

        public int DestinationFloor { get; set; }

        public bool OnElevator { get; set; }

    }
}
