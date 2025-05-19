using System;
using System.Threading.Tasks;
using VMManager.Common.Logging;
using VMManager.Core.Interfaces;

namespace VMManager.Infrastructure.PowerShell
{
    /// <summary>
    /// Provides a secure abstraction to run PowerShell scripts using IPowerShellService.
    /// </summary>
    public class SecurePowerShellService
    {
        private readonly ILoggingService _logger;
        private readonly IPowerShellService _psService;

        public SecurePowerShellService(ILoggingService logger, IPowerShellService psService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _psService = psService ?? throw new ArgumentNullException(nameof(psService));
        }

        /// <summary>
        /// Runs a PowerShell script securely and returns combined output as string.
        /// </summary>
        public async Task<string> RunScriptAsync(string script)
        {
            try
            {
                var result = await _psService.InvokeAsync(script, new System.Collections.Generic.Dictionary<string, object>());

                if (!result.IsSuccess)
                {
                    _logger.LogError($"PowerShell error: {result.Error}", null);
                    throw new InvalidOperationException($"PowerShell script failed: {result.Error}");
                }

                return string.Join(Environment.NewLine, result.Output);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unhandled exception in SecurePowerShellService.RunScriptAsync", ex);
                throw;
            }
        }
    }
}
