// AuthService.cs
using CovidAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Service responsible for user authentication and JWT token generation.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly IPasswordService _passwordService;


    /// <summary>
    /// Initializes a new instance of the <see cref="AuthService"/> class.
    /// </summary>
    /// <param name="userService">The user service.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="passwordService">The password service.</param>
    public AuthService(IUserService userService, IConfiguration configuration, IPasswordService passwordService)
    {
        _userService = userService;
        _configuration = configuration;
        _passwordService = passwordService;

    }

    /// <summary>
    /// Authenticates a user based on the provided username and password.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns>The JWT token if authentication is successful; otherwise, null.</returns>
    public async Task<string> AuthenticateAsync(string username, string password)
    {
        try
        {
            var user = await _userService.GetUserByUsernameAsync(username);

            if (user != null)
            {
                Console.WriteLine($"User is not null. Username: {user.Username}, Password: {password}");

                if (_passwordService.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
                {
                    // Authentication successful
                    Console.WriteLine("Authentication successful");
                    return await GenerateTokenAsync(user);
                }
                else
                {
                    Console.WriteLine("Authentication failed. Password verification failed.");
                }
            }
            else
            {
                Console.WriteLine("User is null. Authentication failed.");
            }

            return null; // Authentication failed
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in AuthService.AuthenticateAsync: {ex}");
            return null;
        }
    }

    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="user">The user for whom the token should be generated.</param>
    /// <returns>The generated JWT token.</returns>
    public async Task<string> GenerateTokenAsync(User user)
    {
        try
        {
            Console.WriteLine($"Generating token for user: {user.Username}");

            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = _configuration["JwtSettings:Secret"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];

            if (string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                throw new InvalidOperationException("JWT configuration values are missing or invalid.");
            }

            // Convert the Base64-encoded key back to a byte array
            var keyBytes = Convert.FromBase64String(secret);

            // Ensure the key is exactly 32 bytes (256 bits)
            if (keyBytes.Length != 32)
            {
                throw new InvalidOperationException("JWT secret key must be exactly 256 bits (32 bytes).");
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, user.Username),
                    // Add other claims as needed
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature),
                Issuer = issuer,
                Audience = audience,
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            if (token == null)
            {
                throw new InvalidOperationException("Failed to generate a JWT token.");
            }

            return await Task.FromResult(tokenHandler.WriteToken(token));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in AuthService.GenerateTokenAsync: {ex}");
            return null;
        }
    }


}
