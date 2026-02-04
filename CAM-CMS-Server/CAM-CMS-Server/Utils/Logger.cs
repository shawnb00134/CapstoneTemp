using System.Runtime.CompilerServices;
using Serilog;

namespace CAMCMSServer.Utils;
#pragma warning disable CS8602
public class Logger
{
    #region Methods

    [ModuleInitializer]
    public static void Init()
    {
        Initialize();
    }

    public static void Initialize()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.File(
                Path.Combine("../", "LogFiles", $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}",
                    "Log.log"),
                rollingInterval: RollingInterval.Infinite,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}]")
            .CreateLogger();
    }

    #endregion
}