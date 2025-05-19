using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace VMManager.UI.Converters
{
    /// <summary>
    /// Converts a boolean value to a specific <see cref="Brush"/> based on true, false, or null.
    /// Commonly used in UI to visually represent states.
    /// </summary>
    public sealed class BoolToBrushConverter : IValueConverter
    {
        /// <summary>
        /// Brush used when the boolean value is true.
        /// </summary>
        public Brush TrueBrush { get; set; } = Brushes.Green;

        /// <summary>
        /// Brush used when the boolean value is false.
        /// </summary>
        public Brush FalseBrush { get; set; } = Brushes.Red;

        /// <summary>
        /// Brush used when the input is null or not a boolean.
        /// </summary>
        public Brush NullBrush { get; set; } = Brushes.Gray;

        /// <summary>
        /// Converts a boolean or nullable boolean to the corresponding brush.
        /// </summary>
        /// <param name="value">The value to convert (expected to be boolean).</param>
        /// <param name="targetType">The target type (should be Brush).</param>
        /// <param name="parameter">Optional parameter (unused).</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The corresponding brush.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                bool b => b ? TrueBrush : FalseBrush,
                _ => NullBrush
            };
        }

        /// <summary>
        /// Not supported: Converts a brush back to a boolean value.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException("BoolToBrushConverter does not support ConvertBack.");
    }
}
