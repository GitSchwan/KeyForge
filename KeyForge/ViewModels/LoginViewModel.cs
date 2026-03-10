using System;
using CommunityToolkit.Mvvm.Input;
using KeyForge.Services;

namespace KeyForge.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly ILoginService _loginService;
    private readonly Action _navigateToHome;

    private string? _masterPassword;
    public string? MasterPassword
    {
        get => _masterPassword;
        set => SetProperty(ref _masterPassword, value);
    }

    private string? _username;
    public string? Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public IRelayCommand LoginCommand { get; }

    public LoginViewModel(ILoginService loginService, Action navigateToHome)
    {
        _loginService = loginService;
        _navigateToHome = navigateToHome;
        LoginCommand = new RelayCommand(Login);
    }

    private void Login()
    {
        var username = Username;
        var password = MasterPassword;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return;

        var done = _loginService.Login(username, password);
        if (done)
        {
            _navigateToHome();
        }
    }
}
