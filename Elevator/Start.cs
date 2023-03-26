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
using Microsoft.Extensions.Hosting;

namespace ElevatorSim
{
    public class Start
    {
        private static Config elevatorConfig;


        static async Task Main(string[] args)
        {
            ParseConfigFile();

            var host = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {

                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IElevatorManager, ElevatorManager>();
                    services.AddSingleton<IFloorController, FloorManager>();
                    services.AddSingleton<IBuildingManager, BuildingManager>();
                    services.AddSingleton<Config>(elevatorConfig);
                    services.AddTransient<IAppLogger, AppLogger>();
                    services.AddHostedService<BuildingBackgroundService>();
                    services.AddTransient<IElevatorController, ElevatorController>();
                })
                .UseConsoleLifetime()
                .Build();

          

            host.RunAsync();

            var serviceProvider = host.Services;
   
            IBuildingManager buildingController = serviceProvider.GetRequiredService<IBuildingManager>();

            buildingController.CreateBuilding();

            Console.WriteLine("We are done!\n");

            bool exit = false;
            while (exit == false)
            {

                Console.WriteLine("---------------------------------------");
                Console.WriteLine("---------- 1: Call Elevator   ---------");
                Console.WriteLine("---------- 2: Get Status      ---------");
                Console.WriteLine("---------------------------------------");

                string userResponse = Console.ReadLine();

                if (int.TryParse(userResponse, out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            CallElevator(buildingController);
                            break;
                        case 2:
                            await GetStatus(buildingController);
                            break;
                        case 99:
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Invalid choice! Please choice from menu.");
                            break;

                    }
                }
            }



            Console.WriteLine("Hello");
        }

        private static async Task GetStatus(IBuildingManager buildingController)
        {
            string response = await buildingController.GetBuildingStatus();
            Console.WriteLine(response);
        }

        private static void CallElevator(IBuildingManager buildingController)
        {
            int maxRetries = 5;
            int retries = 0;
            int userFloor = 0;
            while (userFloor != 99)
            {
                Console.WriteLine("Which floor are you on? (Enter 99 if you are done!)");

                while (int.TryParse(Console.ReadLine(), out userFloor) == false || (retries < maxRetries &&
                           (userFloor < 0 ||
                            userFloor > elevatorConfig
                                .GetNumFloors() && userFloor != 99)))
                {
                    retries++;
                    Console.WriteLine(
                        $"Invalid number for floor specified, please specify a floor in the range of 0 and {elevatorConfig.GetNumFloors()}");
                    Console.WriteLine("Which floor are you on?");
                }

                if (userFloor != 99)
                {
                    Task.Run(async () =>
                    {
                        try
                        {
                            await buildingController.CallElevator(userFloor);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    });
                }
            }
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
