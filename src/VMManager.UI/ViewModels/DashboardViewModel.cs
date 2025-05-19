using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using VMManager.Models;
using VMManager.Services.Interfaces;

namespace VMManager.UI.ViewModels
{
    /// <summary>
    /// ViewModel responsible for managing and displaying hosts and their associated virtual machines on the dashboard.
    /// </summary>
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly IDialogService _dialogService;

        /// <summary>
        /// Collection of Hyper-V hosts and their virtual machines.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<HostDto> hosts = new();

        // 🔌 Commands injected from MainViewModel

        /// <summary>
        /// Command to start a virtual machine.
        /// </summary>
        [ObservableProperty]
        private IAsyncRelayCommand<VMDto>? startCommand;

        /// <summary>
        /// Command to gracefully shut down a virtual machine.
        /// </summary>
        [ObservableProperty]
        private IAsyncRelayCommand<VMDto>? shutdownCommand;

        /// <summary>
        /// Command to open RDP connection to a virtual machine.
        /// </summary>
        [ObservableProperty]
        private IAsyncRelayCommand<VMDto>? connectCommand;

        /// <summary>
        /// Command to show a welcome dialog (typically on first load).
        /// </summary>
        public IAsyncRelayCommand ShowWelcomeMessageCommand { get; }

        /// <summary>
        /// Creates a new instance of the DashboardViewModel.
        /// </summary>
        public DashboardViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            ShowWelcomeMessageCommand = new AsyncRelayCommand(OnShowWelcomeMessageAsync);
        }

        /// <summary>
        /// Shows a one-time welcome message to the user.
        /// </summary>
        private async Task OnShowWelcomeMessageAsync()
        {
            await _dialogService.ShowMessageAsync("🎉 Welcome to the VM Manager Dashboard!", "Welcome");
        }
    }
}
