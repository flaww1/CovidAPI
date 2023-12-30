namespace CovidAPI.Models
{
    /// <summary>
    /// Represents the data transfer object (DTO) for user registration requests.
    /// </summary>
    public class RegisterRequestDTO
    {
        /// <summary>
        /// Gets or sets the username for the registration request.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password for the registration request.
        /// </summary>
        public string Password { get; set; }
    }
}
