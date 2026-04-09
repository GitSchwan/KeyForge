using System.Collections.Generic;
using System.Linq;
using KeyForge.Models;

namespace KeyForge.Services;

public interface IThemeRegistry
{
    IEnumerable<Theme> GetAll();
    Theme? GetById(string themeid);
}


public class ThemeRegistry : IThemeRegistry
{
    private readonly List<Theme> _themes = new()
    {
        new Theme("sunset", "#ff7e5f", "#feb47b", 45, "#ff7e5f", "#ffffff"),
        new Theme("ocean", "#2193b0", "#6dd5ed", 90 , "#2193b0", "#ffffff")
    };

    public IEnumerable<Theme> GetAll() => _themes;

    public Theme? GetById(string themeid)
        => _themes.FirstOrDefault(t => t.ThemeId == themeid);  
}