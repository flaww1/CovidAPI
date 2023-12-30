namespace CovidAPI.Models
{
    /// <summary>
    /// Represents the settings for JSON Web Token (JWT) authentication.
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Gets or sets the secret key used for signing and verifying JWT tokens.
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Gets or sets the issuer of the JWT token.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Gets or sets the audience of the JWT token.
        /// </summary>
        public string Audience { get; set; }
    }
}
