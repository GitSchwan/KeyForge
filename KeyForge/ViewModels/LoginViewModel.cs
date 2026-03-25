using System;
using CommunityToolkit.Mvvm.Input;
using KeyForge.Services;

namespace KeyForge.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly ILoginService _loginService;
    private readonly Action _navigateToHome;
    private readonly Action _navigateToCreateUser;

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
    public IRelayCommand NavCreateUserCommand { get; }

    public LoginViewModel(ILoginService loginService, Action navigateToHome, Action navigateToCreateUser)
    {
        _loginService = loginService;
        _navigateToHome = navigateToHome;
        _navigateToCreateUser = navigateToCreateUser;
        LoginCommand = new RelayCommand(Login);
        NavCreateUserCommand = new RelayCommand(navigateToCreateUser);
    }

    private void Login()
    {
        var username = _username;
        var password = _masterPassword;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) return;

        var done = _loginService.Login(username, password);
        if (done)
        {
            _navigateToHome();
        }
    }
}
