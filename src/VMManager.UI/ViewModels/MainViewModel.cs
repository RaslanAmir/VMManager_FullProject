using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using VMManager.Common.Services;
using VMManager.Common.Logging;
using VMManager.Models;
using VMManager.Services.Interfaces;
using Wpf.Ui.Appearance;

namespace VMManager.UI.ViewModels
{
    /// <summary>
    /// Main ViewModel for controlling navigation, VM actions, theming, and logging.
    /// </summary>
    public partial class MainViewModel : ObservableObject
    {
        private readonly IVmControlService _vmService;
        private readonly IHostManagementService _hostService;
        private readonly IHostConfigService _hostConfigService;
        private readonly IDialogService _dialogService;
        private readonly ILoggingService _logger;

        public ObservableCollection<VMDto> Vms { get; } = new();
        public ObservableCollection<string> ActivityLog { get; } = new();

        [ObservableProperty] private object currentViewModel = null!;
        [ObservableProperty] private string _searchText = string.Empty;
        [ObservableProperty] private bool isConnected = true;
        [ObservableProperty] private object? selectedPage;

        private readonly DashboardViewModel _dashboardViewModel;
        private readonly ScheduleViewModel _scheduleViewModel;
        private readonly SettingsViewModel _settingsViewModel;
        private readonly VmActionViewModel _vmActionViewModel;

        public IRelayCommand ShowDashboardCommand { get; }
        public IRelayCommand ShowScheduleCommand { get; }
        public IRelayCommand ShowSettingsCommand { get; }
        public IRelayCommand ShowVmActionsCommand { get; }

        public IRelayCommand<Type> ChangePageCommand { get; }
        public IRelayCommand ToggleThemeCommand { get; }

        public IAsyncRelayCommand RefreshStatusCommand { get; }
        public IAsyncRelayCommand<VMDto> StartCommand { get; }
        public IAsyncRelayCommand<VMDto> ShutdownCommand { get; }
        public IAsyncRelayCommand<VMDto> RestartCommand { get; }
        public IAsyncRelayCommand<VMDto> SnapshotCommand { get; }
        public IAsyncRelayCommand<VMDto> RestoreSnapshotCommand { get; }
        public IAsyncRelayCommand<VMDto> ConnectCommand { get; }

        public IRelayCommand AddHostCommand { get; }
        public IRelayCommand<HostDto> EditHostCommand { get; }
        public IRelayCommand<HostDto> RemoveHostCommand { get; }
        public IAsyncRelayCommand<HostDto> GracefulShutdownHostCommand { get; }
        public IRelayCommand<HostDto> OpenHostRdpCommand { get; }

        public MainViewModel(
            IVmControlService vmService,
            IHostManagementService hostService,
            IHostConfigService hostConfigService,
            IDialogService dialogService,
            ILoggingService logger,
            DashboardViewModel dashboardViewModel,
            ScheduleViewModel scheduleViewModel,
            SettingsViewModel settingsViewModel,
            VmActionViewModel vmActionViewModel)
        {
            _vmService = vmService;
            _hostService = hostService;
            _hostConfigService = hostConfigService;
            _dialogService = dialogService;
            _logger = logger;
            _dashboardViewModel = dashboardViewModel;
            _scheduleViewModel = scheduleViewModel;
            _settingsViewModel = settingsViewModel;
            _vmActionViewModel = vmActionViewModel;

            ShowDashboardCommand = new RelayCommand(() => CurrentViewModel = _dashboardViewModel);
            ShowScheduleCommand = new RelayCommand(() => CurrentViewModel = _scheduleViewModel);
            ShowSettingsCommand = new RelayCommand(() => CurrentViewModel = _settingsViewModel);
            ShowVmActionsCommand = new RelayCommand(() => CurrentViewModel = _vmActionViewModel);

            ChangePageCommand = new RelayCommand<Type>(OnChangePage);
            ToggleThemeCommand = new RelayCommand(OnToggleTheme);

            RefreshStatusCommand = new AsyncRelayCommand(OnRefreshAsync);
            StartCommand = new AsyncRelayCommand<VMDto>(OnStartAsync);
            ShutdownCommand = new AsyncRelayCommand<VMDto>(OnShutdownAsync);
            RestartCommand = new AsyncRelayCommand<VMDto>(OnRestartAsync);
            SnapshotCommand = new AsyncRelayCommand<VMDto>(OnSnapshotAsync);
            RestoreSnapshotCommand = new AsyncRelayCommand<VMDto>(OnRestoreSnapshotAsync);
            ConnectCommand = new AsyncRelayCommand<VMDto>(OnConnectAsync);

            AddHostCommand = new RelayCommand(OnAddHost);
            EditHostCommand = new RelayCommand<HostDto>(OnEditHost);
            RemoveHostCommand = new RelayCommand<HostDto>(OnRemoveHost);
            GracefulShutdownHostCommand = new AsyncRelayCommand<HostDto>(OnGracefulShutdownHostAsync);
            OpenHostRdpCommand = new RelayCommand<HostDto>(OnOpenHostRdp);

            _dashboardViewModel.StartCommand = StartCommand;
            _dashboardViewModel.ShutdownCommand = ShutdownCommand;
            _dashboardViewModel.ConnectCommand = ConnectCommand;

            SelectedPage = typeof(DashboardViewModel);
            CurrentViewModel = _dashboardViewModel;
        }

