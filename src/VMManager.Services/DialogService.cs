using System;
using System.Threading.Tasks;
using VMManager.Services.Interfaces;

namespace VMManager.Services
{
    /// <summary>
    /// Console-based fallback dialog service for headless environments.
    /// </summary>
    public sealed class DialogService : IDialogService
    {
        public Task ShowMessageAsync(string message, string title = "Information")
        {
            Console.WriteLine($"\n[INFO] {title}: {message}\n");
            return Task.CompletedTask;
        }

        public Task<bool> ShowConfirmationAsync(string message, string title = "Confirm")
        {
            Console.WriteLine($"\n[CONFIRM] {title}: {message} → auto-confirmed ✅\n");
            return Task.FromResult(true);
        }

        public Task ShowErrorAsync(string message, string title = "Error")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"\n[ERROR] {title}: {message}\n");
            Console.ResetColor();
            return Task.CompletedTask;
        }
    }
}
