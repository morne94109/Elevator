using ElevatorSim.Contracts;
using ElevatorSim.Models;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSim.BackgroungServices
{
    [ExcludeFromCodeCoverage]
    public class BuildingBackgroundService : IHostedService, IDisposable
    {
        private IBuildingManager _buildingController { get; }
        private Timer? _timer = null;
        private ILogger _logger;
        private readonly Config _config;

        public BuildingBackgroundService(IBuildingManager buildingManager, Config config)
        {
            this._buildingController = buildingManager;
            this._config = config;
            _logger = SetupLogger();
        }

      
        //Setup logger for this class
        public ILogger SetupLogger()
        {

            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File($"logs/Building-BackgroundService-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }


        public void Dispose()
        {
           // throw new NotImplementedException();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Debug("Building Background Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(_config.GetPollingDelay()));

            return Task.CompletedTask;
        }

        //Execute the background service do send elevator to pickup people
        private async void DoWork(object? state)
        {
            _logger.Debug("Checking for floors to pickup people");

            int floor = await _buildingController.GetNextFromQueue();
            if(floor == -1)
            {
                _logger.Debug("Found to floors requiring pickup.");
            }
            else
            {
                _ = _buildingController.ScheduleAndNotifyElevator(floor);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Debug("Building Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
                
    }
}
