using System;
using System.Globalization;
using System.Windows.Data;

namespace VMManager.UI.Converters
{
    /// <summary>
    /// Converts a SmartEnum instance into its string representation.
    /// Typically used in XAML to display enum names in combo boxes or data bindings.
    /// </summary>
    public sealed class SmartEnumToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts a SmartEnum value to its name string.
        /// </summary>
        /// <param name="value">The SmartEnum object to convert.</param>
        /// <param name="targetType">The target binding type (usually string).</param>
        /// <param name="parameter">Optional converter parameter (not used).</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>The name of the SmartEnum, or empty string if null.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Not supported: SmartEnum binding is typically one-way.
        /// </summary>
        /// <exception cref="NotSupportedException">Always thrown.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("SmartEnumToStringConverter does not support ConvertBack.");
        }
    }
}
