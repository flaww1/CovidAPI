using CovidAPI.Models;
/// <summary>
/// Defines authentication service methods.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticates a user based on the provided username and password.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an authentication token.</returns>
    Task<string> AuthenticateAsync(string username, string password);

    /// <summary>
    /// Generates an authentication token for the specified user.
    /// </summary>
    /// <param name="user">The user for whom the token should be generated.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an authentication token.</returns>
    Task<string> GenerateTokenAsync(User user);
}
