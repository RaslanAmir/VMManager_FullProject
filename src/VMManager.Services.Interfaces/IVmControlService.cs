using System.Collections.Generic;
using System.Threading.Tasks;
using VMManager.Models;

namespace VMManager.Services.Interfaces
{
    /// <summary>
    /// Defines methods to monitor and control virtual machines (VMs) on Hyper-V hosts.
    /// </summary>
    public interface IVmControlService
    {
        /// <summary>
        /// Retrieves status information for all VMs across all configured hosts.
        /// </summary>
        Task<IEnumerable<VMDto>> GetAllVmStatusAsync();

        /// <summary>
        /// Initiates a graceful shutdown of the specified VM.
        /// </summary>
        Task<string> ShutdownVmAsync(string hostName, string vmName);

        /// <summary>
        /// Starts the specified virtual machine.
        /// </summary>
        Task<string> StartVmAsync(string hostName, string vmName);

        /// <summary>
        /// Connects to the console session of a specified VM.
        /// </summary>
        Task<string> ConnectVmAsync(string hostName, string vmName);

        /// <summary>
        /// Performs a forced restart of the specified VM.
        /// </summary>
        Task<string> RestartVmAsync(string hostName, string vmName);

        /// <summary>
        /// Creates a checkpoint (snapshot) of the specified virtual machine.
        /// </summary>
        Task<string> SnapshotVmAsync(string hostName, string vmName);

        /// <summary>
        /// Restores the latest snapshot for the specified VM.
        /// </summary>
        Task<string> RestoreSnapshotVmAsync(string hostName, string vmName);

        /// <summary>
        /// Initiates shutdown of the entire host machine.
        /// </summary>
        Task<string> ShutdownHostAsync(string hostName);
    }
}
