using System.Threading.Tasks;
using System.Windows;
using VMManager.Services.Interfaces;

namespace VMManager.UI.Services
{
    /// <summary>
    /// WPF implementation of <see cref="IDialogService"/> using native MessageBox dialogs.
    /// Ensures UI-safe asynchronous message display via Dispatcher.
    /// </summary>
    public sealed class WpfDialogService : IDialogService
    {
        /// <inheritdoc />
        public async Task ShowMessageAsync(string message, string title = "Information")
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        /// <inheritdoc />
        public async Task<bool> ShowConfirmationAsync(string message, string title = "Confirm")
        {
            var result = await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
            });

            return result;
        }

        /// <inheritdoc />
        public async Task ShowErrorAsync(string message, string title = "Error")
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }
    }
}
