public interface IPasswordService
{
    byte[] HashPassword(string password, out string salt);
    bool VerifyPassword(string password, byte[] storedHash, string storedSalt);    
    string GenerateSalt();

}
