namespace VMManager.Common.Models
{
    /// <summary>
    /// Stores Remote Desktop Protocol (RDP) credential information for a host.
    /// Used for automated remote desktop connection launches.
    /// </summary>
    public class RdpSettings
    {
        /// <summary>
        /// Gets or sets the username used when initiating RDP sessions.
        /// Typically in the format <c>DOMAIN\\Username</c> or just <c>Username</c>.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the encrypted password associated with the RDP user.
        /// The password should be securely stored and decrypted only at runtime.
        /// </summary>
        public string EncryptedPassword { get; set; } = string.Empty;
    }
}
