using System;
using System.Globalization;
using System.Collections.Generic;
using System.Threading.Tasks;
using VMManager.Core.Interfaces;

namespace VMManager.Services.Infrastructure
{
    /// <summary>
    /// Service for managing the application's localization and culture settings.
    /// </summary>
    public sealed class LanguageService : ILanguageService
    {
        private const string FileName = "cultures.json";
        private readonly IFileRepository _repo;

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageService"/> class.
        /// </summary>
        /// <param name="repo">The file repository for persisting selected culture.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="repo"/> is null.</exception>
        public LanguageService(IFileRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// Gets the list of cultures supported by the application.
        /// </summary>
        /// <returns>An enumerable of available <see cref="CultureInfo"/> objects.</returns>
        public IEnumerable<CultureInfo> GetAvailableCultures() => new[]
        {
            CultureInfo.InvariantCulture,
            new CultureInfo("en-US"),
            new CultureInfo("de-DE")
            // Add additional cultures here (e.g., new CultureInfo("hr-HR"))
        };

        /// <summary>
        /// Gets the currently active thread culture.
        /// </summary>
        /// <returns>The current <see cref="CultureInfo"/>.</returns>
        public CultureInfo GetCurrentCulture() =>
            CultureInfo.DefaultThreadCurrentCulture ?? CultureInfo.InvariantCulture;

        /// <summary>
        /// Sets the culture for the current thread and saves it persistently.
        /// </summary>
        /// <param name="culture">The culture to set.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="culture"/> is null.</exception>
        public async Task SetCultureAsync(CultureInfo culture)
        {
            if (culture == null)
                throw new ArgumentNullException(nameof(culture));

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            await _repo.WriteAsync(FileName, culture.Name);
        }
    }
}
