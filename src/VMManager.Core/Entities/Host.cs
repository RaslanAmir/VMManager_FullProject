using LiteDB;

namespace VMManager.Core.Entities
{
    /// <summary>
    /// Represents a managed Hyper-V host machine.
    /// </summary>
    public class Host
    {
        /// <summary>
        /// LiteDB primary key. Auto-incremented.
        /// </summary>
        [BsonId]
        public int Id { get; set; }

        /// <summary>
        /// Friendly name or DNS name of the host.
        /// </summary>
        [BsonField("host_name")]
        public string HostName { get; set; } = string.Empty;

        /// <summary>
        /// IP address or network endpoint of the host.
        /// </summary>
        [BsonField("ip_address")]
        public string IpAddress { get; set; } = string.Empty;

        /// <summary>
        /// Compatibility alias for HostName.
        /// </summary>
        [BsonIgnore]
        public string Name
        {
            get => HostName;
            set => HostName = value;
        }
    }
}
