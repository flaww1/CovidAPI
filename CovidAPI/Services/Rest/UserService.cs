using CovidAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Service for user-related operations, including authentication and user retrieval.
/// </summary>
public class UserService : IUserService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordService _passwordService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserService"/> class.
    /// </summary>
    /// <param name="dbContext">The application database context.</param>
    /// <param name="passwordService">The password service for hashing and verification.</param>
    public UserService(ApplicationDbContext dbContext, IPasswordService passwordService)
    {
        _dbContext = dbContext;
        _passwordService = passwordService;
    }

    /// <summary>
    /// Gets a user by their unique identifier.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <returns>A task representing the asynchronous operation, returning the user if found; otherwise, null.</returns>
    public async Task<User> GetUserByIdAsync(int userId)
    {
        return await _dbContext.Users.FindAsync(userId);
    }

    /// <summary>
    /// Gets a user by their username.
    /// </summary>
    /// <param name="username">The username of the user to retrieve.</param>
    /// <returns>A task representing the asynchronous operation, returning the user if found; otherwise, null.</returns>
    public async Task<User> GetUserByUsernameAsync(string username)
    {
        try
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user != null)
            {
                Console.WriteLine($"User found for username: {username}");
            }
            else
            {
                Console.WriteLine($"User not found for username: {username}");
            }

            return user;
        }
        catch (Exception ex)
        {
            // Log or handle the exception as needed
            Console.WriteLine($"Exception in UserService.GetUserByUsernameAsync: {ex}");
            return null;
        }
    }

    /// <summary>
    /// Creates a new user with the provided information, including password hashing and salting.
    /// </summary>
    /// <param name="user">The user information.</param>
    /// <param name="password">The plaintext password to be hashed.</param>
    /// <returns>A task representing the asynchronous operation, returning the created user.</returns>
    public async Task<User> CreateUserAsync(User user, string password)
    {
        // Hash the password and generate a salt
        string salt;
        user.PasswordHash = _passwordService.HashPassword(password, out salt);
        user.PasswordSalt = salt;

        // Add the user to the database
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return user;
    }

    /// <summary>
    /// Authenticates a user based on the provided username and password.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="password">The password to be verified.</param>
    /// <returns>A task representing the asynchronous operation, returning the authenticated user if successful; otherwise, null.</returns>
    public async Task<User> AuthenticateAsync(string username, string password)
    {
        var user = await GetUserByUsernameAsync(username);

        if (user != null && _passwordService.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
        {
            // Authentication successful
            return user;
        }

        return null; // Authentication failed
    }
}
