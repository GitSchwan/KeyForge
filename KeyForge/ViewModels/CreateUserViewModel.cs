using System;
using System.Text;
using CommunityToolkit.Mvvm.Input;
using KeyForge.Services;

namespace KeyForge.ViewModels;

public class CreateUserViewModel : ViewModelBase
{
    private readonly Action _navigateToLogin;
    private readonly ICryptoService _cryptoService;

    private string? _masterPassword;
    public string? MasterPassword
    {
        get => _masterPassword;
        set => SetProperty(ref _masterPassword, value);
    }
    
    private string? _confirmPassword;

    public string? ConfirmPassword
    {
        get => _confirmPassword; 
        set => SetProperty(ref _confirmPassword, value);
    }

    private string? _username;
    public string? Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public IRelayCommand CreateUserCommand { get; }
    public IRelayCommand NavigateToLogin { get; }

    public CreateUserViewModel(ICryptoService cryptoService, Action navigateToLogin)
    {
        _navigateToLogin = navigateToLogin;
        _cryptoService = cryptoService;
        CreateUserCommand = new RelayCommand(CreateUser);
        NavigateToLogin = new RelayCommand(navigateToLogin);
    }

    private void CreateUser()
    {
        var username = _username;
        var password = _masterPassword;
        var confirmPassword = _confirmPassword;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) return;
        
        if (password != confirmPassword) return;
        
        var hashedPassword = _cryptoService.HashMasterPassword(password);
        
        _cryptoService.InsertUserData(username, hashedPassword);
        
        
        _navigateToLogin();
    }
}