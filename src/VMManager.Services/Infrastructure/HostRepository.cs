using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMManager.Core.Entities;
using VMManager.Core.Interfaces;
using VMManager.Services.Interfaces;

namespace VMManager.Services
{
    /// <summary>
    /// Provides file-backed CRUD operations for managing <see cref="Host"/> entities.
    /// </summary>
    public sealed class HostRepository : IHostRepository
    {
        private readonly IFileRepository _fileRepository;
        private const string DataFile = "hostdb.json";

        /// <summary>
        /// Initializes a new instance of the <see cref="HostRepository"/> class.
        /// </summary>
        /// <param name="fileRepository">File repository for persistent storage.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="fileRepository"/> is null.</exception>
        public HostRepository(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository ?? throw new ArgumentNullException(nameof(fileRepository));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Host>> GetAllAsync()
        {
            var hosts = await _fileRepository.ReadAsync<List<Host>>(DataFile);
            return hosts ?? Enumerable.Empty<Host>();
        }

        /// <inheritdoc />
        public async Task AddAsync(Host host)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host), "Host cannot be null.");

            var hosts = (await GetAllAsync()).ToList();

            if (hosts.Any(h => h.Name.Equals(host.Name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Host '{host.Name}' already exists.");

            hosts.Add(host);
            await _fileRepository.WriteAsync(DataFile, hosts);
        }

        /// <inheritdoc />
        public async Task UpdateAsync(Host host)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host), "Host cannot be null.");

            var hosts = (await GetAllAsync()).ToList();
            var index = hosts.FindIndex(h => h.Name.Equals(host.Name, StringComparison.OrdinalIgnoreCase));

            if (index >= 0)
            {
                hosts[index] = host;
                await _fileRepository.WriteAsync(DataFile, hosts);
            }
            else
            {
                throw new InvalidOperationException($"Host '{host.Name}' not found for update.");
            }
        }

        /// <inheritdoc />
        public async Task RemoveAsync(string hostName)
        {
            if (string.IsNullOrWhiteSpace(hostName))
                throw new ArgumentException("Host name cannot be null or empty.", nameof(hostName));

            var hosts = (await GetAllAsync()).ToList();
            var removed = hosts.RemoveAll(h => h.Name.Equals(hostName, StringComparison.OrdinalIgnoreCase));

            if (removed > 0)
            {
                await _fileRepository.WriteAsync(DataFile, hosts);
            }
            else
            {
                throw new InvalidOperationException($"Host '{hostName}' not found for removal.");
            }
        }
    }
}
