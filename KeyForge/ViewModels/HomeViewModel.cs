using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using KeyForge.Data;
using KeyForge.Models;
using KeyForge.Services;

namespace KeyForge.ViewModels;

interface IHomeViewModel
{
    void LoadData();
}

public class HomeViewModel : ViewModelBase
{
    public ObservableCollection<VaultEntry> Data { get; }
    private readonly IVaultService _vaultService;
    private readonly SessionService _sessionService;

    public IRelayCommand NavigateToAddCommand { get; }

    private string _welcomeMessage = string.Empty;
    public string WelcomeMessage
    {
        get => _welcomeMessage;
        set => SetProperty(ref _welcomeMessage, value);
    }
    
    public IRelayCommand<VaultEntry> SaveCommand { get; }
    public IRelayCommand<VaultEntry> DeleteEntryCommand { get; }

    public static void Save(VaultEntry? entry)
    {
        if (entry is null) return;

        using (var context = new KeyForgeDbContext())
        {
            var existingEntry = context.VaultEntries.Find(entry.Id);
            if (existingEntry != null)
            {
                existingEntry.Website = entry.Website;
                existingEntry.Websiteusername = entry.Websiteusername;
                existingEntry.Password = entry.Password;
                context.SaveChanges();
            }
        }

        entry.IsModified = false;
    }

    public static void DeleteEntry(VaultEntry? entry)
    {
        if (entry is null) return;

        using (var context = new KeyForgeDbContext())
        {
            context.VaultEntries.Remove(entry);
            context.SaveChanges();
        }
    }


    /// <summary>
    /// Represents the ViewModel for the Home view, managing data and commands for displaying and interacting
    /// with the user's vault entries.
    /// </summary>
    public HomeViewModel(
        Action navigateToAdd,
        IVaultService vaultService,
        SessionService sessionService)
    {
        NavigateToAddCommand = new RelayCommand(navigateToAdd);
        _vaultService = vaultService;
        _sessionService = sessionService;
        
        
        SaveCommand = new RelayCommand<VaultEntry>(Save);
        DeleteEntryCommand = new RelayCommand<VaultEntry>(DeleteEntry);

        WelcomeMessage = $"Willkommen {_sessionService.CurrentUsername}";

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
}
