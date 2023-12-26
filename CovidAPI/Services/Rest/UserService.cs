﻿using CovidAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Threading.Tasks;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordService _passwordService;

    public UserService(ApplicationDbContext dbContext, IPasswordService passwordService)
    {
        _dbContext = dbContext;
        _passwordService = passwordService;
    }

    public async Task<User> GetUserByIdAsync(int userId)
    {
        return await _dbContext.Users.FindAsync(userId);
    }

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