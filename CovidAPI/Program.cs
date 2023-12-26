using CovidAPI;
using System.Security.Cryptography;

public class Program
{
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


    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

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
