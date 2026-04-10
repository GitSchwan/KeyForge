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

    public Theme? GetCurrentTheme(int userId)
    {
        var userid = _sessionService.CurrentUserId;
        var themeId = _userSettingsService.GetThemeId(userid);
        return _themeRegistry.GetById(themeId);
    }
    
    public void SetTheme(string themeId)
    {
        var userid = _sessionService.CurrentUserId;
        _userSettingsService.SetThemeId(userid, themeId);
    }

    public void initialCreateBackground(string themeid) //WIP
    {
        var background = new LinearGradientBrush
        {
            StartPoint = new Avalonia.RelativePoint(0, 0, Avalonia.RelativeUnit.Relative),
            EndPoint = new Avalonia.RelativePoint(1, 1, Avalonia.RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop { Color = Color.FromRgb(128, 0, 128), Offset = 0 },
                new GradientStop { Color = Color.FromRgb(151, 17, 56), Offset = 1 }
            }
        };
    }
}