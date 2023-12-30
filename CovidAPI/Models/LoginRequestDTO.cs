namespace CovidAPI.Models
{
    /// <summary>
    /// Represents the data transfer object (DTO) for user login requests.
    /// </summary>
    public class LoginRequestDTO
    {
        /// <summary>
        /// Gets or sets the username for the login request.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password for the login request.
        /// </summary>
        public string Password { get; set; }
    }
}
