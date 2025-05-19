using System;
using System.Globalization;
using System.Windows.Data;

namespace VMManager.UI.Converters
{
    /// <summary>
    /// A value converter that filters VM names based on a search query.
    /// Used in XAML bindings to control VM list visibility.
    /// </summary>
    public sealed class VmFilter : IValueConverter
    {
        /// <summary>
        /// Determines whether the provided VM name contains the search keyword.
        /// </summary>
        /// <param name="value">The VM name (string).</param>
        /// <param name="targetType">The expected return type (bool).</param>
        /// <param name="parameter">The search string.</param>
        /// <param name="culture">The current culture info.</param>
        /// <returns>True if the VM name matches the search; otherwise, false.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string vmName || string.IsNullOrWhiteSpace(vmName))
                return false;

            var search = parameter?.ToString()?.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(search))
                return true;

            return vmName.ToLowerInvariant().Contains(search);
        }

        /// <summary>
        /// Not supported — one-way conversion only.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException("VmFilter does not support ConvertBack.");
    }
}
