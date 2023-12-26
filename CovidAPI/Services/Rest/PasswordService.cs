
using System.Security.Cryptography;
using System.Text;

using BCrypt.Net;

public class PasswordService : IPasswordService
{
    public byte[] HashPassword(string password, out string salt)
    {
        // Generate a new salt in BCrypt format
        salt = BCrypt.Net.BCrypt.GenerateSalt();

        // Hash the password with the generated salt
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

        // Convert the hashed password to a byte array
        return Encoding.UTF8.GetBytes(hashedPassword);
    }

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



    public string GenerateSalt()
    {
        return BCrypt.Net.BCrypt.GenerateSalt();
    }
}
