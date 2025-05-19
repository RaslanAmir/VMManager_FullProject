using System.Threading.Tasks;

namespace VMManager.Services.Interfaces
{
    /// <summary>
    /// Provides export, restore, and control operations for virtual machines.
    /// </summary>
    public interface IExportRestoreService
    {
        /// <summary>
        /// Exports a specific VM on the given host to the specified destination path.
        /// </summary>
        /// <param name="host">The Hyper-V host name.</param>
        /// <param name="vm">The virtual machine name.</param>
        /// <param name="destPath">The destination path for the export.</param>
        Task ExportVMAsync(string host, string vm, string destPath);

        /// <summary>
        /// Restores a specific VM on the given host from the specified source path.
        /// </summary>
        /// <param name="host">The Hyper-V host name.</param>
        /// <param name="vm">The virtual machine name.</param>
        /// <param name="sourcePath">The source path of the backup.</param>
        Task RestoreVMAsync(string host, string vm, string sourcePath);

        /// <summary>
        /// Starts the specified VM on the given host.
        /// </summary>
        /// <param name="host">The Hyper-V host name.</param>
        /// <param name="vm">The virtual machine name.</param>
        Task StartVMAsync(string host, string vm);

        /// <summary>
        /// Stops (shuts down) the specified VM on the given host.
        /// </summary>
        /// <param name="host">The Hyper-V host name.</param>
        /// <param name="vm">The virtual machine name.</param>
        Task StopVMAsync(string host, string vm);

        /// <summary>
        /// Restarts the specified VM on the given host.
        /// </summary>
        /// <param name="host">The Hyper-V host name.</param>
        /// <param name="vm">The virtual machine name.</param>
        Task RestartVMAsync(string host, string vm);

        /// <summary>
        /// Connects to the console of the specified VM on the given host.
        /// </summary>
        /// <param name="host">The Hyper-V host name.</param>
        /// <param name="vm">The virtual machine name.</param>
        Task ConnectConsoleAsync(string host, string vm);

        /// <summary>
        /// Exports all virtual machines on the given host to the default backup location.
        /// </summary>
        /// <param name="host">The Hyper-V host name.</param>
        Task ExportAllVMsOnHostAsync(string host);

        /// <summary>
        /// Exports all virtual machines on the given host to the specified backup path.
        /// </summary>
        /// <param name="host">The Hyper-V host name.</param>
        /// <param name="backupPath">The directory to save all VM exports.</param>
        Task ExportAllVMsOnHostAsync(string host, string backupPath);
    }
}
