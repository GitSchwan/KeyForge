using KeyForge.Models;

namespace KeyForge.Services;

public class ThemeService
{
    private readonly IThemeRegistry _themeRegistry;
    private readonly SessionService _sessionService;
    private readonly UserSettingsService _userSettingsService;
    
    
    public ThemeService(IThemeRegistry themeRegistry, SessionService sessionService)
    {
        _themeRegistry = themeRegistry;
        _sessionService = sessionService;
    }

    public Theme? GetCurrentTheme(int userId)
    {
        var userid = _sessionService.CurrentUserId;
        var themeId = _userSettingsService.GetThemeId(userid);
        return _themeRegistry.GetById(themeId);
    }
}