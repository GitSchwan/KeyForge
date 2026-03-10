using System;
using CommunityToolkit.Mvvm.Input;
using KeyForge.Services;

namespace KeyForge.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ILoginService _loginService;
    
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

    public MainWindowViewModel(ILoginService loginService)
    {
        _loginService = loginService;
        LoginCommand = new RelayCommand(Login);
    }

    private void Login()
    {
        var password = MasterPassword;
        var username = Username;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return;

        var done = _loginService.Login(username, password);
        if(done)
        {
            Console.WriteLine("Login successful");
        }
        else
        {
            Console.WriteLine("Login failed");
        }
    }
}
