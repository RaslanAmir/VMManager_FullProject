using CommunityToolkit.Mvvm.ComponentModel;

namespace VMManager.Models
{
    /// <summary>
    /// Represents a virtual machine (VM) managed within a Hyper-V host environment.
    /// </summary>
    public partial class VMDto : ObservableObject
    {
        /// <summary>
        /// The name of the host machine where the virtual machine is hosted.
        /// </summary>
        [ObservableProperty]
        private string hostName = string.Empty;

        /// <summary>
        /// The technical identifier of the virtual machine.
        /// </summary>
        [ObservableProperty]
        private string vmName = string.Empty;

        /// <summary>
        /// The current operational status of the VM (e.g., Running, Stopped, Saved).
        /// </summary>
        [ObservableProperty]
        private string status = string.Empty;

        /// <summary>
        /// Indicates whether the VM is designated as critical.
        /// Used to orchestrate shutdown priorities.
        /// </summary>
        [ObservableProperty]
        private bool isCritical;

        /// <summary>
        /// Clones the current instance of the virtual machine DTO.
        /// </summary>
        /// <returns>A deep copy of the current VMDto instance.</returns>
        public VMDto Clone()
        {
            return new VMDto
            {
                HostName = this.HostName,
                VmName = this.VmName,
                Status = this.Status,
                IsCritical = this.IsCritical
            };
        }

        /// <summary>
        /// Legacy compatibility alias for the VM name (PascalCase for binding/backwards compatibility).
        /// </summary>
        public string VMName
        {
            get => VmName;
            set => VmName = value;
        }
    }
}
