using System.Collections.Generic;
using System.Threading.Tasks;
using VMManager.Models; // ✅ Corrected namespace for VMDto
using VMManager.Services.Interfaces;

namespace VMManager.Application.Services
{
    /// <summary>
    /// Simulated VM control service for testing UI without backend logic.
    /// Replace this with the real PowerShell/Hyper-V service in production.
    /// </summary>
    public class VmControlService : IVmControlService
    {
        /// <inheritdoc />
        public Task<IEnumerable<VMDto>> GetAllVmStatusAsync()
        {
            var vms = new List<VMDto>
            {
                new() { HostName = "HYPER-V-YAS009", VMName = "aplikata", Status = "Running", IsCritical = false },
                new() { HostName = "HYPER-V-YAS009", VMName = "DHCP01", Status = "Running", IsCritical = true },
                new() { HostName = "HYPER-V-YAS009", VMName = "EPO", Status = "Running", IsCritical = false },
                new() { HostName = "HYPER-V-YAS009", VMName = "SYN-BAK", Status = "Running", IsCritical = false },
                new() { HostName = "HYPER-V-YAS009", VMName = "w2k12r2-on-009", Status = "Running", IsCritical = false },

                new() { HostName = "HYPER-V-YAS004", VMName = "Filesrv", Status = "Running", IsCritical = false },
                new() { HostName = "HYPER-V-YAS004", VMName = "Ydc", Status = "Running", IsCritical = true },

                new() { HostName = "HYPER-V-YAS049", VMName = "Appsrv", Status = "Running", IsCritical = false },
                new() { HostName = "HYPER-V-YAS049", VMName = "SYN-CON", Status = "Running", IsCritical = false },
                new() { HostName = "HYPER-V-YAS049", VMName = "Veeam Backup O365", Status = "Running", IsCritical = false },
                new() { HostName = "HYPER-V-YAS049", VMName = "y-bdc", Status = "Running", IsCritical = true },
                new() { HostName = "HYPER-V-YAS049", VMName = "y-wireguard2", Status = "Running", IsCritical = false }
            };

            return Task.FromResult<IEnumerable<VMDto>>(vms);
        }

        /// <inheritdoc />
        public Task<string> ShutdownVmAsync(string hostName, string vmName) =>
            Task.FromResult($"🛑 Shutdown initiated for VM '{vmName}' on host '{hostName}'.");

        /// <inheritdoc />
        public Task<string> StartVmAsync(string hostName, string vmName) =>
            Task.FromResult($"▶️ Start initiated for VM '{vmName}' on host '{hostName}'.");

        /// <inheritdoc />
        public Task<string> ConnectVmAsync(string hostName, string vmName) =>
            Task.FromResult($"🔗 Connect initiated for VM '{vmName}' on host '{hostName}'.");

        /// <inheritdoc />
        public Task<string> RestartVmAsync(string hostName, string vmName) =>
            Task.FromResult($"🔁 Restart initiated for VM '{vmName}' on host '{hostName}'.");

        /// <inheritdoc />
        public Task<string> SnapshotVmAsync(string hostName, string vmName) =>
            Task.FromResult($"📸 Snapshot initiated for VM '{vmName}' on host '{hostName}'.");

        /// <inheritdoc />
        public Task<string> RestoreSnapshotVmAsync(string hostName, string vmName) =>
            Task.FromResult($"♻️ Restore snapshot initiated for VM '{vmName}' on host '{hostName}'.");

        /// <inheritdoc />
        public Task<string> ShutdownHostAsync(string hostName) =>
            Task.FromResult($"⚠️ Shutdown initiated for host '{hostName}'.");
    }
}
