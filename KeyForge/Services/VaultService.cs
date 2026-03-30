using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using KeyForge.Data;
using KeyForge.Models;
using Microsoft.EntityFrameworkCore;

namespace KeyForge.Services;

public interface IVaultService
{
    List<VaultEntry> GetVaultEntriesForUser(int userId);
    
    string getUserSpecifcWebsitePassword(int id, int userId);
}

public class VaultService : IVaultService
{
    private readonly KeyForgeDbContext _dbContext;

    public VaultService(KeyForgeDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    /// <summary>
    /// Gets all vault entries for a user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public List<VaultEntry> GetVaultEntriesForUser(int userId)
    {
        return _dbContext.VaultEntries
            .AsNoTracking()
            .Where(v => v.UserId == userId)
            .ToList();
    }
    
    /// <summary>
    /// Gets the password for a specific website
    /// </summary>
    /// <param name="id"><see cref="int"/></param>
    /// <param name="userId"><see cref="int"/></param>
    /// <returns></returns>
    public string getUserSpecifcWebsitePassword(int id, int userId)
    {
        try
        {
            return _dbContext.VaultEntries.AsNoTracking().Where(v => v.Id == id && v.UserId == userId).FirstOrDefault()?.Password ?? string.Empty; 
        }
        catch
        {
            return string.Empty;
        }
            
    }

    public static IClipboard Get()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: { } window }) {
            return window.Clipboard!;
        }

        return null!;
    }
    
}