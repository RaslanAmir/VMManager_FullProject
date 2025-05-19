using System.Threading.Tasks;

namespace VMManager.Application.Services
{
    /// <summary>
    /// Defines operations for initiating backup processes across all configured hosts.
    /// </summary>
    public interface IBackupService
    {
        /// <summary>
        /// Executes a one-time backup operation for all known virtual machines on all hosts.
        /// </summary>
        /// <returns>A task that completes when the backup operation finishes.</returns>
        Task RunBackupOnceAsync();
    }
}
