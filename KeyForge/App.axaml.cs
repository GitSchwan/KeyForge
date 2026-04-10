using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using KeyForge.ViewModels;
using KeyForge.Views;
using KeyForge.Services;
using KeyForge.Data;
using Microsoft.EntityFrameworkCore;

namespace KeyForge;

public partial class App : Application
{
    private KeyForgeDbContext? _dbContext;
    public IServiceProvider Services { get; }

    public App(IServiceProvider services)
    {
        Services = services;
    }
    

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();

            _dbContext = new KeyForgeDbContext();
            _dbContext.Database.Migrate();

            var usersession = new SessionService();
            
            ILoginService loginService = new LoginService(_dbContext, usersession);
            ICryptoService cryptoService = new CryptoService(_dbContext,usersession);
            IThemeRegistry themeRegistry = new ThemeRegistry();
            ThemeService themeService = new ThemeService(themeRegistry, usersession, new UserSettingsService(_dbContext));

            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(loginService, cryptoService, _dbContext, usersession, themeRegistry, themeService),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}