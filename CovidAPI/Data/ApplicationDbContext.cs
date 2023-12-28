using CovidAPI.Models;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Represents the database context for the application, providing access to data entities.
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Gets or sets the DbSet for CovidData entities in the database.
    /// </summary>
    public DbSet<CovidData> CovidData { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for User entities in the database.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
    /// </summary>
    /// <param name="options">The options for configuring the DbContext.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Configures the model for the database context.
    /// </summary>
    /// <param name="modelBuilder">The builder used to construct the model for the DbContext.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Specify the new table name for the CovidData entity
        modelBuilder.Entity<CovidData>().ToTable("coviddata_2022");

        // Specify the new table name for the User entity
        modelBuilder.Entity<User>().ToTable("users");

        // Call the base class method to complete model configuration
        base.OnModelCreating(modelBuilder);
    }
}
