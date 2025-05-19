// 📍 File: src/VMManager.UI/ViewModels/DashboardViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using VMManager.Services.Interfaces;

namespace VMManager.UI.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly IDialogService _dialogService;

        public string Title => "Dashboard";

        public IAsyncRelayCommand ShowWelcomeMessageCommand { get; }

        public DashboardViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            ShowWelcomeMessageCommand = new AsyncRelayCommand(OnShowWelcomeMessageAsync);
        }

        private async Task OnShowWelcomeMessageAsync()
        {
            await _dialogService.ShowMessageAsync("Welcome to the dashboard!");
        }
    }
}
