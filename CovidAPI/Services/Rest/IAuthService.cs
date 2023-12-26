using CovidAPI.Models;

public interface IAuthService
{
    Task<string> AuthenticateAsync(string username, string password);
    Task<string> GenerateTokenAsync(User user);
}
