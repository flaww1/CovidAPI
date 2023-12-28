using CovidAPI.Models;
/// <summary>
/// Represents a service for managing user-related operations.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Retrieves a user by their unique identifier asynchronously.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A task that represents the asynchronous operation and holds the user information.</returns>
    Task<User> GetUserByIdAsync(int userId);

    /// <summary>
    /// Retrieves a user by their username asynchronously.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <returns>A task that represents the asynchronous operation and holds the user information.</returns>
    Task<User> GetUserByUsernameAsync(string username);

    /// <summary>
    /// Creates a new user asynchronously and associates the provided password.
    /// </summary>
    /// <param name="user">The user information to be created.</param>
    /// <param name="password">The password to be associated with the user.</param>
    /// <returns>A task that represents the asynchronous operation and holds the created user information.</returns>
    Task<User> CreateUserAsync(User user, string password);

    /// <summary>
    /// Authenticates a user by verifying the provided username and password asynchronously.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="password">The password to be verified.</param>
    /// <returns>
    /// A task that represents the asynchronous authentication operation.
    /// If authentication is successful, the task holds the authenticated user information; otherwise, it returns null.
    /// </returns>
    Task<User> AuthenticateAsync(string username, string password);
}
