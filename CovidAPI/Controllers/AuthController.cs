using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using CovidAPI.Models;
using Microsoft.Extensions.Configuration;
using CovidAPI.Services.Rest;

/// <summary>
/// Controller for authentication-related operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="authService">The authentication service.</param>
    /// <param name="userService">The user service.</param>
    public AuthController(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    /// <summary>
    /// Logs in a user.
    /// </summary>
    /// <param name="request">The login request.</param>
    /// <returns>Returns a JWT token if login is successful.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
    {
        try
        {
            // Validate request data, e.g., check if the username is provided
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { Message = "Invalid username or password." });
            }

            // Check if the user exists
            var user = await _userService.GetUserByUsernameAsync(request.Username);
            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid username or password." });
            }

            // Authenticate user credentials against your user service
            var token = await _authService.AuthenticateAsync(request.Username, request.Password);

            if (token == null)
            {
                return Unauthorized(new { Message = "Invalid username or password." });
            }

            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in AuthController.Login: {ex}");
            return StatusCode(500, "An error occurred during login.");
        }
    }


    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="request">The registration request.</param>
    /// <returns>Returns a success message and JWT token if registration is successful.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
    {
        try
        {
            // Validate request data, e.g., check if the username is unique
            var existingUser = await _userService.GetUserByUsernameAsync(request.Username);
            if (existingUser != null)
            {
                return BadRequest(new { Message = "Username is already taken." });
            }

            // Validate password complexity (you can customize this according to your requirements)
            if (string.IsNullOrEmpty(request.Password) || request.Password.Length < 6)
            {
                return BadRequest(new { Message = "Password must be at least 6 characters long." });
            }

            // Create a new User object
            var newUser = new User
            {
                Username = request.Username,
            };

            // Call the authentication service to create the user
            var createdUser = await _userService.CreateUserAsync(newUser, request.Password);

            // Generate a JWT token for the newly registered user
            var token = await _authService.GenerateTokenAsync(createdUser);

            // You might want to return some information about the created user or just a success message
            return Ok(new { Message = "Registration successful", UserId = createdUser.Id, Token = token });
        }
        catch (Exception ex)
        {
            // Log or handle the exception as needed
            Console.WriteLine($"Exception in AuthController.Register: {ex}");
            return StatusCode(500, "An error occurred during registration.");
        }
    }

    /// <summary>
    /// Logs out the currently authenticated user.
    /// </summary>
    /// <returns>Returns a success message upon successful logout.</returns>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {

            return Ok(new { Message = "Logout successful" });
        }
        catch (Exception ex)
        {
            // Log or handle the exception as needed
            Console.WriteLine($"Exception in AuthController.Logout: {ex}");
            return StatusCode(500, "An error occurred during logout.");
        }
    }

}
