namespace VMManager.Services.Interfaces
{
    /// <summary>
    /// Simple logging interface for informational, warning, and error messages.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs an informational message.
        /// </summary>
        void Info(string message);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        void Warn(string message);

        /// <summary>
        /// Logs an error message.
        /// </summary>
        void Error(string message);
    }
}
