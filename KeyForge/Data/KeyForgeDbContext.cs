using KeyForge.Models;
using Microsoft.EntityFrameworkCore;

namespace KeyForge.Data;

public class KeyForgeDbContext : DbContext
{
    public DbSet<VaultEntry> VaultEntries => Set<VaultEntry>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserSettings> UserSettings => Set<UserSettings>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=keyforge.db");
        }
    }
}