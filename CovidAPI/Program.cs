using CovidAPI;
using System.Security.Cryptography;

/// <summary>
/// The entry point class for the application.
/// </summary>
public class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    public static void Main(string[] args)
    {
        // Generate a secure 256-bit (32-byte) key
        byte[] keyBytes = GenerateRandomKey(32);

        // Convert the byte array to a Base64-encoded string
        string base64Key = Convert.ToBase64String(keyBytes);

        Console.WriteLine("Generated Key: " + base64Key);

        // Add a breakpoint or another Console.WriteLine statement to inspect values
        Console.WriteLine("Application starting...");

        CreateHostBuilder(args).Build().Run();
    }

    /// <summary>
    /// Creates the default host builder for the application.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    /// <returns>The configured host builder.</returns>
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

    /// <summary>
    /// Generates a random key of the specified length using a secure random number generator.
    /// </summary>
    /// <param name="length">The length of the key in bytes.</param>
    /// <returns>The generated random key.</returns>
    private static byte[] GenerateRandomKey(int length)
    {
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            byte[] keyBytes = new byte[length];
            rng.GetBytes(keyBytes);
            return keyBytes;
        }
    }
}
