using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using System.Collections.Generic;
using VMManager.Services.Interfaces;

namespace VMManager.Services
{
    /// <summary>
    /// Provides PowerShell-based implementation for exporting, restoring, and managing Hyper-V VMs remotely.
    /// </summary>
    public class ExportRestoreService : IExportRestoreService
    {
        private readonly ILogger _logger;
        private readonly string[] _criticalVMs;

        public ExportRestoreService(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _criticalVMs = new[] { "DHCP01", "Ydc", "y-bdc" };
        }

        public Task ExportVMAsync(string host, string vm, string destPath) =>
            RunPowerShellAsync(
                host,
                $"Export-VM -Name '{vm}' -Path '{destPath}' -ErrorAction Stop",
                $"Exporting VM '{vm}' to '{destPath}' on '{host}'");

        public Task RestoreVMAsync(string host, string vm, string sourcePath) =>
            RunPowerShellAsync(
                host,
                $"Import-VM -Path '{sourcePath}' -Copy -ErrorAction Stop",
                $"Restoring VM '{vm}' from '{sourcePath}' on '{host}'");

        public Task StartVMAsync(string host, string vm) =>
            RunPowerShellAsync(
                host,
                $"Start-VM -Name '{vm}' -ErrorAction Stop",
                $"Starting VM '{vm}' on '{host}'");

        public async Task StopVMAsync(string host, string vm)
        {
            if (_criticalVMs.Contains(vm, StringComparer.OrdinalIgnoreCase) &&
                await HasNonCriticalVMsRunningAsync(host))
            {
                throw new InvalidOperationException(
                    $"⛔ Cannot stop critical VM '{vm}' on '{host}' while non-critical VMs are still running.");
            }

            var script = $@"
                Stop-VM -Name '{vm}' -Shutdown -ErrorAction SilentlyContinue
                Start-Sleep -Seconds 10
                if ((Get-VM -Name '{vm}').State -eq 'Running') {{
                    Stop-VM -Name '{vm}' -Force -ErrorAction Stop
                }}";

            await RunPowerShellAsync(host, script, $"Stopping VM '{vm}' on '{host}'");
        }

        public Task RestartVMAsync(string host, string vm) =>
            RunPowerShellAsync(
                host,
                $"Restart-VM -Name '{vm}' -Force -ErrorAction Stop",
                $"Restarting VM '{vm}' on '{host}'");

        public Task ConnectConsoleAsync(string host, string vm)
        {
            try
            {
                var psi = new ProcessStartInfo("vmconnect.exe", $"{host} \"{vm}\"")
                {
                    UseShellExecute = true
                };

                using var proc = Process.Start(psi);
                _logger.Info($"Attempting console connection to VM '{vm}' on '{host}'...");

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                var errorMsg = $"❌ Failed to launch console for VM '{vm}' on '{host}': {ex.Message}";
                _logger.Error(errorMsg);
                throw new InvalidOperationException(errorMsg, ex);
            }
        }

        public Task ExportAllVMsOnHostAsync(string host) =>
            ExportAllVMsOnHostAsync(host, Path.Combine("C:\\Backups\\FullExports", $"{host}_{DateTime.Now:yyyyMMdd_HHmmss}"));

        public async Task ExportAllVMsOnHostAsync(string host, string backupPath)
        {
            const string queryScript = "Get-VM | Where-Object { $_.State -eq 'Running' } | Select-Object -ExpandProperty Name";

            using var ps = PowerShell.Create();
            ps.AddCommand("Invoke-Command")
              .AddParameter("ComputerName", host)
              .AddParameter("ScriptBlock", ScriptBlock.Create(queryScript));

            var result = await Task.Run(() => ps.Invoke());

            if (ps.Streams.Error.Count > 0)
            {
                var errors = string.Join(Environment.NewLine, ps.Streams.Error.Select(e => e.ToString()));
                _logger.Error($"❌ PowerShell error while enumerating VMs on {host}:\n{errors}");
                throw new InvalidOperationException(errors);
            }

            var vmNames = result
                .Select(r => r?.ToString())
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToList();

            foreach (var vm in vmNames)
            {
                if (!string.IsNullOrWhiteSpace(vm))
                {
                    var destPath = Path.Combine(backupPath, vm);
                    await ExportVMAsync(host, vm, destPath);
                }
            }

            _logger.Info($"✅ Successfully exported all VMs on host '{host}' to '{backupPath}'.");
        }

        private async Task RunPowerShellAsync(string host, string script, string actionDescription)
        {
            using var ps = PowerShell.Create();
            ps.AddCommand("Invoke-Command")
              .AddParameter("ComputerName", host)
              .AddParameter("ScriptBlock", ScriptBlock.Create(script))
              .AddParameter("ErrorAction", ActionPreference.Stop);

            await Task.Run(() =>
            {
                ps.Invoke();

                if (ps.Streams.Error.Count > 0)
                {
                    var errors = string.Join(Environment.NewLine, ps.Streams.Error.Select(e => e.ToString()));
                    _logger.Error($"❌ PowerShell error during {actionDescription}:\n{errors}");
                    throw new Exception($"PowerShell error during {actionDescription}:\n{errors}");
                }

                _logger.Info($"✅ {actionDescription} completed successfully.");
            });
        }

        private async Task<bool> HasNonCriticalVMsRunningAsync(string host)
        {
            using var ps = PowerShell.Create();
            ps.AddCommand("Invoke-Command")
              .AddParameter("ComputerName", host)
              .AddParameter("ScriptBlock", ScriptBlock.Create(
                  "Get-VM | Where-Object { $_.State -eq 'Running' } | Select-Object -ExpandProperty Name"));

            var results = await Task.Run(() => ps.Invoke());

            var runningVms = results
                .Select(r => r?.ToString())
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToList();

            return runningVms.Any(vm =>
                !_criticalVMs.Contains(vm, StringComparer.OrdinalIgnoreCase));
        }
    }
}
