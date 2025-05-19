using System;
using System.Threading.Tasks;
using VMManager.Models;
using VMManager.Core.Entities;
using VMManager.Core.Interfaces;
using VMManager.Services.Interfaces; // ✅ FIXED interface location

namespace VMManager.Infrastructure.Persistence
{
    /// <summary>
    /// Provides logic to add, update, and remove Hyper-V host configurations.
    /// </summary>
    public sealed class HostManagementService : IHostManagementService
    {
        private readonly IHostRepository _hostRepo;
        private readonly ILogger _logger;

        public HostManagementService(IHostRepository hostRepo, ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(hostRepo);
            ArgumentNullException.ThrowIfNull(logger);

            _hostRepo = hostRepo;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task AddHostAsync(HostDto hostDto)
        {
            ArgumentNullException.ThrowIfNull(hostDto);

            var entity = new Host
            {
                HostName = hostDto.HostName,
                IpAddress = hostDto.IpAddress,
                IsCritical = hostDto.IsCritical
            };

            await _hostRepo.AddAsync(entity);
            _logger.Info($"✅ Host added: {hostDto.HostName}");
        }

        /// <inheritdoc />
        public async Task UpdateHostAsync(HostDto hostDto)
        {
            ArgumentNullException.ThrowIfNull(hostDto);

            var entity = new Host
            {
                HostName = hostDto.HostName,
                IpAddress = hostDto.IpAddress,
                IsCritical = hostDto.IsCritical
            };

            await _hostRepo.UpdateAsync(entity);
            _logger.Info($"✏️ Host updated: {hostDto.HostName}");
        }

        /// <inheritdoc />
        public async Task RemoveHostAsync(string hostName)
        {
            if (string.IsNullOrWhiteSpace(hostName))
                throw new ArgumentException("❌ Host name cannot be empty.", nameof(hostName));

            await _hostRepo.RemoveAsync(hostName);
            _logger.Info($"🗑️ Host removed: {hostName}");
        }
    }
}
