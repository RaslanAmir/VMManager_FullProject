using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMManager.Core.Entities;
using VMManager.Services.Interfaces; // ✅ Correct namespace!
using Xunit;

namespace VMManager.Tests.Core
{
    public class HostRepositoryTests
    {
        private class InMemoryHostRepository : IHostRepository
        {
            private readonly List<Host> _hosts = new();

            public Task AddAsync(Host host)
            {
                _hosts.Add(host);
                return Task.CompletedTask;
            }

            public Task<IEnumerable<Host>> GetAllAsync()
            {
                return Task.FromResult<IEnumerable<Host>>(_hosts);
            }

            public Task RemoveAsync(string hostName)
            {
                _hosts.RemoveAll(h => h.HostName == hostName);
                return Task.CompletedTask;
            }

            public Task UpdateAsync(Host updatedHost)
            {
                var existing = _hosts.FirstOrDefault(h => h.HostName == updatedHost.HostName);
                if (existing != null)
                {
                    existing.IpAddress = updatedHost.IpAddress;
                }
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task AddAndGetHosts_ShouldWork()
        {
            IHostRepository repo = new InMemoryHostRepository();
            var host = new Host { HostName = "test", IpAddress = "127.0.0.1" };
            await repo.AddAsync(host);
            var hosts = await repo.GetAllAsync();
            Assert.Contains(hosts, h => h.HostName == "test");
        }
    }
}
