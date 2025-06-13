namespace UsersService.Data;

using Microsoft.EntityFrameworkCore;
using UsersService.Model;

public class dbContext : DbContext
{

    public DbSet<User> Users { get; set; }
    public string DbPath { get; }

    public dbContext()
    {
        DbPath = System.IO.Path.Join("mini_user.db");
    }
    public dbContext(DbContextOptions<dbContext> opts) : base(opts) { }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}
