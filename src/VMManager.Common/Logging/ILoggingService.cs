namespace VMManager.Common.Logging
{
    public interface ILoggingService
    {
        void LogInformation(string message);
        void LogError(string message, Exception ex);
    }
}