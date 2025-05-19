using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using VMManager.Models;

namespace VMManager.Core
{
    /// <summary>
    /// Manages localization settings and loading/saving of host configurations.
    /// </summary>
    public static class LocalizationManager
    {
        private static readonly string HostsConfigPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hosts_config.txt");

        /// <summary>
        /// Gets or sets the current language (culture code).
        /// </summary>
        public static string CurrentLanguage { get; set; } = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

        /// <summary>
        /// Returns a list of available language codes supported by the app.
        /// </summary>
        public static List<string> GetAvailableLanguages() => new() { "en", "hr" };

        /// <summary>
        /// Loads the list of host entries from the local configuration file.
        /// </summary>
        public static List<HostInfo> LoadHosts()
        {
            if (!File.Exists(HostsConfigPath))
                return new List<HostInfo>();

            var json = File.ReadAllText(HostsConfigPath);
            return JsonConvert.DeserializeObject<List<HostInfo>>(json) ?? new List<HostInfo>();
        }
    }
}
