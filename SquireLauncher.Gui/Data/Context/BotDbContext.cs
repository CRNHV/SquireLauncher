using Microsoft.EntityFrameworkCore;
using SquireLauncher.Gui.Data.Entities;

namespace SquireLauncher.Gui.Data.Context;

public class BotDbContext : DbContext
{
    public BotDbContext() : base()
    {
        Database.EnsureCreated();
    }

    public DbSet<Bot> Bots { get; set; }
    public DbSet<Profile> Profiles { get; set; }

    public DbSet<Farm> Farms { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source = bots.db");
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
