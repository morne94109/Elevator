using ElevatorSim.Contracts;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSim
{
    public class BuildingBackgroundService : IHostedService, IDisposable
    {
        private IBuildingManager _buildingController { get; }
        private Timer? _timer = null;
        private ILogger _logger;

        public BuildingBackgroundService(IBuildingManager buildingController1)
        {
            _buildingController = buildingController1;
            _logger = SetupLogger();
        }

      
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
            _logger.Debug("Timed Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(2));

            return Task.CompletedTask;
        }

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
            _logger.Debug("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        
    }
}
