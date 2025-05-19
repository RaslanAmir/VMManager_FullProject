using System;
using System.Globalization;
using System.Windows.Data;

namespace VMManager.UI.Converters
{
    /// <summary>
    /// Converts a VM status string (e.g., "Running", "Off") to a corresponding Unicode emoji icon.
    /// Used in UI to visually represent VM operational states.
    /// </summary>
    public sealed class StatusToIconConverter : IValueConverter
    {
        /// <summary>
        /// Converts a VM status string into a Unicode icon representing the state.
        /// </summary>
        /// <param name="value">The status string (e.g., "running", "off").</param>
        /// <param name="targetType">The target binding type.</param>
        /// <param name="parameter">Optional converter parameter (unused).</param>
        /// <param name="culture">The current culture.</param>
        /// <returns>A string containing a Unicode emoji representing the status.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = value?.ToString()?.Trim().ToLowerInvariant();

            return status switch
            {
                "running" => "🟢",
                "off" => "🔴",
                "paused" => "⏸️",
                "saved" => "💾",
                "starting" => "⏳",
                "stopping" => "🚪",
                _ => "❓"
            };
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException("StatusToIconConverter does not support ConvertBack.");
    }
}
