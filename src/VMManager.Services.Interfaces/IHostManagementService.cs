using System.Threading.Tasks;
using VMManager.Models;

namespace VMManager.Services.Interfaces
{
    /// <summary>
    /// Abstraction for managing the lifecycle of Hyper-V host definitions.
    /// </summary>
    public interface IHostManagementService
    {
        /// <summary>
        /// Adds a new host entry to the system.
        /// </summary>
        /// <param name="host">The host details to be added.</param>
        Task AddHostAsync(HostDto host);

        /// <summary>
        /// Updates an existing host entry.
        /// </summary>
        /// <param name="host">The updated host details.</param>
        Task UpdateHostAsync(HostDto host);

        /// <summary>
        /// Removes a host from the system based on its unique hostname.
        /// </summary>
        /// <param name="hostName">The hostname of the host to remove.</param>
        Task RemoveHostAsync(string hostName);
    }
}
