using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using VMManager.Models;
using VMManager.Services.Interfaces; // ✅ FIXED: Interface reference added

namespace VMManager.Application.Services
{
    /// <summary>
    /// Mock implementation of IHostManagementService using in-memory storage.
    /// Useful for design-time binding and unit testing.
    /// </summary>
    public class MockHostManagementService : IHostManagementService
    {
        /// <summary>
        /// In-memory collection of hosts.
        /// </summary>
        public ObservableCollection<HostDto> Hosts { get; } = new();

        /// <inheritdoc />
        public Task AddHostAsync(HostDto host)
        {
            if (host == null) throw new ArgumentNullException(nameof(host));
            if (Hosts.Any(h => h.HostName.Equals(host.HostName, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Host '{host.HostName}' already exists.");

            Hosts.Add(Clone(host));
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task UpdateHostAsync(HostDto host)
        {
            if (host == null) throw new ArgumentNullException(nameof(host));

            var existing = Hosts.FirstOrDefault(h => h.HostName.Equals(host.HostName, StringComparison.OrdinalIgnoreCase));
            if (existing == null)
                throw new InvalidOperationException($"Host '{host.HostName}' not found.");

            existing.IpAddress = host.IpAddress;
            existing.IsCritical = host.IsCritical;
            existing.Vms = new ObservableCollection<VMDto>(host.Vms ?? new());

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task RemoveHostAsync(string hostName)
        {
            var host = Hosts.FirstOrDefault(h => h.HostName.Equals(hostName, StringComparison.OrdinalIgnoreCase));
            if (host != null)
                Hosts.Remove(host);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Creates a deep clone of a HostDto to isolate state.
        /// </summary>
        private static HostDto Clone(HostDto source)
        {
            return new HostDto
            {
                HostName = source.HostName,
                IpAddress = source.IpAddress,
                IsCritical = source.IsCritical,
                IsOnline = source.IsOnline,
                LastPingTime = source.LastPingTime,
                Tags = new(source.Tags ?? new()),
                Vms = new ObservableCollection<VMDto>(
                    source.Vms.Select(vm => new VMDto
                    {
                        HostName = vm.HostName,
                        VMName = vm.VMName,
                        Status = vm.Status,
                        IsCritical = vm.IsCritical
                    }))
            };
        }
    }
}
