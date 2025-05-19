using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VMManager.Models;
using VMManager.Services.Interfaces;

namespace VMManager.Services.Scheduling
{
    /// <summary>
    /// Background worker for polling and executing due backup jobs every 15 minutes.
    /// </summary>
    public class BackupJobWorker : BackgroundService
    {
        private readonly IBackupScheduler _scheduler;
        private readonly ILogger<BackupJobWorker> _logger;

        public BackupJobWorker(IBackupScheduler scheduler, ILogger<BackupJobWorker> logger)
            => (_scheduler, _logger) = (scheduler, logger);

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("BackupJobWorker started.");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var jobs = (await _scheduler.GetScheduledJobsAsync()).ToList();

                    foreach (var job in jobs.Where(j =>
                               j.IsEnabled && !j.IsRunning && j.NextRunTime <= DateTime.UtcNow))
                    {
                        if (string.IsNullOrWhiteSpace(job.Host))
                        {
                            _logger.LogWarning("Skipping job {0} due to missing host name.", job.Id);
                            continue;
                        }

                        _logger.LogInformation("Executing scheduled job {0} for host '{1}'", job.Id, job.Host);
                        await _scheduler.RunJobNowAsync(job);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while executing scheduled backup jobs.");
                }

                await Task.Delay(TimeSpan.FromMinutes(15), cancellationToken);
            }

            _logger.LogInformation("BackupJobWorker stopped.");
        }
    }
}
