using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using VMManager.Services.Interfaces;

namespace VMManager.Services.Infrastructure
{
    /// <summary>
    /// Executes PowerShell scripts with parameters and returns output and error lines.
    /// </summary>
    public sealed class PowerShellService : IPowerShellService
    {
        /// <inheritdoc />
        public async Task<(string[] Output, string[] Errors)> InvokeAsync(string script, IDictionary<string, object> parameters)
        {
            if (string.IsNullOrWhiteSpace(script))
                throw new ArgumentException("Script cannot be null or empty.", nameof(script));

            using var ps = PowerShell.Create();
            ps.AddScript(script);

            // Add parameters if provided
            if (parameters is not null)
            {
                foreach (var (key, value) in parameters)
                {
                    ps.AddParameter(key, value);
                }
            }

            var outputList = new List<string>();
            var errorList = new List<string>();

            try
            {
                var results = await Task.Factory.FromAsync(ps.BeginInvoke(), ps.EndInvoke);

                // Collect standard output
                foreach (var item in results)
                {
                    if (item != null)
                        outputList.Add(item.ToString());
                }

                // Collect errors
                foreach (var error in ps.Streams.Error)
                {
                    errorList.Add(error.ToString());
                }
            }
            catch (Exception ex)
            {
                errorList.Add($"[Exception] {ex.Message}\n{ex.StackTrace}");
            }

            return (outputList.ToArray(), errorList.ToArray());
        }
    }
}
