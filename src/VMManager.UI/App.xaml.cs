using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
using VMManager.Application.Services;
using VMManager.Common.Services;
using VMManager.Core.Interfaces;
using VMManager.Infrastructure.Persistence;
using VMManager.Services;
using VMManager.Services.Infrastructure;
using VMManager.Services.Interfaces;
using VMManager.Services.Scheduling;
using VMManager.UI.Services;
using VMManager.UI.ViewModels;
using VMManager.UI.Views;

namespace VMManager.UI
{
    /// <summary>
    /// Application entry point: configures DI, services, themes, and main UI shell.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Global service provider accessible application-wide.
        /// </summary>
        public static IServiceProvider Services { get; private set; } = null!;

        /// <inheritdoc />
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            Services = serviceCollection.BuildServiceProvider();

            // 🧠 Set WPF UI service locator (for Wpf.Ui)
            Wpf.Ui.Application.Current = this;
            Wpf.Ui.Application.Services = Services;

            // 🚀 Initialize Background Schedulers (Backup jobs, etc.)
            InitializeBackupScheduler();

            // 🖼️ Show main view
            var mainWindow = Services.GetRequiredService<MainView>();
            mainWindow.Show();
        }

        /// <summary>
        /// Configures all required services and ViewModels.
        /// </summary>
        private void ConfigureServices(IServiceCollection services)
        {
            // 🧠 ViewModels
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<DashboardViewModel>();
            services.AddSingleton<ScheduleViewModel>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<VmActionViewModel>();

            // 🖼 Views
            services.AddSingleton<MainView>();
            services.AddTransient<DashboardView>();
            services.AddTransient<ScheduleView>();
            services.AddTransient<SettingsView>();

            // ⚙️ Core Services
            services.AddSingleton<ILogger, Logger>();
            services.AddSingleton<IDialogService, WpfDialogService>();
            services.AddSingleton<ISettingsManager, SettingsManager>();
            services.AddSingleton<IFileRepository, JsonFileRepository>();
            services.AddSingleton<IPowerShellService, PowerShellService>();
            services.AddSingleton<IExportRestoreService, ScriptBasedExportRestoreService>();
            services.AddSingleton<IHostConfigService, HostConfigService>();
            services.AddSingleton<IHostRepository, HostRepository>();
            services.AddSingleton<IHostManagementService, HostManagementService>();
            services.AddSingleton<IVmControlService, VMManager.Services.Infrastructure.VmControlService>(); // real one
            services.AddSingleton<IBackupScheduler, BackupScheduler>();
            services.AddSingleton<IBackupService, BackupService>();
            services.AddSingleton<VmOrchestrationService>();

            // 🎨 WPF UI toolkit (Wpf.Ui 4.x)
            services.AddWpfUi();
        }

        /// <summary>
        /// Starts the backup scheduler and handles first-run failures.
        /// </summary>
        private void InitializeBackupScheduler()
        {
            try
            {
                if (Services.GetService<IBackupScheduler>() is BackupScheduler scheduler)
                {
                    scheduler.Initialize();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Failed to initialize backup scheduler:\n\n{ex.Message}",
                    "Startup Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
