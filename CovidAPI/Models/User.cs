namespace CovidAPI.Models
{
    /// <summary>
    /// Represents a user entity.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the hash of the user's password.
        /// </summary>
        public byte[] PasswordHash { get; set; }

        /// <summary>
        /// Gets or sets the salt used for password hashing.
        /// </summary>
        public string PasswordSalt { get; set; }
    }
}
