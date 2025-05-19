using System.Collections.Generic;
using System.Threading.Tasks;
using VMManager.Core.Entities;

namespace VMManager.Services.Interfaces
{
    /// <summary>
    /// Provides asynchronous operations for managing and persisting host configurations.
    /// </summary>
    public interface IHostRepository
    {
        /// <summary>
        /// Retrieves all saved hosts from the underlying data source.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing a collection of known hosts.</returns>
        Task<IEnumerable<Host>> GetAllAsync();

        /// <summary>
        /// Adds a new host configuration to the repository.
        /// </summary>
        /// <param name="host">The host entity to be added.</param>
        /// <returns>A task representing the asynchronous add operation.</returns>
        Task AddAsync(Host host);

        /// <summary>
        /// Updates an existing host configuration in the repository.
        /// </summary>
        /// <param name="host">The updated host entity.</param>
        /// <returns>A task representing the asynchronous update operation.</returns>
        Task UpdateAsync(Host host);

        /// <summary>
        /// Removes a host configuration by its host name.
        /// </summary>
        /// <param name="hostName">The name of the host to remove.</param>
        /// <returns>A task representing the asynchronous delete operation.</returns>
        Task RemoveAsync(string hostName);
    }
}
