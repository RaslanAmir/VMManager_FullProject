using Microsoft.Extensions.Configuration;
using VMManager.Common.Models; // ✅ Fixed: AppSettings is defined in VMManager.Models

namespace VMManager.Common.Services
{
    /// <summary>
    /// Provides access to application configuration settings.
    /// </summary>
    public class ConfigService
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Strongly-typed application configuration model.
        /// </summary>
        public AppSettings AppConfig { get; }

        /// <summary>
        /// Initializes the configuration service with bound settings.
        /// </summary>
        public ConfigService()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();
            AppConfig = _configuration.Get<AppSettings>() ?? new AppSettings(); // ✅ Safe fallback
        }

        public string GetRdpUsername() => AppConfig.Rdp?.Username ?? string.Empty;

        public string GetRdpPassword() => AppConfig.Rdp?.EncryptedPassword ?? string.Empty;

        public string GetLiteDbConnectionString() => AppConfig.LiteDb ?? string.Empty;
    }
}
