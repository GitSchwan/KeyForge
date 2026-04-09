using System;
using System.Linq;
using KeyForge.Data;

namespace KeyForge.Services;

public interface IUserSettingsService
{
    string GetThemeId(int userid);
    void SetThemeId(int userId, string themeId);
}

public class UserSettingsService : IUserSettingsService
{
    public readonly KeyForgeDbContext _dbContext;

    public UserSettingsService(KeyForgeDbContext context)
    {
        _dbContext = context;
    }

    public string GetThemeId(int userid)
    {
        var settings = _dbContext.UserSettings.FirstOrDefault(x => x.UserId == userid);
        
        return settings?.PreferredThemeId ?? "sunset";
    }

    public void SetThemeId(int userId, string themeId)
    {
        var settings = _dbContext.UserSettings.FirstOrDefault(x => x.UserId == userId);
        
        settings?.PreferredThemeId = themeId;
        _dbContext.SaveChanges();
    }
}