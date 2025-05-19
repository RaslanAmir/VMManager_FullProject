using System;
using MaterialDesignThemes.Wpf;

namespace VMManager.Core
{
    public static class ThemeManager
    {
        private static readonly PaletteHelper _paletteHelper = new PaletteHelper();

        /// <summary>
        /// Applies either the Light or Dark base theme.
        /// </summary>
        /// <param name="themeName">"Light" or "Dark" (case‐insensitive)</param>
        public static void Apply(string themeName)
        {
            // Determine which base theme to use
            bool useDark = string.Equals(themeName, "Dark", StringComparison.OrdinalIgnoreCase);

            // Grab the current theme object
            ITheme theme = _paletteHelper.GetTheme();

            // Set the base (light/dark) part of the theme
            theme.SetBaseTheme(useDark ? Theme.Dark : Theme.Light);

            // Push it back to the palette helper
            _paletteHelper.SetTheme(theme);
        }
    }
}
