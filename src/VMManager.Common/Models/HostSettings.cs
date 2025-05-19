using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VMManager.Common.Models
{
    /// <summary>
    /// Represents persisted configuration for a Hyper-V host, including hostname,
    /// the list of virtual machines it manages, and its criticality status.
    /// Used in scheduling, export/restore logic, and settings serialization.
    /// </summary>
    public class HostSettings
    {
        /// <summary>
        /// Gets or sets the unique identifier or network-resolvable name of the host.
        /// This can be a NetBIOS name, DNS name, or IP address.
        /// </summary>
        [Required(ErrorMessage = "Host name is required.")]
        public string HostName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the collection of virtual machine names hosted on this Hyper-V host.
        /// Used to associate scheduled backup or orchestration logic to specific VMs.
        /// </summary>
        public List<string> VirtualMachines { get; set; } = new();

        /// <summary>
        /// Indicates whether this host is marked as critical in orchestration logic.
        /// Critical hosts may have delayed shutdown sequences or restricted operations.
        /// </summary>
        public bool IsCritical { get; set; } = false;
    }
}