        private void OnChangePage(Type? pageType)
        {
            if (pageType == typeof(DashboardViewModel)) CurrentViewModel = _dashboardViewModel;
            else if (pageType == typeof(ScheduleViewModel)) CurrentViewModel = _scheduleViewModel;
            else if (pageType == typeof(SettingsViewModel)) CurrentViewModel = _settingsViewModel;
            else if (pageType == typeof(VmActionViewModel)) CurrentViewModel = _vmActionViewModel;
        }

        private void OnToggleTheme()
        {
            var theme = ApplicationThemeManager.GetAppTheme();
            var newTheme = theme == ApplicationTheme.Dark ? ApplicationTheme.Light : ApplicationTheme.Dark;
            ApplicationThemeManager.Apply(newTheme);
        }

        private async Task OnRefreshAsync()
        {
            try
            {
                Vms.Clear();
                var statuses = await _vmService.GetAllVmStatusAsync();
                foreach (var dto in statuses)
                    Vms.Add(dto);

                IsConnected = true;
                Log("✅ Refreshed VM statuses.");
                GroupAndInjectHosts();
            }
            catch (Exception ex)
            {
                IsConnected = false;
                await _dialogService.ShowErrorAsync($"❌ Error refreshing VMs: {ex.Message}");
                _logger.LogError($"VM refresh failed: {ex}");
            }
        }

        private void GroupAndInjectHosts()
        {
            var grouped = Vms
                .GroupBy(vm => vm.HostName)
                .Select(g => new HostDto
                {
                    HostName = g.Key,
                    VMs = new ObservableCollection<VMDto>(g)
                });

            _dashboardViewModel.Hosts = new ObservableCollection<HostDto>(grouped);
        }

        private async Task OnShutdownAsync(VMDto vm)
        {
            if (vm == null) return;
            Log($"🔻 Shutting down {vm.VMName}...");
            var result = await _vmService.ShutdownVmAsync(vm.HostName, vm.VMName);
            Log(result);
            _logger.LogInformation(result);
            await OnRefreshAsync();
        }

        private async Task OnStartAsync(VMDto vm)
        {
            if (vm == null) return;
            Log($"▶️ Starting {vm.VMName}...");
            var result = await _vmService.StartVmAsync(vm.HostName, vm.VMName);
            Log(result);
            _logger.LogInformation(result);
            await OnRefreshAsync();
        }

        private async Task OnRestartAsync(VMDto vm)
        {
            if (vm == null) return;
            Log($"🔁 Restarting {vm.VMName}...");
            var result = await _vmService.RestartVmAsync(vm.HostName, vm.VMName);
            Log(result);
            _logger.LogInformation(result);
            await OnRefreshAsync();
        }

        private async Task OnSnapshotAsync(VMDto vm)
        {
            if (vm == null) return;
            Log($"📸 Snapshot for {vm.VMName}...");
            var result = await _vmService.SnapshotVmAsync(vm.HostName, vm.VMName);
            Log(result);
            _logger.LogInformation(result);
            await OnRefreshAsync();
        }

        private async Task OnRestoreSnapshotAsync(VMDto vm)
        {
            if (vm == null) return;
            Log($"♻️ Restore snapshot for {vm.VMName}...");
            var result = await _vmService.RestoreSnapshotVmAsync(vm.HostName, vm.VMName);
            Log(result);
            _logger.LogInformation(result);
            await OnRefreshAsync();
        }

        private async Task OnConnectAsync(VMDto vm)
        {
            if (vm == null) return;
            Log($"🔗 Connecting to {vm.VMName}...");
            var result = await _vmService.ConnectVmAsync(vm.HostName, vm.VMName);
            Log(result);
            _logger.LogInformation(result);
        }

        private async Task OnGracefulShutdownHostAsync(HostDto host)
        {
            if (host == null) return;

            if (!await _dialogService.ShowConfirmationAsync($"Shutdown all VMs on {host.HostName}?")) return;

            Log($"⚠️ Graceful shutdown: {host.HostName}");
            _logger.LogInformation($"Graceful shutdown started for host: {host.HostName}");

            var hostVms = Vms.Where(v => v.HostName == host.HostName).ToList();
            var nonCriticalVms = hostVms.Where(v => !v.IsCritical).ToList();
            var criticalVms = hostVms.Where(v => v.IsCritical).ToList();

            foreach (var vm in nonCriticalVms)
            {
                Log($"🔻 Shutting down non-critical VM: {vm.VMName}");
                await _vmService.ShutdownVmAsync(host.HostName, vm.VMName);
            }

            foreach (var vm in criticalVms)
            {
                Log($"🔻 Shutting down critical VM: {vm.VMName}");
                await _vmService.ShutdownVmAsync(host.HostName, vm.VMName);
            }

            Log($"✅ All VMs shut down. Shutting down host {host.HostName}...");
            await _vmService.ShutdownHostAsync(host.HostName);
            _logger.LogInformation($"Host shutdown complete: {host.HostName}");

            await OnRefreshAsync();
        }

        private void OnOpenHostRdp(HostDto host)
        {
            if (host == null) return;

            Log($"💻 Opening RDP to {host.HostName}...");
            _ = OnConnectAsync(new VMDto { HostName = host.HostName, VMName = host.HostName });
        }

        private void OnAddHost() => Log("➕ Add Host triggered");
        private void OnEditHost(HostDto host) => Log($"✏️ Edit Host: {host.HostName}");
        private void OnRemoveHost(HostDto host) => Log($"❌ Remove Host: {host.HostName}");

        partial void OnSearchTextChanged(string value)
        {
            // Future: Apply live filtering to VMs.
        }

        private void Log(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            ActivityLog.Add(message);
            _logger.LogInformation(message);
            OnPropertyChanged(nameof(ActivityLog));
        }
    }
}
