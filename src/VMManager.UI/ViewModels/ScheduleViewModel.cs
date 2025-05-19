using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using VMManager.Models;
using VMManager.Services.Interfaces;

namespace VMManager.UI.ViewModels
{
    /// <summary>
    /// ViewModel for managing scheduled backup and restore jobs.
    /// </summary>
    public partial class ScheduleViewModel : ObservableObject
    {
        private readonly IBackupScheduler _scheduler;
        private readonly ISettingsManager _settingsManager;
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;

        /// <summary>
        /// Collection of all jobs.
        /// </summary>
        public ObservableCollection<ScheduledJob> Jobs { get; } = new();

        /// <summary>
        /// List of available hosts.
        /// </summary>
        public ObservableCollection<string> Hosts { get; }

        /// <summary>
        /// List of VMs for selected host.
        /// </summary>
        public ObservableCollection<string> VMs { get; } = new();

        [ObservableProperty]
        private ScheduledJob? selectedJob;

        [ObservableProperty]
        private string selectedHost = string.Empty;

        [ObservableProperty]
        private string selectedVm = string.Empty;

        public IAsyncRelayCommand LoadJobsCommand { get; }
        public IAsyncRelayCommand AddJobCommand { get; }
        public IAsyncRelayCommand EditJobCommand { get; }
        public IAsyncRelayCommand DeleteJobCommand { get; }
        public IAsyncRelayCommand RunNowCommand { get; }
        public IRelayCommand ToggleEnableCommand { get; }

        public ScheduleViewModel(
            IBackupScheduler scheduler,
            ISettingsManager settingsManager,
            ILogger logger,
            IDialogService dialogService)
        {
            _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
            _settingsManager = settingsManager ?? throw new ArgumentNullException(nameof(settingsManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            Hosts = new ObservableCollection<string>(_settingsManager.GetHosts());

            LoadJobsCommand = new AsyncRelayCommand(LoadJobsAsync);
            AddJobCommand = new AsyncRelayCommand(AddJobAsync);
            EditJobCommand = new AsyncRelayCommand(EditJobAsync, () => SelectedJob != null);
            DeleteJobCommand = new AsyncRelayCommand(DeleteJobAsync, () => SelectedJob != null);
            RunNowCommand = new AsyncRelayCommand(RunJobNowAsync, () => SelectedJob != null);
            ToggleEnableCommand = new RelayCommand(ToggleEnable, () => SelectedJob != null);

            PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(SelectedJob))
                {
                    EditJobCommand.NotifyCanExecuteChanged();
                    DeleteJobCommand.NotifyCanExecuteChanged();
                    RunNowCommand.NotifyCanExecuteChanged();
                    ToggleEnableCommand.NotifyCanExecuteChanged();
                }
            };

            _ = LoadJobsAsync();
        }

        /// <summary>
        /// Reloads the job list from the scheduler.
        /// </summary>
        private async Task LoadJobsAsync()
        {
            try
            {
                Jobs.Clear();
                var jobs = await _scheduler.GetScheduledJobsAsync();
                foreach (var job in jobs)
                    Jobs.Add(job);
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to load jobs: {ex.Message}");
                await _dialogService.ShowErrorAsync("Load Error", $"Failed to load jobs.\n\n{ex.Message}");
            }
        }

        partial void OnSelectedHostChanged(string value)
        {
            VMs.Clear();
            if (string.IsNullOrWhiteSpace(value)) return;

            var vms = _settingsManager.GetVmsForHost(value);
            foreach (var vm in vms)
                VMs.Add(vm);
        }

        /// <summary>
        /// Adds a new job.
        /// </summary>
        private async Task AddJobAsync()
        {
            if (string.IsNullOrWhiteSpace(SelectedHost) || string.IsNullOrWhiteSpace(SelectedVm))
            {
                await _dialogService.ShowMessageAsync("Incomplete Selection", "Please select both a host and a VM.");
                return;
            }

            var newJob = new ScheduledJob
            {
                Host = SelectedHost,
                VM = SelectedVm,
                Type = JobType.Export,
                Recurrence = RecurrenceType.OneTime,
                NextRunTime = DateTime.UtcNow.AddMinutes(2),
                IsEnabled = true
            };

            try
            {
                var jobs = (await _scheduler.GetScheduledJobsAsync()).ToList();
                jobs.Add(newJob);
                await _scheduler.SaveJobsAsync(jobs);
                await LoadJobsAsync();
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to add job: {ex.Message}");
                await _dialogService.ShowErrorAsync("Add Error", $"Could not add job.\n\n{ex.Message}");
            }
        }

        /// <summary>
        /// Edits the selected job.
        /// </summary>
        private async Task EditJobAsync()
        {
            if (SelectedJob == null) return;

            try
            {
                var jobs = (await _scheduler.GetScheduledJobsAsync()).ToList();
                var job = jobs.FirstOrDefault(j => j.Id == SelectedJob.Id);
                if (job != null)
                {
                    job.Host = SelectedHost;
                    job.VM = SelectedVm;
                    job.NextRunTime = DateTime.UtcNow.AddMinutes(5); // Simulated edit
                    await _scheduler.SaveJobsAsync(jobs);
                    await LoadJobsAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Edit failed: {ex.Message}");
                await _dialogService.ShowErrorAsync("Edit Error", $"Failed to edit job.\n\n{ex.Message}");
            }
        }

        /// <summary>
        /// Deletes the selected job.
        /// </summary>
        private async Task DeleteJobAsync()
        {
            if (SelectedJob == null) return;

            try
            {
                var jobs = (await _scheduler.GetScheduledJobsAsync()).ToList();
                jobs.RemoveAll(j => j.Id == SelectedJob.Id);
                await _scheduler.SaveJobsAsync(jobs);
                await LoadJobsAsync();
            }
            catch (Exception ex)
            {
                _logger.Error($"Delete failed: {ex.Message}");
                await _dialogService.ShowErrorAsync("Delete Error", $"Failed to delete job.\n\n{ex.Message}");
            }
        }

        /// <summary>
        /// Executes the selected job immediately.
        /// </summary>
        private async Task RunJobNowAsync()
        {
            if (SelectedJob == null) return;

            try
            {
                await _scheduler.RunJobNowAsync(SelectedJob);
                await _dialogService.ShowMessageAsync("Success", "Job executed successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error($"RunNow failed: {ex.Message}");
                await _dialogService.ShowErrorAsync("Execution Error", $"Error running job.\n\n{ex.Message}");
            }

            await LoadJobsAsync();
        }

        /// <summary>
        /// Toggles whether the selected job is enabled.
        /// </summary>
        private void ToggleEnable()
        {
            if (SelectedJob == null) return;

            SelectedJob.IsEnabled = !SelectedJob.IsEnabled;
            _ = SaveJobsAsync();
        }

        /// <summary>
        /// Saves the current job list.
        /// </summary>
        private async Task SaveJobsAsync()
        {
            try
            {
                var jobs = Jobs.ToList();
                await _scheduler.SaveJobsAsync(jobs);
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to save jobs: {ex.Message}");
                await _dialogService.ShowErrorAsync("Save Error", $"Error saving jobs.\n\n{ex.Message}");
            }
        }
    }
}
