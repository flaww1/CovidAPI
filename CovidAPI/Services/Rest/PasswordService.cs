
using System.Security.Cryptography;
using System.Text;

using BCrypt.Net;

public class PasswordService : IPasswordService
{
    public byte[] HashPassword(string password, out byte[] salt)
    {
        salt = new byte[16]; // Generate a new salt
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(salt);
        }

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, Encoding.UTF8.GetString(salt));
        return Encoding.UTF8.GetBytes(hashedPassword);
    }

    public bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
    {
        string storedPasswordHash = BCrypt.Net.BCrypt.HashPassword(password, Encoding.UTF8.GetString(storedSalt));
        string hashedPasswordString = Encoding.UTF8.GetString(storedHash);

        return storedPasswordHash == hashedPasswordString;
    }
    public string GenerateSalt()
    {
        return BCrypt.Net.BCrypt.GenerateSalt();
    }
}
