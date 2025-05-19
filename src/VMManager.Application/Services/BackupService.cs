using System;
using System.Threading.Tasks;
using VMManager.Services.Interfaces;
using VMManager.Common.Logging;

namespace VMManager.Application.Services
{
    /// <summary>
    /// Provides functionality to trigger VM backups across all configured hosts.
    /// </summary>
    public sealed class BackupService : IBackupService
    {
        private readonly IHostRepository _hostRepository;
        private readonly IExportRestoreService _exportRestoreService;
        private readonly ILoggingService _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackupService"/> class.
        /// </summary>
        public BackupService(
            IHostRepository hostRepository,
            IExportRestoreService exportRestoreService,
            ILoggingService logger)
        {
            _hostRepository = hostRepository ?? throw new ArgumentNullException(nameof(hostRepository));
            _exportRestoreService = exportRestoreService ?? throw new ArgumentNullException(nameof(exportRestoreService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task RunBackupOnceAsync()
        {
            var hosts = await _hostRepository.GetAllAsync();

            foreach (var host in hosts)
            {
                try
                {
                    _logger.LogInformation($"🚀 [Backup] Starting backup for host: {host.HostName}");
                    await _exportRestoreService.ExportAllVMsOnHostAsync(host.HostName);
                    _logger.LogInformation($"✅ [Backup] Successfully completed backup for: {host.HostName}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ [Backup] Failed to back up host '{host.HostName}'", ex);
                }
            }
        }
    }
}
