using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VMManager.Models;

namespace VMManager.Core.Interfaces
{
    /// <summary>
    /// Defines methods for managing and executing scheduled backup and restore jobs.
    /// </summary>
    public interface IBackupScheduler
    {
        /// <summary>
        /// Retrieves the list of scheduled jobs from persistent storage.
        /// </summary>
        Task<IEnumerable<ScheduledJob>> GetScheduledJobsAsync();

        /// <summary>
        /// Schedules a one-time backup for a specific host.
        /// </summary>
        Task ScheduleBackupAsync(HostInfo host);

        /// <summary>
        /// Runs the provided job immediately.
        /// </summary>
        Task RunJobNowAsync(ScheduledJob job);

        /// <summary>
        /// Adds a new job to the schedule.
        /// </summary>
        void AddJob(ScheduledJob job);

        /// <summary>
        /// Removes a job from the schedule by its ID.
        /// </summary>
        void RemoveJob(Guid jobId);

        /// <summary>
        /// Initializes the scheduler with timers and file watchers.
        /// </summary>
        void Initialize(double checkIntervalMs = 60000);

        /// <summary>
        /// Stops the scheduler timers and disposes resources.
        /// </summary>
        void Stop();

        /// <summary>
        /// Returns all in-memory jobs.
        /// </summary>
        IReadOnlyList<ScheduledJob> Jobs { get; }
    }
}
