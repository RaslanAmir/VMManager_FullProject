using System;
using System.Globalization;
using System.Windows.Data;

namespace VMManager.UI.Converters
{
    /// <summary>
    /// Converts an enum value to its string representation for UI binding,
    /// and converts a string back to an enum for editing scenarios (e.g., ComboBox).
    /// </summary>
    public sealed class EnumToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts an enum value to a human-readable string.
        /// </summary>
        /// <param name="value">The enum value to convert.</param>
        /// <param name="targetType">The target type (usually string).</param>
        /// <param name="parameter">Optional converter parameter (unused).</param>
        /// <param name="culture">The culture to use during conversion.</param>
        /// <returns>The string name of the enum, or empty if value is null.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Converts a string back to the specified enum type.
        /// </summary>
        /// <param name="value">The string representation of the enum.</param>
        /// <param name="targetType">The enum type to convert to.</param>
        /// <param name="parameter">Optional converter parameter (unused).</param>
        /// <param name="culture">The culture to use during conversion.</param>
        /// <returns>The corresponding enum value or <see cref="Binding.DoNothing"/> on failure.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string stringValue || string.IsNullOrWhiteSpace(stringValue) || targetType == null)
                return Binding.DoNothing;

            return Enum.TryParse(targetType, stringValue.Trim(), ignoreCase: true, out var result)
                ? result
                : Binding.DoNothing;
        }
    }
}
