using System.Collections.Generic;
using System.Linq;
using KeyForge.Data;
using KeyForge.Models;
using Microsoft.EntityFrameworkCore;

namespace KeyForge.Services;

public interface IVaultService
{
    List<VaultEntry> GetVaultEntriesForUser(int userId);
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
}