using System.Collections.Generic;

namespace VMManager.Services.Interfaces
{
    /// <summary>
    /// Manages application settings including localization, FTP configuration, RDP credentials, and persistence.
    /// </summary>
    public interface ISettingsManager
    {
        /// <summary>
        /// Gets or sets the current application language code (e.g., "en", "hr").
        /// </summary>
        string Language { get; set; }

        /// <summary>
        /// Gets or sets whether FTP synchronization is enabled.
        /// </summary>
        bool EnableFtpSync { get; set; }

        /// <summary>
        /// Gets or sets the FTP server host name or address.
        /// </summary>
        string FtpHost { get; set; }

        /// <summary>
        /// Gets or sets the FTP username.
        /// </summary>
        string FtpUser { get; set; }

        /// <summary>
        /// Gets or sets the FTP password.
        /// </summary>
        string FtpPassword { get; set; }

        /// <summary>
        /// Gets or sets the RDP username.
        /// </summary>
        string RdpUsername { get; set; }

        /// <summary>
        /// Gets or sets the RDP password.
        /// </summary>
        string RdpPassword { get; set; }

        /// <summary>
        /// Gets or sets the interval in seconds for auto-refresh operations.
        /// </summary>
        int AutoRefreshIntervalSeconds { get; set; }

        /// <summary>
        /// Gets a list of supported language codes for localization (e.g., "en", "hr").
        /// </summary>
        /// <returns>A list of language codes.</returns>
        IEnumerable<string> GetSupportedLanguages();

        /// <summary>
        /// Gets a list of configured host names.
        /// </summary>
        /// <returns>A list of host names.</returns>
        IEnumerable<string> GetHosts();

        /// <summary>
        /// Gets a list of VM names associated with the specified host.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <returns>A list of VM names for the given host.</returns>
        IEnumerable<string> GetVmsForHost(string host);

        /// <summary>
        /// Gets the default path for exporting virtual machines.
        /// </summary>
        /// <returns>The export path.</returns>
        string GetExportPath();

        /// <summary>
        /// Gets the default path for restoring virtual machines.
        /// </summary>
        /// <returns>The restore path.</returns>
        string GetRestorePath();

        /// <summary>
        /// Persists current settings to the configuration storage (e.g., JSON file).
        /// </summary>
        void Save();
    }
}
