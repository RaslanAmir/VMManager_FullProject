using System;
using System.Windows;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
using VMManager.UI.ViewModels;

namespace VMManager.UI.Views
{
    /// <summary>
    /// Main application window that serves as the root container for all views and controls.
    /// Supports dynamic theming and navigation.
    /// </summary>
    public partial class MainWindow : FluentWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        /// <param name="viewModel">Injected <see cref="MainViewModel"/> responsible for managing app views.</param>
        /// <exception cref="ArgumentNullException">Thrown if the provided view model is null.</exception>
        public MainWindow(MainViewModel viewModel)
        {
            DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel), "MainViewModel cannot be null.");

            InitializeComponent();

            Loaded += OnMainWindowLoaded;
        }

        /// <summary>
        /// Executes logic after the window has finished loading.
        /// </summary>
        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            this.CenterOnScreen();

            // 🌓 Theme persistence could be initialized here later
        }
    }
}
