using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using VMManager.Services.Interfaces;

namespace VMManager.UI.ViewModels
{
    /// <summary>
    /// ViewModel for managing general application settings such as language, FTP configuration, and RDP credentials.
    /// </summary>
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly IDialogService _dialogService;
        private readonly ISettingsManager _settingsManager;

        /// <summary>
        /// Title used for display in UI tabs or window headers.
        /// </summary>
        public string Title => "Settings";

        /// <summary>
        /// List of supported application languages.
        /// </summary>
        public ObservableCollection<string> Languages { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="SettingsViewModel"/> with required services.
        /// </summary>
        /// <param name="dialogService">Dialog interaction service.</param>
        /// <param name="settingsManager">Settings persistence manager.</param>
        public SettingsViewModel(IDialogService dialogService, ISettingsManager settingsManager)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _settingsManager = settingsManager ?? throw new ArgumentNullException(nameof(settingsManager));

            Languages = new ObservableCollection<string>(_settingsManager.GetSupportedLanguages());

            // Initialize ViewModel properties with saved values
            SelectedLanguage = _settingsManager.Language;
            EnableFtpSync = _settingsManager.EnableFtpSync;
            FtpHost = _settingsManager.FtpHost;
            FtpUser = _settingsManager.FtpUser;
            FtpPassword = _settingsManager.FtpPassword;
            RdpUsername = _settingsManager.RdpUsername;
            RdpPassword = _settingsManager.RdpPassword;
            AutoRefreshIntervalSeconds = _settingsManager.AutoRefreshIntervalSeconds;
        }

        // 🌐 General Language Configuration

        /// <summary>
        /// Selected language used in the application.
        /// </summary>
        [ObservableProperty]
        private string selectedLanguage = string.Empty;

        // 🔄 FTP Synchronization Configuration

        /// <summary>
        /// Indicates whether FTP synchronization is enabled.
        /// </summary>
        [ObservableProperty]
        private bool enableFtpSync;

        /// <summary>
        /// Hostname or IP address of the FTP server.
        /// </summary>
        [ObservableProperty]
        private string ftpHost = string.Empty;

        /// <summary>
        /// FTP username used for authentication.
        /// </summary>
        [ObservableProperty]
        private string ftpUser = string.Empty;

        /// <summary>
        /// FTP password used for authentication.
        /// </summary>
        [ObservableProperty]
        private string ftpPassword = string.Empty;

        // 🖥 RDP Access Configuration

        /// <summary>
        /// Username for RDP connections.
        /// </summary>
        [ObservableProperty]
        private string rdpUsername = string.Empty;

        /// <summary>
        /// Password for RDP authentication.
        /// </summary>
        [ObservableProperty]
        private string rdpPassword = string.Empty;

        // 🔁 Application Refresh Interval

        /// <summary>
        /// Refresh interval (in seconds) for checking VM and host status.
        /// </summary>
        [ObservableProperty]
        private int autoRefreshIntervalSeconds = 30;

        /// <summary>
        /// Saves all modified settings back to the configuration store.
        /// </summary>
        [RelayCommand]
        private async Task SaveSettingsAsync()
        {
            try
            {
                _settingsManager.Language = SelectedLanguage;
                _settingsManager.EnableFtpSync = EnableFtpSync;
                _settingsManager.FtpHost = FtpHost;
                _settingsManager.FtpUser = FtpUser;
                _settingsManager.FtpPassword = FtpPassword;
                _settingsManager.RdpUsername = RdpUsername;
                _settingsManager.RdpPassword = RdpPassword;
                _settingsManager.AutoRefreshIntervalSeconds = AutoRefreshIntervalSeconds;

                _settingsManager.Save();

                await _dialogService.ShowMessageAsync("✅ Settings saved successfully.", "Success");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync($"❌ Failed to save settings:\n{ex.Message}", "Save Error");
            }
        }
    }
}
