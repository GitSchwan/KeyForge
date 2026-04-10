using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeyForge.Models;
using KeyForge.Services;

namespace KeyForge.ViewModels;

public partial class VisualSettingsViewModel : ViewModelBase
{
    private readonly ThemeService _themeService;
    private readonly IThemeRegistry _themeRegistry;

    public IEnumerable<Theme> Themes { get; }

    public VisualSettingsViewModel(IThemeRegistry themeRegistry, ThemeService themeService)
    {
        _themeRegistry = themeRegistry;
        _themeService = themeService;

        Themes = new ObservableCollection<Theme>(_themeRegistry.GetAll());

        foreach (var theme in Themes)
        {
            var list = theme.ThemeId;
        }

    }

    [ObservableProperty] private Theme? _selectedTheme;

    partial void OnSelectedThemeChanged(Theme? value)
    {
        if (value is not null)
        {
            Console.WriteLine("Theme Changed");
            Console.WriteLine(value.ThemeId);
            _themeService.SetTheme(value.ThemeId);
        }
    }
}