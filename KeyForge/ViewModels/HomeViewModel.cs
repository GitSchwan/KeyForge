using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using KeyForge.Models;
using KeyForge.Services;

namespace KeyForge.ViewModels;

public class HomeViewModel : ViewModelBase
{
    public ObservableCollection<VaultEntry> Data { get; }
    private readonly Action _navigateToAdd;
    private readonly IVaultService _vaultService;
    private readonly SessionService _sessionService;

    public IRelayCommand NavigateToAddCommand { get; }

    public HomeViewModel(
        Action navigateToAdd,
        IVaultService vaultService,
        SessionService sessionService)
    {
        NavigateToAddCommand = new RelayCommand(navigateToAdd);
        _navigateToAdd = navigateToAdd;
        _vaultService = vaultService;
        _sessionService = sessionService;

        Data = new ObservableCollection<VaultEntry>();
        LoadData();
    }

    private void LoadData()
    {
        var entries = _vaultService.GetVaultEntriesForUser(_sessionService.CurrentUserId);
        Data.Clear();

        foreach (var entry in entries)
            Data.Add(entry);
    }

    private void NavigateToAdd()
    {
        _navigateToAdd();
    }
}
