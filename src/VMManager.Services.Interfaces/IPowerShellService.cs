using System.Collections.Generic;
using System.Threading.Tasks;

namespace VMManager.Services.Interfaces
{
    /// <summary>
    /// Provides asynchronous execution of PowerShell scripts with input parameters.
    /// </summary>
    public interface IPowerShellService
    {
        /// <summary>
        /// Executes a PowerShell script with the specified named parameters.
        /// </summary>
        /// <param name="script">The PowerShell script content to execute.</param>
        /// <param name="parameters">A dictionary of parameter names and values passed to the script.</param>
        /// <returns>
        /// A task that represents the asynchronous execution. 
        /// The result is a tuple containing:
        /// <list type="bullet">
        /// <item><description><c>Output</c>: Lines of output produced by the script.</description></item>
        /// <item><description><c>Errors</c>: Lines of error messages, if any.</description></item>
        /// </list>
        /// </returns>
        Task<(string[] Output, string[] Errors)> InvokeAsync(string script, IDictionary<string, object> parameters);
    }
}
