using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace VMManager.Models
{
    /// <summary>
    /// Represents metadata about a Hyper-V host, including IP address, criticality,
    /// and the list of associated virtual machines. This model is used for configuration
    /// persistence, orchestration logic, and job scheduling.
    /// </summary>
    public class HostInfo
    {
        /// <summary>
        /// Gets or sets the name of the host. This must be a valid and resolvable
        /// network identifier (DNS or NetBIOS).
        /// </summary>
        [Required(ErrorMessage = "Host name is required.")]
        [JsonProperty("hostName")]
        public string HostName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the IP address of the host. Optional but used for RDP or UI identification.
        /// </summary>
        [JsonProperty("ipAddress")]
        public string IpAddress { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the host contains critical VMs that must be shut down last.
        /// </summary>
        [JsonProperty("isCritical")]
        public bool IsCritical { get; set; }

        /// <summary>
        /// Gets or sets the list of virtual machine names hosted on this machine.
        /// </summary>
        [JsonProperty("virtualMachines")]
        public List<string> VirtualMachines { get; set; } = new();
    }
}
