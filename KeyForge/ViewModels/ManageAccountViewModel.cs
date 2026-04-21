using System;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeyForge.Data;
using KeyForge.Models;
using KeyForge.Services;
using KeyForge.Views;

namespace KeyForge.ViewModels;

public partial class ManageAccountViewModel : ViewModelBase
{
    public IAsyncRelayCommand DeleteUserCommand { get; } 
    private readonly SessionService _sessionService;
    private readonly ICryptoService _cryptoService;
    private readonly Action _navigateToLogin;

    public ManageAccountViewModel(ICryptoService cryptoService, SessionService sessionService, Action navigateToLogin)
    {
        DeleteUserCommand = new AsyncRelayCommand(DeleteUserAsync);
        _sessionService = sessionService;
        _cryptoService = cryptoService;
        _navigateToLogin = navigateToLogin;
    }
    
    private async Task DeleteUserAsync() //async because of the dialog
    {
        var userid = _sessionService.CurrentUserId;
        var dialog = new ConfirmPopUpView
        {
            DataContext = new ConfirmPopUpViewModel("Wirklich löschen?")
        };

        var owner = App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;

        bool confirmed = owner is not null && await dialog.ShowDialog<bool>(owner);

        if (confirmed)
        {
            _cryptoService.DeleteUserData(userid);
            _sessionService.Logout();
            _navigateToLogin();
        }
    }
    
}
