using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using VMManager.Services.Interfaces;
using VMManager.Common.Logging;

namespace VMManager.UI.ViewModels
{
    /// <summary>
    /// ViewModel for performing virtual machine actions such as Start, Stop, Export, Restore, and RDP Connect.
    /// </summary>
    public partial class VmActionViewModel : ObservableObject
    {
        private readonly IVmControlService _vmService;
        private readonly IDialogService _dialogService;
        private readonly ILogger _logger;

        public VmActionViewModel(IVmControlService vmService, IDialogService dialogService, ILogger logger)
        {
            _vmService = vmService ?? throw new ArgumentNullException(nameof(vmService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            StartCommand = new AsyncRelayCommand(StartVmAsync);
            StopCommand = new AsyncRelayCommand(StopVmAsync);
            RestartCommand = new AsyncRelayCommand(RestartVmAsync);
            ExportCommand = new AsyncRelayCommand(ExportVmAsync);
            RestoreCommand = new AsyncRelayCommand(RestoreVmAsync);
            ConnectCommand = new AsyncRelayCommand(ConnectToVmAsync);
        }

        [ObservableProperty]
        private string host = string.Empty;

        [ObservableProperty]
        private string vmName = string.Empty;

        public IAsyncRelayCommand StartCommand { get; }
        public IAsyncRelayCommand StopCommand { get; }
        public IAsyncRelayCommand RestartCommand { get; }
        public IAsyncRelayCommand ExportCommand { get; }
        public IAsyncRelayCommand RestoreCommand { get; }
        public IAsyncRelayCommand ConnectCommand { get; }

        private async Task StartVmAsync()
        {
            try
            {
                _logger.Info($"▶️ Starting {vmName} on {host}...");
                var result = await _vmService.StartVmAsync(host, vmName);
                _logger.Info(result);
                await _dialogService.ShowMessageAsync("Start VM", result);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync($"Failed to start VM '{vmName}'", ex);
            }
        }

        private async Task StopVmAsync()
        {
            try
            {
                _logger.Info($"🔻 Stopping {vmName} on {host}...");
                var result = await _vmService.ShutdownVmAsync(host, vmName);
                _logger.Info(result);
                await _dialogService.ShowMessageAsync("Stop VM", result);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync($"Failed to stop VM '{vmName}'", ex);
            }
        }

        private async Task RestartVmAsync()
        {
            try
            {
                _logger.Info($"🔁 Restarting {vmName} on {host}...");
                var result = await _vmService.RestartVmAsync(host, vmName);
                _logger.Info(result);
                await _dialogService.ShowMessageAsync("Restart VM", result);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync($"Failed to restart VM '{vmName}'", ex);
            }
        }

        private async Task ExportVmAsync()
        {
            try
            {
                _logger.Info($"📤 Exporting {vmName} from {host}...");
                var result = await _vmService.SnapshotVmAsync(host, vmName);
                _logger.Info(result);
                await _dialogService.ShowMessageAsync("Export Snapshot", result);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync($"Failed to export VM '{vmName}'", ex);
            }
        }

        private async Task RestoreVmAsync()
        {
            try
            {
                _logger.Info($"♻️ Restoring {vmName} on {host}...");
                var result = await _vmService.RestoreSnapshotVmAsync(host, vmName);
                _logger.Info(result);
                await _dialogService.ShowMessageAsync("Restore Snapshot", result);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync($"Failed to restore VM '{vmName}'", ex);
            }
        }

        private async Task ConnectToVmAsync()
        {
            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(vmName))
            {
                var message = "Host or VM name is empty. Cannot connect.";
                _logger.Error(message);
                await _dialogService.ShowErrorAsync("Connection Error", message);
                return;
            }

            try
            {
                _logger.Info($"🔗 Connecting to {vmName} on {host}...");
                var result = await _vmService.ConnectVmAsync(host, vmName);
                _logger.Info(result);
                await _dialogService.ShowMessageAsync("RDP Connect", result);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync($"Failed to connect to VM '{vmName}'", ex);
            }
        }

        private async Task HandleErrorAsync(string title, Exception ex)
        {
            var message = $"{title}\n\n{ex.Message}";
            _logger.Error($"❌ {message}");
            await _dialogService.ShowErrorAsync(title, message);
        }
    }
}
