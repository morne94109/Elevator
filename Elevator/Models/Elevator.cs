using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSim.Models
{
    public class Elevator
    {
        public Status CurrentStatus { get; set; } = Status.STOPPED;

        public Direction CurrentDirection { get; set; } = Direction.UP;

        public int CurrentFloor { get; set; } = 0;

        public int TotalPeople { get; set; } = 0;

        public int MaxPeople { get; init; } = 5;

    }
}
