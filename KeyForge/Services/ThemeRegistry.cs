using System.Collections.Generic;
using System.Linq;
using KeyForge.Models;

namespace KeyForge.Services;

public interface IThemeRegistry
{
    /// <summary>
    /// Gets all themes.
    /// </summary>
    IEnumerable<Theme> GetAll();
    
    /// <summary>
    /// Gets a theme by its id.
    /// </summary>
    /// <param name="themeid"><see cref="string"/></param>
    Theme? GetById(string themeid);
}


public class ThemeRegistry : IThemeRegistry
{
    private readonly List<Theme> _themes = new()
    {
        new Theme("sunset", "#ff7e5f", "#feb47b", 45, "#ff7e5f", "#ffffff"),
        new Theme("ocean", "#2193b0", "#6dd5ed", 90 , "#2193b0", "#ffffff"),
        new Theme("midnight", "#0f2027", "#203a43", 90, "#0f2027", "#ffffff"),
        new Theme("deep_space", "#000000", "#434343", 45, "#000000", "#ffffff"),
        new Theme("dark_ocean", "#0c2b3a", "#1c5c7a", 90, "#0c2b3a", "#e0f7ff"),
        new Theme("purple_night", "#20002c", "#cbb4d4", 45, "#20002c", "#ffffff"),
        new Theme("cyberpunk", "#0f0c29", "#302b63", 135, "#ff00cc", "#f8fc03"),
        new Theme("emerald_dark", "#0f3d3e", "#2e8b57", 90, "#0f3d3e", "#eafff5"),
        new Theme("lava", "#1a0000", "#b22222", 45, "#ff4500", "#ffffff"),
        new Theme("steel", "#232526", "#414345", 90, "#232526", "#dfe6e9"),
        new Theme("night_sky", "#141e30", "#243b55", 120, "#141e30", "#ffffff"),
        new Theme("obsidian", "#0b0b0b", "#1a1a1a", 90, "#0b0b0b", "#f5f5f5")
    };

    public IEnumerable<Theme> GetAll() => _themes;

    public Theme? GetById(string themeid)
        => _themes.FirstOrDefault(t => t.ThemeId == themeid);  
}