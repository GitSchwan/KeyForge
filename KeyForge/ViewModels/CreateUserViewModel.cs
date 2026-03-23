using System;
using CommunityToolkit.Mvvm.Input;
using KeyForge.Services;

namespace KeyForge.ViewModels;

public class CreateUserViewModel : ViewModelBase
{
    private readonly Action _navigateToLogin;

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

    public IRelayCommand CreateUserCommand { get; }

    public CreateUserViewModel(Action navigateToHome)
    {
        _navigateToLogin = navigateToHome;
        CreateUserCommand = new RelayCommand(CreateUser);
    }

    private void CreateUser()
    {
        var username = _username;
        var password = _masterPassword;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) return;
        
        _navigateToLogin();
    }
}