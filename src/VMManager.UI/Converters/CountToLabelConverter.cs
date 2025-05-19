using System;
using System.Globalization;
using System.Windows.Data;

namespace VMManager.UI.Converters
{
    /// <summary>
    /// Converts the number of virtual machines (VMs) into a user-friendly label like "VMs: 5".
    /// Intended for status bars or summary UI elements.
    /// </summary>
    public sealed class CountToLabelConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts multiple input values into a formatted VM count label.
        /// </summary>
        /// <param name="values">The bound values (expects first to be an integer).</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">Optional converter parameter (unused).</param>
        /// <param name="culture">The culture to use for formatting.</param>
        /// <returns>A formatted string like "VMs: 5".</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 0 || values[0] == null)
                return "VMs: 0";

            return values[0] is int count
                ? $"VMs: {count}"
                : "VMs: ?";
        }

        /// <summary>
        /// Not supported. Converting back to multiple values is not implemented.
        /// </summary>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
            throw new NotSupportedException("CountToLabelConverter does not support ConvertBack.");
    }
}
