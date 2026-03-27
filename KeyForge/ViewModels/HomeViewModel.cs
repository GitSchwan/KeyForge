using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using KeyForge.Data;
using KeyForge.Models;
using KeyForge.Services;
using KeyForge.Views;

namespace KeyForge.ViewModels;

public class HomeViewModel : ViewModelBase
{
    public ObservableCollection<VaultEntry> Data { get; }
    private readonly IVaultService _vaultService;
    private readonly SessionService _sessionService;

    public IRelayCommand NavigateToAddCommand { get; }
    public IRelayCommand<VaultEntry> SaveCommand { get; }
    public IRelayCommand<VaultEntry> DeleteEntryCommand { get; }

    private string _welcomeMessage = string.Empty;
    public string WelcomeMessage
    {
        get => _welcomeMessage;
        set => SetProperty(ref _welcomeMessage, value);
    }

    public HomeViewModel(
        Action navigateToAdd,
        IVaultService vaultService,
        SessionService sessionService)
    {
        NavigateToAddCommand = new RelayCommand(navigateToAdd);
        _vaultService = vaultService;
        _sessionService = sessionService;

        SaveCommand = new RelayCommand<VaultEntry>(Save);
        DeleteEntryCommand = new AsyncRelayCommand<VaultEntry>(DeleteEntryAsync);

        WelcomeMessage = $"Willkommen {_sessionService.CurrentUsername}";

        Data = new ObservableCollection<VaultEntry>();
        LoadData();
    }

    private async Task DeleteEntryAsync(VaultEntry? entry)
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

    private void LoadData()
    {
        var entries = _vaultService.GetVaultEntriesForUser(_sessionService.CurrentUserId);
        Data.Clear();

        foreach (var entry in entries)
            Data.Add(entry);
    }
}
