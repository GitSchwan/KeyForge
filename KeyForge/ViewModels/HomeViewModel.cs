using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using KeyForge.Models;
using KeyForge.Services;

namespace KeyForge.ViewModels;

public class HomeViewModel : ViewModelBase
{
    public ObservableCollection<VaultEntry> Data { get; }
    private readonly IVaultService _vaultService;
    private readonly SessionService _sessionService;

    public IRelayCommand NavigateToAddCommand { get; }
    
    public ICommand SaveCommand { get; }

    private string _welcomeMessage = string.Empty;

    public string WelcomeMessage
    {
        get => _welcomeMessage;
        set => SetProperty(ref _welcomeMessage, value);
    }

    public HomeViewModel(
        Action navigateToAdd,
        IVaultService vaultService,
        SessionService sessionService )
    {
        NavigateToAddCommand = new RelayCommand(navigateToAdd);
        _vaultService = vaultService;
        _sessionService = sessionService;
        SaveCommand = new RelayCommand<VaultEntry>(Save);

        WelcomeMessage = $"Willkommen {_sessionService.CurrentUsername ?? "Gast"}.";

        Data = new ObservableCollection<VaultEntry>();
        LoadData();
    }

    private void Save(VaultEntry? entry)
    {
        if (entry is null)
            return;

        entry.IsModified = false;
    }

    private void LoadData()
    {
        var entries = _vaultService.GetVaultEntriesForUser(_sessionService.CurrentUserId);
        Data.Clear();

        foreach (var entry in entries)
            Data.Add(entry);
    }
}
