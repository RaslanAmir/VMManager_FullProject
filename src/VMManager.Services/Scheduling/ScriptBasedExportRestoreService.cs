using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VMManager.Services.Interfaces;

namespace VMManager.Services.Scheduling
{
    /// <summary>
    /// PowerShell-based implementation of <see cref="IExportRestoreService"/>.
    /// Handles VM lifecycle operations by invoking native PowerShell scripts.
    /// </summary>
    public sealed class ScriptBasedExportRestoreService : IExportRestoreService
    {
        private readonly IPowerShellService _ps;
        private readonly ILogger _logger;

        public ScriptBasedExportRestoreService(IPowerShellService ps, ILogger logger)
        {
            _ps = ps ?? throw new ArgumentNullException(nameof(ps));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task ExportVMAsync(string hostName, string vmName, string outputPath)
        {
            ValidateHostAndVm(hostName, vmName);
            Directory.CreateDirectory(outputPath);

            _logger.Info($"Starting export of VM '{vmName}' on host '{hostName}' to '{outputPath}'");

            var script = @"
                Param($Host, $VM, $Out)
                Export-VM -HostName $Host -Name $VM -Path $Out
            ";

            var parameters = new Dictionary<string, object>
            {
                { "Host", hostName },
                { "VM", vmName },
                { "Out", outputPath }
            };

            await _ps.InvokeAsync(script, parameters);
        }

        /// <inheritdoc />
        public async Task RestoreVMAsync(string hostName, string vmName, string sourcePath)
        {
            if (!Directory.Exists(sourcePath))
                throw new DirectoryNotFoundException($"Backup path not found: {sourcePath}");

            ValidateHostAndVm(hostName, vmName);

            _logger.Info($"Restoring VM '{vmName}' on host '{hostName}' from '{sourcePath}'");

            var script = @"
                Param($P, $Host, $VM)
                Import-VM -Path $P -HostName $Host -Name $VM
            ";

            var parameters = new Dictionary<string, object>
            {
                { "P", sourcePath },
                { "Host", hostName },
                { "VM", vmName }
            };

            await _ps.InvokeAsync(script, parameters);
        }

        /// <inheritdoc />
        public async Task StartVMAsync(string host, string vm)
        {
            ValidateHostAndVm(host, vm);
            _logger.Info($"Starting VM '{vm}' on host '{host}'");

            var script = "Start-VM -ComputerName $Host -Name $VM";
            var parameters = new Dictionary<string, object>
            {
                { "Host", host },
                { "VM", vm }
            };

            await _ps.InvokeAsync(script, parameters);
        }

        /// <inheritdoc />
        public async Task StopVMAsync(string host, string vm)
        {
            ValidateHostAndVm(host, vm);
            _logger.Info($"Stopping VM '{vm}' on host '{host}'");

            var script = "Stop-VM -ComputerName $Host -Name $VM -Force";
            var parameters = new Dictionary<string, object>
            {
                { "Host", host },
                { "VM", vm }
            };

            await _ps.InvokeAsync(script, parameters);
        }

        /// <inheritdoc />
        public async Task RestartVMAsync(string host, string vm)
        {
            ValidateHostAndVm(host, vm);
            _logger.Info($"Restarting VM '{vm}' on host '{host}'");

            var script = "Restart-VM -ComputerName $Host -Name $VM -Force";
            var parameters = new Dictionary<string, object>
            {
                { "Host", host },
                { "VM", vm }
            };

            await _ps.InvokeAsync(script, parameters);
        }

        /// <inheritdoc />
        public async Task ConnectConsoleAsync(string host, string vm)
        {
            ValidateHostAndVm(host, vm);
            _logger.Info($"Opening VMConnect for '{vm}' on host '{host}'");

            var script = @"
                Param($Host, $VM)
                vmconnect.exe $Host $VM
            ";

            var parameters = new Dictionary<string, object>
            {
                { "Host", host },
                { "VM", vm }
            };

            await _ps.InvokeAsync(script, parameters);
        }

        /// <inheritdoc />
        public async Task ExportAllVMsOnHostAsync(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
                throw new ArgumentException("Host is required.", nameof(host));

            string exportBaseDir = Path.Combine(
                AppContext.BaseDirectory,
                "Backups",
                "FullExports",
                $"{host}_{DateTime.Now:yyyyMMdd_HHmmss}"
            );

            await ExportAllVMsOnHostAsync(host, exportBaseDir);
        }

        /// <inheritdoc />
        public async Task ExportAllVMsOnHostAsync(string host, string backupPath)
        {
            if (string.IsNullOrWhiteSpace(host))
                throw new ArgumentException("Host is required.", nameof(host));

            if (string.IsNullOrWhiteSpace(backupPath))
                throw new ArgumentException("Backup path is required.", nameof(backupPath));

            Directory.CreateDirectory(backupPath);

            _logger.Info($"Exporting all VMs on host '{host}' to '{backupPath}'");

            var script = @"
                Param($Host)
                Get-VM -ComputerName $Host | Select-Object -ExpandProperty Name
            ";

            var parameters = new Dictionary<string, object> { { "Host", host } };
            var result = await _ps.InvokeAsync(script, parameters);

            if (result.Output == null || result.Output.Length == 0)
                throw new Exception($"No VMs found on host '{host}'");

            foreach (string vmName in result.Output)
            {
                if (!string.IsNullOrWhiteSpace(vmName))
                {
                    var vmExportPath = Path.Combine(backupPath, vmName);
                    Directory.CreateDirectory(vmExportPath);
                    await ExportVMAsync(host, vmName, vmExportPath);
                }
            }
        }

        /// <summary>
        /// Validates that both host and VM names are provided.
        /// </summary>
        private static void ValidateHostAndVm(string host, string vm)
        {
            if (string.IsNullOrWhiteSpace(host))
                throw new ArgumentException("Host name is required.", nameof(host));

            if (string.IsNullOrWhiteSpace(vm))
                throw new ArgumentException("VM name is required.", nameof(vm));
        }
    }
}
