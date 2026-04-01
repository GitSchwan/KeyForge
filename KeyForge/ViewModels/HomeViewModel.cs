using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using KeyForge.Data;
using KeyForge.Models;
using KeyForge.Services;
using KeyForge.Views;

namespace KeyForge.ViewModels;

public class HomeViewModel : ViewModelBase
{
    #region Properties and Command
    
    public ObservableCollection<VaultEntry> Data { get; }
    private readonly IVaultService _vaultService;
    private readonly SessionService _sessionService;
    private readonly ICryptoService _cryptoService;

    public IRelayCommand NavigateToAddCommand { get; }
    public IRelayCommand<VaultEntry> SaveCommand { get; }
    public IRelayCommand<VaultEntry> DeleteEntryCommand { get; }
    
    public IRelayCommand<VaultEntry> DecryptSelectedEntryCommand { get; }

    private string _welcomeMessage = string.Empty;
    public string WelcomeMessage
    {
        get => _welcomeMessage;
        set => SetProperty(ref _welcomeMessage, value);
    }
    
    private bool _editNotAllowed;
    public bool EditNotAllowed
    {
        get => _editNotAllowed;
        set => SetProperty(ref _editNotAllowed, value);
    }

    private bool _canSave;
    public bool CanSave
    {
        get => _canSave;
        set => SetProperty(ref _canSave, value);
    }
    
    #endregion

    private readonly HashSet<int> _decryptedEntries = new();
    private bool _shown;
    
    public HomeViewModel(
        Action navigateToAdd,
        IVaultService vaultService,
        SessionService sessionService,
        ICryptoService cryptoService)
    {
        NavigateToAddCommand = new RelayCommand(navigateToAdd);
        _vaultService = vaultService;
        _sessionService = sessionService;
        _cryptoService = cryptoService;

        SaveCommand = new RelayCommand<VaultEntry>(Save);
        DeleteEntryCommand = new AsyncRelayCommand<VaultEntry>(DeleteEntryAsync);
        DecryptSelectedEntryCommand = new RelayCommand<VaultEntry>(DecryptSelectedEntry);

        WelcomeMessage = $"Willkommen {_sessionService.CurrentUsername}";

        Data = new ObservableCollection<VaultEntry>();
        LoadData();
    }

    private async Task DeleteEntryAsync(VaultEntry? entry) //async because of the dialog
    {
        if (entry is null)
            return;

        var dialog = new ConfirmPopUpView
        {
            DataContext = new ConfirmPopUpViewModel("Wirklich löschen?")
        };

        var owner = App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;

        bool confirmed = owner is not null && await dialog.ShowDialog<bool>(owner);

        if (confirmed)
        {
            DeleteEntry(entry);
            Data.Remove(entry);
        }
    }

    private void Save(VaultEntry? entry)
    {
        if (entry is null) return;

        using var context = new KeyForgeDbContext();
        var existingEntry = context.VaultEntries.Find(entry.Id);

        if (existingEntry is null)
            return;

        existingEntry.Website = entry.Website;
        existingEntry.Websiteusername = entry.Websiteusername;

        if (entry.IsPasswordDecrypted)
        {
            existingEntry.Password = _cryptoService.EncryptPassword(entry.Password);
            entry.IsPasswordDecrypted = false;
        }
        else
        {
            existingEntry.Password = entry.Password;
        }

        context.SaveChanges();
        entry.IsModified = false;
        CanSave = false;
        EditNotAllowed = true;
    }

    private void DecryptSelectedEntry(VaultEntry? entry)
    {
        if (entry is null) return;

        using var context = new KeyForgeDbContext();
        var password = _vaultService.getUserSpecifcWebsitePassword(entry.Id, _sessionService.CurrentUserId);

        if (entry.IsPasswordDecrypted)
        {
            entry.Password = password;
            entry.IsPasswordDecrypted = false;
            EditNotAllowed = true;
        }
        else
        {
            entry.Password = _cryptoService.DecryptPassword(password);
            entry.IsPasswordDecrypted = true;
            EditNotAllowed = false;
        }

        CanSave = true;
    }

    private static void DeleteEntry(VaultEntry? entry)
    {
        if (entry is null) return;

        using var context = new KeyForgeDbContext();
        {
            context.VaultEntries.Remove(entry);
            context.SaveChanges();
        }
    }

    private void LoadData()
    {
        var entries = _vaultService.GetVaultEntriesForUser(_sessionService.CurrentUserId);
        Data.Clear();

        foreach (var entry in entries)
        {
            entry.IsPasswordDecrypted = false;
            Data.Add(entry);
        }
    }
}
