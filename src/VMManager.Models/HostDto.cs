using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VMManager.Models
{
    /// <summary>
    /// Represents a Hyper-V host with associated metadata and virtual machines.
    /// </summary>
    public partial class HostDto : ObservableObject
    {
        /// <summary>
        /// Display name or identifier of the host.
        /// </summary>
        [ObservableProperty]
        private string hostName = string.Empty;

        /// <summary>
        /// IP address or DNS-resolvable name of the host.
        /// </summary>
        [ObservableProperty]
        private string ipAddress = string.Empty;

        /// <summary>
        /// Indicates if this host contains critical VMs.
        /// </summary>
        [ObservableProperty]
        private bool isCritical;

        /// <summary>
        /// Collection of virtual machines associated with this host.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<VMDto> vms = new();

        /// <summary>
        /// Host reachability status.
        /// </summary>
        [ObservableProperty]
        private bool isOnline;

        /// <summary>
        /// Last known successful ping or contact time.
        /// </summary>
        [ObservableProperty]
        private DateTime lastPingTime;

        /// <summary>
        /// Custom tags used for classifying this host.
        /// </summary>
        [ObservableProperty]
        private List<string> tags = new();

        /// <summary>
        /// Creates a deep copy of this <see cref="HostDto"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="HostDto"/> with copied values.</returns>
        public HostDto Clone()
        {
            return new HostDto
            {
                HostName = hostName,
                IpAddress = ipAddress,
                IsCritical = isCritical,
                IsOnline = isOnline,
                LastPingTime = lastPingTime,
                Tags = tags != null ? new List<string>(tags) : new(),
                vms = vms != null
                    ? new ObservableCollection<VMDto>(vms.Select(vm => vm?.Clone() ?? new VMDto()))
                    : new ObservableCollection<VMDto>()
            };
        }
    }
}
