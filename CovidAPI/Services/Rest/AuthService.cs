// AuthService.cs
using CovidAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public AuthService(IUserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    public async Task<string> AuthenticateAsync(string username, string password)
    {
        try
        {
            // Authenticate user credentials against your user service
            var user = await _userService.AuthenticateAsync(username, password);

            // If authentication is successful, generate a JWT token
            if (user != null)
            {
                var token = await GenerateTokenAsync(user);
                return token;
            }

            return null; // Authentication failed
        }
        catch (Exception ex)
        {
            // Log or handle the exception as needed
            return null;
        }
    }

    public async Task<string> GenerateTokenAsync(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                // Add other claims as needed
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"],
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return await Task.FromResult(tokenHandler.WriteToken(token));
    }
}
