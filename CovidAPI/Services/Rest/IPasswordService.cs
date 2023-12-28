/// <summary>
/// Represents a service for hashing, verifying passwords, and generating password salts.
/// </summary>
public interface IPasswordService
{
    /// <summary>
    /// Hashes the provided password and generates a salt.
    /// </summary>
    /// <param name="password">The password to be hashed.</param>
    /// <param name="salt">The generated salt used in the hashing process.</param>
    /// <returns>The hashed password as a byte array.</returns>
    byte[] HashPassword(string password, out string salt);

    /// <summary>
    /// Verifies if the provided password matches the stored hash and salt.
    /// </summary>
    /// <param name="password">The password to be verified.</param>
    /// <param name="storedHash">The stored hashed password.</param>
    /// <param name="storedSalt">The stored salt used in the hashing process.</param>
    /// <returns>True if the password is verified; otherwise, false.</returns>
    bool VerifyPassword(string password, byte[] storedHash, string storedSalt);

    /// <summary>
    /// Generates a random salt for password hashing.
    /// </summary>
    /// <returns>The generated salt as a string.</returns>
    string GenerateSalt();
}
