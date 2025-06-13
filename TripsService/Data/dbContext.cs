namespace TripServices.Data;

using Microsoft.EntityFrameworkCore;
using TripServices.Model;

public class dbContext : DbContext
{

    public DbSet<Trip> Trips { get; set; }
    public string DbPath { get; }

    public dbContext()
    {
        DbPath = System.IO.Path.Join("mini_trip.db");
    }
    public dbContext(DbContextOptions<dbContext> opts) : base(opts) { }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}
