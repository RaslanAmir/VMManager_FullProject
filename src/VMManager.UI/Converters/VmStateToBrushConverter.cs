using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace VMManager.UI.Converters
{
    /// <summary>
    /// Converts a VM status string (e.g., "Running", "Off") to a corresponding color brush.
    /// Used in UI elements to visually reflect VM state.
    /// </summary>
    public sealed class VmStateToBrushConverter : IValueConverter
    {
        /// <summary>
        /// Converts a VM status string to a SolidColorBrush.
        /// </summary>
        /// <param name="value">The VM state string (e.g., "Running").</param>
        /// <param name="targetType">The target binding type (Brush).</param>
        /// <param name="parameter">Optional additional parameter (unused).</param>
        /// <param name="culture">The current culture.</param>
        /// <returns>A Brush representing the VM state color.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = value?.ToString()?.Trim().ToLowerInvariant();

            return status switch
            {
                "running" => Brushes.LimeGreen,
                "off" => Brushes.Red,
                "paused" => Brushes.Orange,
                "saved" => Brushes.CadetBlue,
                _ => Brushes.Gray
            };
        }

        /// <summary>
        /// Not supported. This converter does not support backward conversion.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException("VmStateToBrushConverter only supports one-way binding.");
    }
}
