using RideSharing.Domain.Enums;

namespace RideSharing.Domain.Entities
{
    /// <summary>
    /// Abstract base class for all system users.
    /// Holds identity and credential properties shared across every role.
    /// </summary>
    public abstract class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Username used during login. Must be unique across all users.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Plaintext password stored for simplicity in this console simulation.
        /// A real system would store a salted hash.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        public UserRole Role { get; set; }
    }
}
