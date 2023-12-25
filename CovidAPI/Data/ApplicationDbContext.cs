using CovidAPI.Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public DbSet<CovidData> CovidData { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Specify the new table name for the CovidData entity
        modelBuilder.Entity<CovidData>().ToTable("coviddata_2022");

        // Additional configurations if needed...

        base.OnModelCreating(modelBuilder);
    }
}
