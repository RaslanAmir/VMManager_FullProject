namespace VMManager.Models
{
    /// <summary>
    /// Defines the type of VM maintenance job that can be scheduled or executed.
    /// </summary>
    public enum JobType
    {
        /// <summary>
        /// Unrecognized or default job type. Used as a fallback when type is undefined.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Represents an export job, typically used to export VM configuration or state.
        /// </summary>
        Export = 1,

        /// <summary>
        /// Represents a restore job, typically used to restore a VM from a previous backup.
        /// </summary>
        Restore = 2
    }
}
