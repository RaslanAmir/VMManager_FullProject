using System;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;
using VMManager.UI.ViewModels;

namespace VMManager.UI.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml.
    /// Binds to <see cref="SettingsViewModel"/> and handles secure password field input.
    /// </summary>
    public partial class SettingsView : UiPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsView"/> class and its components.
        /// </summary>
        public SettingsView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles real-time updates to the FTP password field.
        /// Updates <see cref="SettingsViewModel.FtpPassword"/> securely when text changes.
        /// </summary>
        private void FtpPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox box && DataContext is SettingsViewModel vm)
            {
                vm.FtpPassword = box.Password;
            }
        }

        /// <summary>
        /// Handles real-time updates to the RDP password field.
        /// Updates <see cref="SettingsViewModel.RdpPassword"/> securely when text changes.
        /// </summary>
        private void RdpPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox box && DataContext is SettingsViewModel vm)
            {
                vm.RdpPassword = box.Password;
            }
        }
    }
}
