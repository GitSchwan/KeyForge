using System;
using CommunityToolkit.Mvvm.Input;
using KeyForge.Services;

namespace KeyForge.ViewModels;

public class AddViewModel : ViewModelBase
{
    private readonly Action _navigateToHome;
    private readonly ICryptoService _cryptoService;
    private readonly SessionService _sessionService;

    
    private string? _addViewWebsite;
    public string? AddViewWebsite
    {
        get => _addViewWebsite;
        set => SetProperty(ref _addViewWebsite, value); 
    }
    
    private string? _addViewUsername;
    
    public string? AddViewUsername
    {
        get => _addViewUsername;
        set => SetProperty(ref _addViewUsername, value); 
    }
    
    private string? _addViewPassword;
    
    public string? AddViewPassword
    {
        get => _addViewPassword;
        set => SetProperty(ref _addViewPassword, value); 
    }

    public IRelayCommand AddViewSaveCommand { get; }
    
    // Back Button
    public IRelayCommand ToHomeView { get; }

    public AddViewModel(Action navigateToHome, ICryptoService cryptoService, SessionService sessionService)
    {
        _navigateToHome = navigateToHome;
        ToHomeView = new RelayCommand(GoBackToHomeView);
        _cryptoService = cryptoService;
        AddViewSaveCommand = new RelayCommand(SecureAndSaveData);
        _sessionService = sessionService;
    }
    
    private void SecureAndSaveData()
    {
        var website = _addViewWebsite;
        var username = _addViewUsername;
        var password = _addViewPassword;

        if (string.IsNullOrWhiteSpace(website) ||
            string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password)) return;


        var master = _sessionService.HashedMasterPassword;
        var encryptedPassword = _cryptoService.EncryptPassword(password);
        _cryptoService.InsertVaultData(website, username, encryptedPassword);
        GoBackToHomeView();
        
    }
    
    private void GoBackToHomeView() 
    { 
        _navigateToHome();
    }
}