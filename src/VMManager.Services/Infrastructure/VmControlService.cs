using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMManager.Models;
using VMManager.Services.Interfaces;
using VMManager.Common.Logging;

namespace VMManager.Services.Infrastructure
{
    /// <summary>
    /// Real implementation of IVmControlService using PowerShell and Hyper-V.
    /// </summary>
    public sealed class VmControlService : IVmControlService
    {
        private readonly IPowerShellService _ps;
        private readonly ILoggingService _logger;

        public VmControlService(IPowerShellService ps, ILoggingService logger)
        {
            _ps = ps ?? throw new ArgumentNullException(nameof(ps));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<VMDto>> GetAllVmStatusAsync()
        {
            const string script = @"
                Get-VM | Select-Object -Property Name, State, ComputerName
            ";

            try
            {
                var (output, errors) = await _ps.InvokeAsync(script, new Dictionary<string, object>());

                if (errors.Length > 0)
                {
                    _logger.LogError($"PowerShell errors in GetAllVmStatusAsync:\n{string.Join("\n", errors)}", new Exception("PowerShell error"));
                }

                var result = output
                    .Select(line =>
                    {
                        var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length < 3) return null;

                        return new VMDto
                        {
                            HostName = parts[2],
                            VMName = parts[0],
                            Status = parts[1],
                            IsCritical = parts[0].ToLower().Contains("dc") || parts[0].ToLower().Contains("dhcp")
                        };
                    })
                    .Where(vm => vm != null)!
                    .ToList();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to retrieve VM status.", ex);
                return Enumerable.Empty<VMDto>();
            }
        }

        public async Task<string> ShutdownVmAsync(string hostName, string vmName)
        {
            var script = "Stop-VM -ComputerName $Host -Name $VM -Force";
            var parameters = new Dictionary<string, object> { { "Host", hostName }, { "VM", vmName } };
            return await InvokeAndFormat("Shutdown VM", script, parameters);
        }

        public async Task<string> StartVmAsync(string hostName, string vmName)
        {
            var script = "Start-VM -ComputerName $Host -Name $VM";
            var parameters = new Dictionary<string, object> { { "Host", hostName }, { "VM", vmName } };
            return await InvokeAndFormat("Start VM", script, parameters);
        }

        public async Task<string> ConnectVmAsync(string hostName, string vmName)
        {
            var script = "Start-Process vmconnect.exe -ArgumentList \"$Host\", \"$VM\"";
            var parameters = new Dictionary<string, object> { { "Host", hostName }, { "VM", vmName } };
            return await InvokeAndFormat("Connect to VM", script, parameters);
        }

        public async Task<string> RestartVmAsync(string hostName, string vmName)
        {
            var script = "Restart-VM -ComputerName $Host -Name $VM -Force";
            var parameters = new Dictionary<string, object> { { "Host", hostName }, { "VM", vmName } };
            return await InvokeAndFormat("Restart VM", script, parameters);
        }

        public async Task<string> SnapshotVmAsync(string hostName, string vmName)
        {
            var script = "Checkpoint-VM -ComputerName $Host -Name $VM -SnapshotName \"AutoCheckpoint-$(Get-Date -Format yyyyMMdd-HHmmss)\"";
            var parameters = new Dictionary<string, object> { { "Host", hostName }, { "VM", vmName } };
            return await InvokeAndFormat("Snapshot VM", script, parameters);
        }

        public async Task<string> RestoreSnapshotVmAsync(string hostName, string vmName)
        {
            var script = @"
                $snap = Get-VMSnapshot -ComputerName $Host -VMName $VM | Sort-Object -Property CreationTime -Descending | Select-Object -First 1
                Restore-VMSnapshot -VMName $VM -ComputerName $Host -Name $snap.Name -Confirm:$false
            ";
            var parameters = new Dictionary<string, object> { { "Host", hostName }, { "VM", vmName } };
            return await InvokeAndFormat("Restore snapshot", script, parameters);
        }

        public async Task<string> ShutdownHostAsync(string hostName)
        {
            var script = "Stop-Computer -ComputerName $Host -Force";
            var parameters = new Dictionary<string, object> { { "Host", hostName } };
            return await InvokeAndFormat("Shutdown Host", script, parameters);
        }

        private async Task<string> InvokeAndFormat(string action, string script, IDictionary<string, object> parameters)
        {
            try
            {
                _logger.LogInformation($"🔧 {action} started...");

                var (output, errors) = await _ps.InvokeAsync(script, parameters);

                if (errors.Length > 0)
                {
                    var errorMsg = $"❌ {action} failed:\n{string.Join("\n", errors)}";
                    _logger.LogError(errorMsg, new Exception("PowerShell execution error"));
                    return errorMsg;
                }

                var result = $"✅ {action} successful.\n{string.Join("\n", output)}";
                _logger.LogInformation(result);
                return result;
            }
            catch (Exception ex)
            {
                var fallbackMsg = $"❌ {action} encountered an unexpected error: {ex.Message}";
                _logger.LogError(fallbackMsg, ex);
                return fallbackMsg;
            }
        }
    }
}
