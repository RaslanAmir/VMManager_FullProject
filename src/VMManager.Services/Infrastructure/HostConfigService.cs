using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VMManager.Core.Interfaces;
using VMManager.Models;
using VMManager.Services.Interfaces;

namespace VMManager.Services
{
    /// <summary>
    /// Concrete service for managing saved host configurations using persistent JSON storage.
    /// </summary>
    public sealed class HostConfigService : IHostConfigService
    {
        private readonly IFileRepository _fileRepository;
        private const string ConfigFileName = "hostconfig.json";

        /// <summary>
        /// Initializes a new instance of the <see cref="HostConfigService"/> class.
        /// </summary>
        /// <param name="fileRepository">The file repository for persistence.</param>
        public HostConfigService(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository ?? throw new ArgumentNullException(nameof(fileRepository));
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<HostInfo>> GetHostsAsync()
        {
            var hosts = await _fileRepository.ReadAsync<List<HostInfo>>(ConfigFileName);
            return hosts ?? new List<HostInfo>();
        }

        /// <inheritdoc />
        public async Task SaveHostsAsync(IEnumerable<HostInfo> hosts)
        {
            if (hosts == null)
                throw new ArgumentNullException(nameof(hosts));

            await _fileRepository.WriteAsync(ConfigFileName, hosts);
        }

        /// <inheritdoc />
        public async Task AddOrUpdateHostAsync(HostInfo host)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host));

            var hosts = new List<HostInfo>(await GetHostsAsync());
            var index = hosts.FindIndex(h => h.HostName.Equals(host.HostName, StringComparison.OrdinalIgnoreCase));

            if (index >= 0)
                hosts[index] = host;
            else
                hosts.Add(host);

            await SaveHostsAsync(hosts);
        }

        /// <inheritdoc />
        public async Task RemoveHostAsync(string hostName)
        {
            if (string.IsNullOrWhiteSpace(hostName))
                throw new ArgumentException("Host name cannot be null or empty.", nameof(hostName));

            var hosts = new List<HostInfo>(await GetHostsAsync());
            hosts.RemoveAll(h => h.HostName.Equals(hostName, StringComparison.OrdinalIgnoreCase));

            await SaveHostsAsync(hosts);
        }
    }
}
