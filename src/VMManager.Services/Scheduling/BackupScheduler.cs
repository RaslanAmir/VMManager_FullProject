using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VMManager.Models;
using VMManager.Services.Interfaces;
using SysTimer = System.Timers.Timer;

namespace VMManager.Services.Scheduling
{
    /// <summary>
    /// Handles scheduling and execution of backup jobs.
    /// </summary>
    public class BackupScheduler : IBackupScheduler, IDisposable
    {
        private const int DefaultCheckIntervalMs = 60000;
        private const int AutoBackupIntervalHours = 6;
        private const int MaxBackupFiles = 10;

        private readonly IExportRestoreService _exportService;
        private readonly ILogger _logger;
        private readonly SysTimer _checkTimer;
        private readonly SysTimer _fullBackupTimer;
        private readonly object _lockObj = new();

        private List<ScheduledJob> _jobs = new();
        private FileSystemWatcher? _watcher;

        private string JobsFilePath => Path.Combine(AppContext.BaseDirectory, "jobs.json");
        private string BackupFolder => Path.Combine(AppContext.BaseDirectory, "Backups", "ScheduledJobs");
        private string FullBackupFolder => Path.Combine(AppContext.BaseDirectory, "Backups", "FullExports");

        public BackupScheduler(IExportRestoreService exportService, ILogger logger)
        {
            _exportService = exportService ?? throw new ArgumentNullException(nameof(exportService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _checkTimer = new SysTimer(DefaultCheckIntervalMs);
            _checkTimer.Elapsed += async (s, e) => await CheckScheduledJobsAsync();

            _fullBackupTimer = new SysTimer(TimeSpan.FromHours(AutoBackupIntervalHours).TotalMilliseconds);
            _fullBackupTimer.Elapsed += async (s, e) => await PerformFullExportBackupAsync();
        }

        /// <inheritdoc />
        public void Initialize(double checkIntervalMs = DefaultCheckIntervalMs)
        {
            LoadJobs();
            SetupWatcher();

            _checkTimer.Interval = checkIntervalMs;
            _checkTimer.AutoReset = true;
            _checkTimer.Start();

            _fullBackupTimer.AutoReset = true;
            _fullBackupTimer.Start();

            _logger.Info($"BackupScheduler initialized. Interval: {_checkTimer.Interval}ms");
        }

        /// <inheritdoc />
        public Task<IEnumerable<ScheduledJob>> GetScheduledJobsAsync()
        {
            lock (_lockObj)
                return Task.FromResult<IEnumerable<ScheduledJob>>(_jobs.ToList());
        }

        /// <inheritdoc />
        public Task SaveJobsAsync(IEnumerable<ScheduledJob> jobs)
        {
            if (jobs == null) throw new ArgumentNullException(nameof(jobs));

            lock (_lockObj)
            {
                _jobs = jobs.ToList();
                SaveJobs();
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task ScheduleBackupAsync(HostInfo host)
        {
            if (host == null || string.IsNullOrEmpty(host.HostName))
                throw new ArgumentException("Host information is missing.", nameof(host));

            var backupPath = Path.Combine(FullBackupFolder, $"{host.HostName}_{DateTime.Now:yyyyMMdd_HHmmss}");

            Directory.CreateDirectory(backupPath);
            await _exportService.ExportAllVMsOnHostAsync(host.HostName, backupPath);
            _logger.Info($"Scheduled backup completed for host: {host.HostName}");
        }

        /// <inheritdoc />
        public Task RunJobNowAsync(ScheduledJob job)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));
            return ExecuteJobAsync(job);
        }

        private async Task CheckScheduledJobsAsync()
        {
            List<ScheduledJob> dueJobs;
            lock (_lockObj)
                dueJobs = _jobs.Where(j => j.IsEnabled && !j.IsRunning && j.NextRunTime <= DateTime.Now).ToList();

            foreach (var job in dueJobs)
                await ExecuteJobAsync(job);
        }

        private async Task ExecuteJobAsync(ScheduledJob job)
        {
            lock (_lockObj) job.IsRunning = true;

            try
            {
                _logger.Info($"Executing job: {job.Id}");

                switch (job.Type)
                {
                    case JobType.Export:
                        await _exportService.ExportVMAsync(job.Host, job.VM, job.DestinationPath);
                        break;

                    case JobType.Restore:
                        await _exportService.RestoreVMAsync(job.Host, job.VM, job.SourcePath);
                        break;

                    default:
                        throw new NotSupportedException($"Unsupported job type: {job.Type}");
                }

                job.LastRunTime = DateTime.Now;
                job.NextRunTime = CalculateNextRunTime(job);
                _logger.Info($"Job complete: {job.Id}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Scheduled job {job.Id} failed: {ex.Message}");
            }
            finally
            {
                lock (_lockObj) job.IsRunning = false;
                SaveJobs();
            }
        }

        private DateTime CalculateNextRunTime(ScheduledJob job) => job.Recurrence switch
        {
            RecurrenceType.Daily => DateTime.Now.AddDays(1),
            RecurrenceType.Weekly => DateTime.Now.AddDays(7),
            RecurrenceType.Monthly => DateTime.Now.AddMonths(1),
            RecurrenceType.OneTime => DateTime.MaxValue,
            _ => DateTime.MaxValue
        };

        private void LoadJobs()
        {
            try
            {
                if (File.Exists(JobsFilePath))
                {
                    var json = File.ReadAllText(JobsFilePath);
                    var loaded = JsonConvert.DeserializeObject<List<ScheduledJob>>(json);
                    if (loaded != null)
                    {
                        lock (_lockObj) _jobs = loaded;
                        _logger.Info($"{_jobs.Count} jobs loaded.");
                        return;
                    }
                }

                _logger.Info("No jobs.json found; initializing empty job list.");
                lock (_lockObj) _jobs = new List<ScheduledJob>();
                SaveJobs();
            }
            catch (Exception ex)
            {
                _logger.Error($"LoadJobs failed: {ex.Message}");
                lock (_lockObj) _jobs = new List<ScheduledJob>();
            }
        }

        private void SaveJobs()
        {
            try
            {
                var json = JsonConvert.SerializeObject(_jobs, Formatting.Indented);
                File.WriteAllText(JobsFilePath, json);
                _logger.Info("Jobs saved.");
                RotateBackups();
            }
            catch (Exception ex)
            {
                _logger.Error($"SaveJobs failed: {ex.Message}");
            }
        }

        private void RotateBackups()
        {
            try
            {
                Directory.CreateDirectory(BackupFolder);
                var backups = Directory.GetFiles(BackupFolder, "jobs_*.json")
                    .OrderByDescending(f => f)
                    .Skip(MaxBackupFiles);

                foreach (var file in backups)
                    File.Delete(file);
            }
            catch (Exception ex)
            {
                _logger.Error($"RotateBackups failed: {ex.Message}");
            }
        }

        private void SetupWatcher()
        {
            try
            {
                _watcher = new FileSystemWatcher(Path.GetDirectoryName(JobsFilePath)!)
                {
                    Filter = Path.GetFileName(JobsFilePath),
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
                };
                _watcher.Changed += (s, e) => LoadJobs();
                _watcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                _logger.Error($"SetupWatcher failed: {ex.Message}");
            }
        }

        private async Task PerformFullExportBackupAsync()
        {
            try
            {
                Directory.CreateDirectory(FullBackupFolder);
                var path = Path.Combine(FullBackupFolder, $"AutoBackup_{DateTime.Now:yyyyMMdd_HHmmss}");
                await _exportService.ExportAllVMsOnHostAsync(Environment.MachineName, path);
                _logger.Info($"Auto full backup performed: {path}");
            }
            catch (Exception ex)
            {
                _logger.Error($"PerformFullExportBackup failed: {ex.Message}");
            }
        }

        /// <inheritdoc />
        public void Stop()
        {
            _checkTimer?.Stop();
            _fullBackupTimer?.Stop();
            _watcher?.Dispose();
            _logger.Info("BackupScheduler stopped.");
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Stop();
            _checkTimer?.Dispose();
            _fullBackupTimer?.Dispose();
            _watcher?.Dispose();
        }
    }
}
