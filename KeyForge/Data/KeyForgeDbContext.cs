using System;
using System.IO;
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
            var folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "KeyForge");

            Directory.CreateDirectory(folder);

            var dbPath = Path.Combine(folder, "keyforge.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }
}