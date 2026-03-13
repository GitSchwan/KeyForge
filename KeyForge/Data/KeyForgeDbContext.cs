using KeyForge.Models;
using Microsoft.EntityFrameworkCore;

namespace KeyForge.Data;

public class KeyForgeDbContext : DbContext
{
    public DbSet<VaultEntry> VaultEntries => Set<VaultEntry>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=keyforge.db");
    }
}