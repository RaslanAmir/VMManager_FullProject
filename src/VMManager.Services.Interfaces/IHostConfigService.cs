using System.Collections.Generic;
using System.Threading.Tasks;
using VMManager.Models;

namespace VMManager.Services.Interfaces
{
    /// <summary>
    /// Provides access to saved Hyper-V host configurations used throughout the application.
    /// </summary>
    public interface IHostConfigService
    {
        /// <summary>
        /// Gets all configured Hyper-V hosts from the underlying store.
        /// </summary>
        /// <returns>A task containing a read-only list of <see cref="HostInfo"/> entries.</returns>
        Task<IReadOnlyList<HostInfo>> GetHostsAsync();

        /// <summary>
        /// Saves the complete list of hosts, overwriting previous data.
        /// </summary>
        /// <param name="hosts">The collection of hosts to save.</param>
        Task SaveHostsAsync(IEnumerable<HostInfo> hosts);

        /// <summary>
        /// Adds a new host or updates the existing one by matching <see cref="HostInfo.HostName"/>.
        /// </summary>
        /// <param name="host">The host to add or update.</param>
        Task AddOrUpdateHostAsync(HostInfo host);

        /// <summary>
        /// Removes the host with the specified name.
        /// </summary>
        /// <param name="hostName">The name of the host to remove.</param>
        Task RemoveHostAsync(string hostName);
    }
}
