using System;
using System.Linq;
using KeyForge.Data;
using KeyForge.Models;

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
    
    /// <summary>
    /// Sets the initial settings for a user
    /// </summary>
    /// <param name="userId"><see cref="int"/></param>
    private void SetInitialSettings(int userId)
    {
        var iniUserSettings = new UserSettings(userId, "sunset");
        _dbContext.UserSettings.Add(iniUserSettings);
        _dbContext.SaveChanges();
    }

    public string GetThemeId(int userid)
    {
        var settings = _dbContext.UserSettings.FirstOrDefault(x => x.UserId == userid);
        return settings?.PreferredThemeId ?? "sunset"; // Fallback to sunset theme
    }

    public void SetThemeId(int userId, string themeId)
    {
        var settings = _dbContext.UserSettings.FirstOrDefault(x => x.UserId == userId); // Is Null :(
        if (settings is null)
        {
            SetInitialSettings(userId);
            settings = _dbContext.UserSettings.FirstOrDefault(x => x.UserId == userId);
        };
        if (settings is null) return;
        settings.PreferredThemeId = themeId;
        _dbContext.SaveChanges();
    }
}