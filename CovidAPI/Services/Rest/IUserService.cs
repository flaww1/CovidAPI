using CovidAPI.Models;

public interface IUserService
{
    Task<User> GetUserByIdAsync(int userId);
    Task<User> GetUserByUsernameAsync(string username);
    Task<User> CreateUserAsync(User user, string password);
    Task<User> AuthenticateAsync(string username, string password);

}
