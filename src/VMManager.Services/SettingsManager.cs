using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using VMManager.Core;
using VMManager.Common.Models;
using VMManager.Services.Interfaces;

namespace VMManager.Services
{
    /// <summary>
    /// Provides persistent storage and access to application-wide configuration settings.
    /// </summary>
    public sealed class SettingsManager : ISettingsManager
    {
        private static readonly string SettingsFile =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

        private AppSettings _settings = new()
        {
            Rdp = new RdpSettings(),
            Hosts = new List<HostSettings>()
        };

        private static readonly Logger Logger = new();

        public SettingsManager()
        {
            Load();
        }

        /// <summary>
        /// Loads the settings file or initializes defaults if invalid.
        /// </summary>
        private void Load()
        {
            try
            {
                if (File.Exists(SettingsFile))
                {
                    var json = File.ReadAllText(SettingsFile);
                    var loaded = JsonConvert.DeserializeObject<AppSettings>(json);

                    if (loaded != null)
                    {
                        loaded.Rdp ??= new RdpSettings();
                        loaded.Hosts ??= new List<HostSettings>();
                        _settings = loaded;
                        Logger.Info("✅ Settings loaded successfully.");
                        return;
                    }
                }

                Logger.Warn("⚠️ settings.json missing or invalid — applying defaults.");
                Save();
            }
            catch (Exception ex)
            {
                Logger.Error($"❌ Failed to load settings: {ex.Message}");
            }
        }

        /// <summary>
        /// Saves the current settings object to disk.
        /// </summary>
        public void Save()
        {
            try
            {
                var json = JsonConvert.SerializeObject(_settings, Formatting.Indented);
                File.WriteAllText(SettingsFile, json);
                Logger.Info("💾 Settings saved to disk.");
            }
            catch (Exception ex)
            {
                Logger.Error($"❌ Failed to save settings: {ex.Message}");
            }
        }

        // 🌐 Language & Localization
        public string Language
        {
            get => LocalizationManager.CurrentLanguage;
            set => LocalizationManager.CurrentLanguage = value;
        }

        public IEnumerable<string> GetSupportedLanguages() =>
            LocalizationManager.GetAvailableLanguages();

        // 🔄 FTP Configuration
        public bool EnableFtpSync
        {
            get => _settings?.EnableFtpSync ?? false;
            set
            {
                EnsureSettingsInitialized();
                _settings.EnableFtpSync = value;
            }
        }

        public string FtpHost
        {
            get => _settings?.FtpHost ?? string.Empty;
            set
            {
                EnsureSettingsInitialized();
                _settings.FtpHost = value?.Trim() ?? string.Empty;
            }
        }

        public string FtpUser
        {
            get => _settings?.FtpUser ?? string.Empty;
            set
            {
                EnsureSettingsInitialized();
                _settings.FtpUser = value?.Trim() ?? string.Empty;
            }
        }

        public string FtpPassword
        {
            get => _settings?.FtpPassword ?? string.Empty;
            set
            {
                EnsureSettingsInitialized();
                _settings.FtpPassword = value?.Trim() ?? string.Empty;
            }
        }

        // 🖥 RDP Configuration
        public string RdpUsername
        {
            get => _settings?.Rdp?.Username ?? string.Empty;
            set
            {
                EnsureSettingsInitialized();
                _settings.Rdp ??= new RdpSettings();
                _settings.Rdp.Username = value?.Trim() ?? string.Empty;
            }
        }

        public string RdpPassword
        {
            get => _settings?.Rdp?.EncryptedPassword ?? string.Empty;
            set
            {
                EnsureSettingsInitialized();
                _settings.Rdp ??= new RdpSettings();
                _settings.Rdp.EncryptedPassword = value?.Trim() ?? string.Empty;
            }
        }

        // 🔄 Auto Refresh Settings
        public int AutoRefreshIntervalSeconds
        {
            get => _settings?.AutoRefreshIntervalSeconds > 0
                ? _settings.AutoRefreshIntervalSeconds
                : 30;
            set
            {
                EnsureSettingsInitialized();
                _settings.AutoRefreshIntervalSeconds = value > 0 ? value : 30;
            }
        }

        // 🧠 Host/VM Mapping Access
        public IEnumerable<string> GetHosts()
        {
            if (_settings?.Hosts == null || _settings.Hosts.Count == 0)
                return Array.Empty<string>();

            var result = new List<string>();
            foreach (var host in _settings.Hosts)
            {
                if (!string.IsNullOrWhiteSpace(host.HostName))
                    result.Add(host.HostName);
            }

            return result;
        }

        public IEnumerable<string> GetVmsForHost(string host)
        {
            if (string.IsNullOrWhiteSpace(host) || _settings?.Hosts == null)
                return Array.Empty<string>();

            var hostObj = _settings.Hosts.Find(h =>
                h.HostName.Equals(host, StringComparison.OrdinalIgnoreCase));

            return hostObj?.VirtualMachines?.Count > 0
                ? hostObj.VirtualMachines
                : Array.Empty<string>();
        }

        // 📁 Path Access
        public string GetExportPath() =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "exports");

        public string GetRestorePath() =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "restores");

        /// <summary>
        /// Defensive check to ensure _settings is initialized.
        /// </summary>
        private void EnsureSettingsInitialized()
        {
            _settings ??= new AppSettings();
            _settings.Rdp ??= new RdpSettings();
            _settings.Hosts ??= new List<HostSettings>();
        }
    }
}
