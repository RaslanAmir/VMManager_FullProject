using Serilog;

namespace VMManager.Common.Logging
{
    public class SerilogLoggingService : ILoggingService
    {
        public SerilogLoggingService()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }

        public void LogInformation(string message) => Log.Information(message);
        public void LogError(string message, Exception ex) => Log.Error(ex, message);
    }
}