using System.Threading.Tasks;

namespace VMManager.Services.Interfaces
{
    /// <summary>
    /// Provides an abstraction for showing dialogs and messages, decoupled from WPF MessageBox.
    /// Enables clean separation of UI logic for testing and maintainability.
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Shows an informational message dialog.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="title">The dialog title (optional).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ShowMessageAsync(string message, string title = "Information");

        /// <summary>
        /// Shows a confirmation dialog (Yes/No).
        /// </summary>
        /// <param name="message">The question to display.</param>
        /// <param name="title">The dialog title (optional).</param>
        /// <returns>A task that returns true if confirmed, false if canceled.</returns>
        Task<bool> ShowConfirmationAsync(string message, string title = "Confirm");

        /// <summary>
        /// Shows an error dialog.
        /// </summary>
        /// <param name="message">The error message to display.</param>
        /// <param name="title">The dialog title (optional).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ShowErrorAsync(string message, string title = "Error");
    }
}
