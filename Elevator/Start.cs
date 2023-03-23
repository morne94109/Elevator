using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElevatorSim.Models;

namespace ElevatorSim
{
    public class Start
    {
        static void Main(string[] args)
        {
            // See https://aka.ms/new-console-template for more information

            Dictionary<int, int> PeopleOnFloors = new Dictionary<int, int>();
            List<Elevator> Elevators = new List<Elevator>();

            Console.WriteLine("Setting up the building!");

            for (int i = 0; i < 10; i++)
            {
                Console.Write(".");
                //Thread.Sleep(250);
            }

            Console.WriteLine("\n");
            Console.WriteLine("How may floors does the building have?");

            int maxRetries = 5;
            int retries = 0;



            int floors = 0;
            while (int.TryParse(Console.ReadLine(), out floors) == false || (retries < maxRetries && (floors < 1 || floors > 10)))
            {
                retries++;
                Console.WriteLine("Invalid number of floors specified, please specify a floor in the range of 1 and 10?");
                Console.WriteLine("How may floors does the building have?");

            }

            Console.WriteLine();

            retries = 0;

            Console.WriteLine("How may elevators does the building have?");
            int totalElevators = 0;
            while (int.TryParse(Console.ReadLine(), out totalElevators) == false || (retries < maxRetries && (totalElevators < 1 || totalElevators > 10)))
            {
                retries++;
                Console.WriteLine("Invalid number of elevators specified, please specify a total num of elevators in the range of 1 and 10?");
                Console.WriteLine("How may elevators does the building have?");

            }
            for(int i = 0;i < totalElevators; i++)
            {
                Elevators.Add(new Elevator());
            }

            Console.WriteLine("\nPlease wait while we are building");

            for (int i = 0; i < 10; i++)
            {
                Console.Write(".");
                // Thread.Sleep(250);
            }
            Console.WriteLine("We are done!\n");

            Console.WriteLine("Is there people waiting on the floors? (Y/N)");

            string response = Console.ReadLine();
            retries = 0;
            while ((response.Equals("y", StringComparison.InvariantCultureIgnoreCase) == false && response.Equals("n", StringComparison.InvariantCultureIgnoreCase) == false) && retries < maxRetries)
            {
                retries++;
                Console.WriteLine("Invalid response, please enter (y/n)");
                response = Console.ReadLine();
            }

            if (response.Equals("y", StringComparison.InvariantCultureIgnoreCase))
            {
                int selectedFloor = 0;

                while (selectedFloor != 99)
                {
                    Console.WriteLine("Which floor is the people waiting on? (Enter 99 if you want to skip)");
                    retries = 0;
                    selectedFloor = 0;
                    while (int.TryParse(Console.ReadLine(), out selectedFloor) == false
                        || (retries < maxRetries && (selectedFloor < 0 || selectedFloor > floors))
                        || PeopleOnFloors.ContainsKey(selectedFloor))
                    {
                        retries++;
                        if (PeopleOnFloors.ContainsKey(selectedFloor))
                        {
                            Console.WriteLine($"You specified a floor that is already listed with people.\nFloors already specified is ({string.Join(",", PeopleOnFloors.Keys)}) please choose another floor.");
                        }
                        else if (selectedFloor == 99)
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine($"You specified a floor that is higher then the building has or that is invalid.\nPlease specify a floor in the range of 0 and {floors}");
                        }
                        Console.WriteLine("Which floor is the people waiting on?");
                    }

                    if (selectedFloor == 99)
                    {
                        break;
                    }

                    Console.WriteLine("How many people is waiting?");
                    retries = 0;
                    int totalPeople = 0;
                    while (int.TryParse(Console.ReadLine(), out totalPeople) == false || (retries < maxRetries && (totalPeople < 1 || totalPeople > 10)))
                    {
                        retries++;
                        Console.WriteLine($"You specified an higher number or invalid number.\nPlease specify a amount of people in the range of 1 and 10");
                        Console.WriteLine("How many people is waiting?");
                    }

                    PeopleOnFloors.Add(selectedFloor, totalPeople);


                }


            }

            Console.WriteLine("Which floor are you on?");
            int userFloor = 0;
            while (int.TryParse(Console.ReadLine(), out userFloor) == false || (retries < maxRetries && (userFloor < 0 || userFloor > floors)))
            {
                retries++;
                Console.WriteLine($"Invalid number for floor specified, please specify a floor in the range of 0 and {floors}");
                Console.WriteLine("Which floor are you on?");
            }

        }
    }


}
