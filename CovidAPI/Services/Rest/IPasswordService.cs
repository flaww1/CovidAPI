public interface IPasswordService
{
    byte[] HashPassword(string password, out byte[] salt);
    bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt);
    string GenerateSalt();

}
