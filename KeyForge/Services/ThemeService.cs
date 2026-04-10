using System;
using Avalonia.Media;
using KeyForge.Models;

namespace KeyForge.Services;

public class ThemeService
{
    private readonly IThemeRegistry _themeRegistry;
    private readonly SessionService _sessionService;
    private readonly UserSettingsService _userSettingsService;
    
    
    public ThemeService(IThemeRegistry themeRegistry, SessionService sessionService, UserSettingsService userSettingsService)
    {
        _themeRegistry = themeRegistry;
        _sessionService = sessionService;
        _userSettingsService = userSettingsService;
    }

    public Theme? GetCurrentTheme()
    {
        var userid = _sessionService.CurrentUserId;
        var themeId = _userSettingsService.GetThemeId(userid);
        return _themeRegistry.GetById(themeId);
    }
    
    public event Action<Theme?> ThemeChanged;
    
    public void SetTheme(string themeId)
    {
        var userid = _sessionService.CurrentUserId;
        _userSettingsService.SetThemeId(userid, themeId);
        ThemeChanged?.Invoke(GetCurrentTheme());
    }
    
    
}