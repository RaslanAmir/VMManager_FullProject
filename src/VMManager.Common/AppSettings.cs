using System.Collections.Generic;

namespace VMManager.Common.Models
{
    /// <summary>
    /// Represents application-wide persisted user preferences and system settings.
    /// </summary>
    public class AppSettings
    {
        public string Language { get; set; } = "en-US";
        public bool EnableFtpSync { get; set; }
        public string FtpHost { get; set; } = string.Empty;
        public string FtpUser { get; set; } = string.Empty;
        public string FtpPassword { get; set; } = string.Empty;
        public int AutoRefreshIntervalSeconds { get; set; } = 60;
        public string Theme { get; set; } = "Light";
        public string LiteDb { get; set; } = string.Empty; // ✅ Fix: required in ConfigService

        public RdpSettings? Rdp { get; set; } = new();
        public List<HostSettings> Hosts { get; set; } = new();
    }
} // ✅ HostSettings and RdpSettings were moved to their own files
