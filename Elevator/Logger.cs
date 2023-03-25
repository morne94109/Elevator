using ElevatorSim.Contracts;
using Serilog;
using Serilog.Core;

namespace ElevatorSim.Logger;

public class AppLogger : IAppLogger
{
    
    public AppLogger()
    {
        SetupLogger();
    }

    public void SetupLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("logs/elevator.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }

    public void LogMessage(string message)
    {
        Log.Debug(message);
    }
}

public static class LoggerExtensions
{
    public static void LogMessage(this ILogger logger, string message)
    {
        logger.Debug(message);
    }
}