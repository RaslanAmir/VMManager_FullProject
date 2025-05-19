using System;
using System.Windows;
using System.Windows.Data;
using Wpf.Ui.Controls;
using Microsoft.Extensions.DependencyInjection;
using VMManager.Models;
using VMManager.UI.ViewModels;

namespace VMManager.UI.Views
{
    /// <summary>
    /// The main shell window of the application using <see cref="FluentWindow"/>.
    /// Hosts the navigation drawer, main content presenter, and attaches view model logic.
    /// </summary>
    public partial class MainView : FluentWindow
    {
        private readonly MainViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainView"/> class and resolves the <see cref="MainViewModel"/>.
        /// </summary>
        public MainView()
        {
            InitializeComponent();

            _viewModel = App.Services.GetRequiredService<MainViewModel>() ?? 
                         throw new InvalidOperationException("MainViewModel service could not be resolved.");

            DataContext = _viewModel;

            Loaded += OnLoaded;
        }

        /// <summary>
        /// Handles the window's Loaded event to attach filtering logic to the CollectionViewSource.
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (Resources["FilteredVms"] is CollectionViewSource collectionViewSource)
            {
                collectionViewSource.Filter += VmFilter;
                collectionViewSource.View?.Refresh();
            }
        }

        /// <summary>
        /// Filter predicate to apply real-time search on VM list based on <see cref="MainViewModel.SearchText"/>.
        /// </summary>
        private void VmFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is not VMDto vm)
            {
                e.Accepted = false;
                return;
            }

            var query = _viewModel.SearchText?.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(query))
            {
                e.Accepted = true;
                return;
            }

            e.Accepted =
                vm.VMName?.ToLowerInvariant().Contains(query) == true ||
                vm.HostName?.ToLowerInvariant().Contains(query) == true ||
                vm.Status?.ToLowerInvariant().Contains(query) == true;
        }
    }
}
