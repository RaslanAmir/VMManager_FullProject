using CommunityToolkit.Mvvm.ComponentModel;

namespace VMManager.ViewModels
{
    /// <summary>
    /// Base class for all ViewModels in the application.
    /// Inherits from <see cref="ObservableObject"/> to support property change notifications.
    /// Designed to provide shared logic and bindable properties across all ViewModels.
    /// </summary>
    public abstract partial class BaseViewModel : ObservableObject
    {
        /// <summary>
        /// Indicates whether a background task is in progress.
        /// Can be used to display a loading spinner or disable UI.
        /// </summary>
        [ObservableProperty]
        private bool isBusy;

        /// <summary>
        /// Optional display title for the ViewModel, useful in tab headers or navigation views.
        /// </summary>
        [ObservableProperty]
        private string title = string.Empty;

        // ??? Future shared features: navigation support, messaging, logging, etc.
    }
}
