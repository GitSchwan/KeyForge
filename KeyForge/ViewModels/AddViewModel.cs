using System;
using CommunityToolkit.Mvvm.Input;
using KeyForge.Services;

namespace KeyForge.ViewModels;

public partial class AddViewModel : ViewModelBase
{
    private readonly Action _navigateToHome;

    
    private string? _addViewWebsite;
    private string? AddViewWebsite
    {
        get => _addViewWebsite;
        set => SetProperty(ref _addViewWebsite, value); 
    }
    
    private string? _addViewUsername;
    
    private string? AddViewUsername
    {
        get => _addViewUsername;
        set => SetProperty(ref _addViewUsername, value); 
    }
    
    private string? _addViewPassword;
    
    private string? AddViewPassword
    {
        get => _addViewPassword;
        set => SetProperty(ref _addViewPassword, value); 
    }

    public IRelayCommand AddViewSaveCommand { get; }
    
    // Back Button
    public IRelayCommand ToHomeView { get; }

    public AddViewModel(Action navigateToHome)
    {
        _navigateToHome = navigateToHome;
        ToHomeView = new RelayCommand(GoBackToHomeView);
        
        AddViewSaveCommand = new RelayCommand(SecureAndSaveData);
    }
    
        private void SecureAndSaveData()
    {
        var website = AddViewWebsite;
        var username = AddViewUsername;
        var password = AddViewPassword;

        if (string.IsNullOrWhiteSpace(website) ||
            string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password)) return;
        
        CryptoService.Hash(password, new byte[32]);
    }
    
    private void GoBackToHomeView() 
    { 
        _navigateToHome();
    }
}