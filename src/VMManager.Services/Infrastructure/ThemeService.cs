using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VMManager.Core.Interfaces;

namespace VMManager.Services.Infrastructure
{
    /// <summary>
    /// Provides methods to manage and persist the application's UI theme settings.
    /// </summary>
    public class ThemeService : IThemeService
    {
        private const string FileName = "theme.json";
        private readonly IFileRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeService"/> class.
        /// </summary>
        /// <param name="repository">The file repository used to persist theme settings.</param>
        /// <exception cref="ArgumentNullException">Thrown if repository is null.</exception>
        public ThemeService(IFileRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Returns a list of supported themes.
        /// </summary>
        /// <returns>A list of theme names.</returns>
        public IEnumerable<string> GetAvailableThemes() =>
            new[] { "Light", "Dark" };

        /// <summary>
        /// Returns the currently active theme.
        /// </summary>
        /// <returns>The name of the currently applied theme.</returns>
        public string GetCurrentTheme()
        {
            // 📝 Future: Could load from repository
            return "Light";
        }

        /// <summary>
        /// Persists the selected theme asynchronously.
        /// </summary>
        /// <param name="theme">The theme to set (e.g., "Light", "Dark").</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentException">Thrown if theme is null or empty.</exception>
        public async Task SetThemeAsync(string theme)
        {
            if (string.IsNullOrWhiteSpace(theme))
                throw new ArgumentException("Theme cannot be null or empty.", nameof(theme));

            await _repository.WriteAsync(FileName, theme);
        }
    }
}
