using CommunityToolkit.Mvvm.ComponentModel;

namespace VMManager.ViewModels
{
    /// <summary>
    /// Base class for all ViewModels in the application.
    /// Provides INotifyPropertyChanged support and common observable properties.
    /// </summary>
    public abstract partial class ViewModelBase : ObservableObject
    {
        /// <summary>
        /// Indicates whether the view is currently performing a task (used to control loading spinners, etc.).
        /// </summary>
        [ObservableProperty]
        private bool isBusy;

        /// <summary>
        /// Display title of the ViewModel (can be used in tabs, headers, or window titles).
        /// </summary>
        [ObservableProperty]
        private string title = string.Empty;

        // 🔧 Extend with shared functionality: Logging, Navigation, Messaging, Theme preferences, etc.
    }
}
