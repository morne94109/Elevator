using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElevatorSim.Contracts;
using ElevatorSim.Models;
using ElevatorSim.Logger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ElevatorSim
{
    public class Start
    {
        private static Config elevatorConfig;
        
        
        static async Task Main(string[] args)
        {
            ParseConfigFile();
            
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IElevatorController,ElevatorController>()
                .AddSingleton<IFloorController,FloorController>()
                .AddSingleton<IBuildingController,BuildingController>()
                .AddSingleton<Config>(elevatorConfig)
                .AddSingleton<IAppLogger,AppLogger>()
                .BuildServiceProvider();

            IBuildingController buildingController = serviceProvider.GetRequiredService<IBuildingController>();

            buildingController.CreateBuilding();
            
            Console.WriteLine("We are done!\n");
            
            int maxRetries = 5;
            int retries = 0;
            int userFloor = 0;
            while (userFloor != 99)
            {
                Console.WriteLine("Which floor are you on? (Enter 99 if you are done!)");
                
                while (int.TryParse(Console.ReadLine(), out userFloor) == false || (retries < maxRetries &&
                           (userFloor < 0 || userFloor > elevatorConfig.GetNumFloors())))
                {
                    retries++;
                    Console.WriteLine(
                        $"Invalid number for floor specified, please specify a floor in the range of 0 and {elevatorConfig.GetNumFloors()}");
                    Console.WriteLine("Which floor are you on?");
                }

                Task.Run(async() =>
                {
                    await buildingController.CallElevator(userFloor);
                });
            }

            Console.WriteLine("Hello");
        }
        private static void ParseConfigFile()
        {
            string fileName = "appsettings.json";
            
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(fileName);
            
            var configuration = builder.Build();
            
            string stringFloors = configuration["num_floors"];
            if (int.TryParse(stringFloors, out int numFloors) == false)
            {
                throw new Exception($"Unable to determine the requested number of floors from config file\nExpected integer got {stringFloors}");
            }

            string stringElevators = configuration["num_elevators"];
            if (int.TryParse(stringElevators, out int numElevators) == false)
            {
                throw new Exception($"Unable to determine the requested number of elevators from config file.\nExpected integer got {stringElevators}");
            }

            string stringCapacity = configuration["capacity"];
            if (int.TryParse(stringCapacity, out int capacity) == false)
            {
                throw new Exception($"Unable to determine the requested capacity from config file.\nExpected integer got {stringCapacity}");
            }
            elevatorConfig = new Config(numFloors, numElevators, capacity);

        }
    }


}
