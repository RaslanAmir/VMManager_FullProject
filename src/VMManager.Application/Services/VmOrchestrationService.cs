using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMManager.Models;
using VMManager.Services.Interfaces;
using VMManager.Common.Logging;

namespace VMManager.Application.Services
{
    /// <summary>
    /// Coordinates a safe shutdown of all virtual machines and the host system itself.
    /// </summary>
    public sealed class VmOrchestrationService
    {
        private readonly IVmControlService _vmControlService;
        private readonly ILoggingService _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="VmOrchestrationService"/> class.
        /// </summary>
        public VmOrchestrationService(IVmControlService vmControlService, ILoggingService logger)
        {
            _vmControlService = vmControlService ?? throw new ArgumentNullException(nameof(vmControlService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gracefully shuts down all VMs on a host, then shuts down the host.
        /// </summary>
        public async Task GracefulShutdownHostAsync(string hostName)
        {
            if (string.IsNullOrWhiteSpace(hostName))
                throw new ArgumentException("Host name cannot be null or empty.", nameof(hostName));

            _logger.LogInformation($"🔄 Initiating graceful shutdown for host: {hostName}");

            var allVms = (await _vmControlService.GetAllVmStatusAsync())
                .Where(vm => vm.HostName.Equals(hostName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var nonCriticalVms = allVms.Where(vm => !vm.IsCritical).ToList();
            var criticalVms = allVms.Where(vm => vm.IsCritical).ToList();

            foreach (var vm in nonCriticalVms)
            {
                try
                {
                    _logger.LogInformation($"🟡 Shutting down non-critical VM: {vm.VMName}");
                    await _vmControlService.ShutdownVmAsync(hostName, vm.VMName);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Failed to shut down VM '{vm.VMName}'", ex);
                }
            }

            await WaitForVmStatusAsync(hostName, nonCriticalVms.Select(v => v.VMName), "Stopped", "Waiting for non-critical VMs...");

            foreach (var vm in criticalVms)
            {
                try
                {
                    _logger.LogInformation($"🔴 Shutting down critical VM: {vm.VMName}");
                    await _vmControlService.ShutdownVmAsync(hostName, vm.VMName);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Failed to shut down critical VM '{vm.VMName}'", ex);
                }
            }

            await WaitForVmStatusAsync(hostName, allVms.Select(v => v.VMName), "Stopped", "Waiting for all VMs to stop...");

            _logger.LogInformation($"✅ All VMs stopped. Proceeding to shut down host '{hostName}'...");
            await _vmControlService.ShutdownHostAsync(hostName);
        }

        /// <summary>
        /// Waits until all specified VMs on a host reach a desired status.
        /// </summary>
        private async Task WaitForVmStatusAsync(string hostName, IEnumerable<string> vmNames, string expectedStatus, string waitMessage)
        {
            const int retryDelayMs = 5000;
            const int maxRetries = 60;

            int attempt = 0;

            while (attempt++ < maxRetries)
            {
                var currentStatuses = (await _vmControlService.GetAllVmStatusAsync())
                    .Where(vm => vm.HostName.Equals(hostName, StringComparison.OrdinalIgnoreCase) &&
                                 vmNames.Contains(vm.VMName))
                    .ToList();

                if (currentStatuses.All(vm => string.Equals(vm.Status, expectedStatus, StringComparison.OrdinalIgnoreCase)))
                    return;

                _logger.LogInformation($"{waitMessage} (retry {attempt}/{maxRetries})");
                await Task.Delay(retryDelayMs);
            }

            throw new TimeoutException($"Timeout waiting for VMs on '{hostName}' to reach '{expectedStatus}' state.");
        }
    }
}
