using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace VMManager.Models
{
    /// <summary>
    /// Represents a scheduled job for a VM operation (export/restore) with optional recurrence.
    /// </summary>
    public partial class ScheduledJob : ObservableObject
    {
        /// <summary>
        /// Unique identifier of the job.
        /// </summary>
        [ObservableProperty]
        [JsonProperty("id")]
        private Guid _id = Guid.NewGuid();

        /// <summary>
        /// The Hyper-V host machine name.
        /// </summary>
        [ObservableProperty]
        [JsonProperty("host")]
        private string _host = string.Empty;

        /// <summary>
        /// The name of the virtual machine.
        /// </summary>
        [ObservableProperty]
        [JsonProperty("vm")]
        private string _vm = string.Empty;

        /// <summary>
        /// Type of job: Export, Restore, etc.
        /// </summary>
        [ObservableProperty]
        [JsonProperty("type")]
        private JobType _type = JobType.Unknown;

        /// <summary>
        /// Defines how often the job recurs.
        /// </summary>
        [ObservableProperty]
        [JsonProperty("recurrence")]
        private RecurrenceType _recurrence = RecurrenceType.OneTime;

        /// <summary>
        /// Path where the VM will be exported to.
        /// </summary>
        [ObservableProperty]
        [JsonProperty("destinationPath")]
        private string _destinationPath = string.Empty;

        /// <summary>
        /// Source path used for restore operations.
        /// </summary>
        [ObservableProperty]
        [JsonProperty("sourcePath")]
        private string _sourcePath = string.Empty;

        /// <summary>
        /// The next scheduled run time.
        /// </summary>
        [ObservableProperty]
        [JsonProperty("nextRunTime")]
        private DateTime _nextRunTime = DateTime.UtcNow;

        /// <summary>
        /// The last time this job was executed.
        /// </summary>
        [ObservableProperty]
        [JsonProperty("lastRunTime")]
        private DateTime _lastRunTime = DateTime.MinValue;

        /// <summary>
        /// Indicates whether the job is enabled.
        /// </summary>
        [ObservableProperty]
        [JsonProperty("isEnabled")]
        private bool _isEnabled = true;

        /// <summary>
        /// True if the job is currently executing.
        /// </summary>
        [ObservableProperty]
        [JsonProperty("isRunning")]
        private bool _isRunning = false;

        /// <summary>
        /// Optional job description or notes.
        /// </summary>
        [ObservableProperty]
        [JsonProperty("description")]
        private string _description = string.Empty;

        /// <summary>
        /// Compatibility alias for 'Vm' property used in legacy systems.
        /// </summary>
        [JsonIgnore]
        public string VM
        {
            get => Vm;
            set => Vm = value;
        }

        /// <summary>
        /// Creates a deep copy of the job. Optionally generates a new ID.
        /// </summary>
        /// <param name="generateNewId">If true, a new ID is assigned.</param>
        /// <returns>A cloned instance of the job.</returns>
        public ScheduledJob Clone(bool generateNewId = true)
        {
            return new ScheduledJob
            {
                Id = generateNewId ? Guid.NewGuid() : Id,
                Host = Host,
                Vm = Vm,
                Type = Type,
                Recurrence = Recurrence,
                DestinationPath = DestinationPath,
                SourcePath = SourcePath,
                NextRunTime = NextRunTime,
                LastRunTime = LastRunTime,
                IsEnabled = IsEnabled,
                IsRunning = IsRunning,
                Description = Description
            };
        }
    }
}
