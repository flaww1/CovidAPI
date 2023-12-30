using System.Security.Cryptography;
using System.Text;

using BCrypt.Net;

/// <summary>
/// Service for hashing, verifying passwords, and generating salts using BCrypt.
/// </summary>
public class PasswordService : IPasswordService
{
    /// <summary>
    /// Hashes the provided password and generates a salt using BCrypt.
    /// </summary>
    /// <param name="password">The password to be hashed.</param>
    /// <param name="salt">The generated salt in BCrypt format.</param>
    /// <returns>The hashed password as a byte array.</returns>
    public byte[] HashPassword(string password, out string salt)
    {
        // Generate a new salt in BCrypt format
        salt = BCrypt.Net.BCrypt.GenerateSalt();

        // Hash the password with the generated salt
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

        // Convert the hashed password to a byte array
        return Encoding.UTF8.GetBytes(hashedPassword);
    }

    /// <summary>
    /// Verifies the provided password against the stored hash and salt using BCrypt.
    /// </summary>
    /// <param name="password">The password to be verified.</param>
    /// <param name="storedHash">The stored hashed password as a byte array.</param>
    /// <param name="storedSalt">The stored salt in BCrypt format.</param>
    /// <returns>True if the password is verified; otherwise, false.</returns>
    public bool VerifyPassword(string password, byte[] storedHash, string storedSalt)
    {
        try
        {
            // Ensure the stored salt is in BCrypt format
            if (!storedSalt.StartsWith("$2a$") && !storedSalt.StartsWith("$2b$"))
            {
                throw new SaltParseException("Invalid salt version");
            }

            // Convert the hashed password to a string for BCrypt Verify method
            string hashedPasswordString = BCrypt.Net.BCrypt.HashPassword(password, storedSalt, false, BCrypt.Net.HashType.SHA256);

            // Verify the password using BCrypt
            return BCrypt.Net.BCrypt.Verify(password, hashedPasswordString);
        }
        catch (SaltParseException ex)
        {
            // Log or handle the exception as needed
            Console.WriteLine($"Salt parsing error: {ex}");
            return false;
        }
    }

    /// <summary>
    /// Generates a new salt using BCrypt.
    /// </summary>
    /// <returns>The generated salt in BCrypt format.</returns>
    public string GenerateSalt()
    {
        return BCrypt.Net.BCrypt.GenerateSalt();
    }
}
