using System.Collections.Generic;
using System.Threading.Tasks;

namespace VMManager.Core.Interfaces
{
    public interface IPowerShellService
    {
        Task<PowerShellResult> InvokeAsync(string script, IDictionary<string, object> parameters);
    }

    public class PowerShellResult
    {
        /// <summary>
        /// Whether the script executed successfully.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Lines of output returned from PowerShell.
        /// </summary>
        public List<string> Output { get; set; } = new();

        /// <summary>
        /// Raw error message, if any.
        /// </summary>
        public string? Error { get; set; }
    }
}
