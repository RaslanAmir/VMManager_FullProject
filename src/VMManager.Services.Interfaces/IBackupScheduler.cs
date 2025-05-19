using System.Collections.Generic;
using System.Threading.Tasks;
using VMManager.Models;

namespace VMManager.Services.Interfaces
{
    /// <summary>
    /// Provides scheduling, persistence, and execution for VM backup and restore jobs.
    /// </summary>
    public interface IBackupScheduler
    {
        /// <summary>
        /// Loads all scheduled backup and restore jobs from persistent storage.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of scheduled jobs.</returns>
        Task<IEnumerable<ScheduledJob>> GetScheduledJobsAsync();

        /// <summary>
        /// Saves the current set of scheduled jobs to persistent storage.
        /// </summary>
        /// <param name="jobs">A collection of jobs to be saved.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        Task SaveJobsAsync(IEnumerable<ScheduledJob> jobs);

        /// <summary>
        /// Schedules a backup for all VMs on the specified host. (Legacy support)
        /// </summary>
        /// <param name="host">The host for which to schedule a backup.</param>
        /// <returns>A task that represents the asynchronous scheduling operation.</returns>
        Task ScheduleBackupAsync(HostInfo host);

        /// <summary>
        /// Immediately executes the specified backup or restore job.
        /// </summary>
        /// <param name="job">The job to run immediately.</param>
        /// <returns>A task that represents the asynchronous job execution.</returns>
        Task RunJobNowAsync(ScheduledJob job);
    }
}
