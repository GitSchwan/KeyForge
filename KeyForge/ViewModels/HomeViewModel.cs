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
    public IAsyncRelayCommand<VaultEntry> ToggleEditCommand { get; }
    public IAsyncRelayCommand<VaultEntry> DeleteEntryCommand { get; }
    
    public IRelayCommand<VaultEntry> DecryptSelectedEntryCommand { get; }
    public IAsyncRelayCommand<string> CopyToClipboardCommand { get; }

    private string _welcomeMessage = string.Empty;
    public string WelcomeMessage
    {
        get => _welcomeMessage;
        set => SetProperty(ref _welcomeMessage, value);
    }
    
    private string _editButtonText = "Bearbeiten";
    public string EditButtonText
    {
        get => _editButtonText;
        set => SetProperty(ref _editButtonText, value);
    }

    private bool _editNotAllowed;
    public bool EditNotAllowed
    {
        get => _editNotAllowed;
        set
        {
            if (SetProperty(ref _editNotAllowed, value))
            {
                if (SelectedEntry != null)
                {
                    SelectedEntry.IsHomeViewModelEditAllowed = !value;
                }
            }
        }
    }

    private bool _canSave;
    public bool CanSave
    {
        get => _canSave;
        set => SetProperty(ref _canSave, value);
    }

    private VaultEntry? _selectedEntry;
    public VaultEntry? SelectedEntry
    {
        get => _selectedEntry;
        set
        {
            if (SetProperty(ref _selectedEntry, value))
            {
                EditNotAllowed = true;
                EditButtonText = "Bearbeiten";
                
                if (_selectedEntry != null)
                {
                    _selectedEntry.IsHomeViewModelEditAllowed = false;
                }
            }
        }
    }
    
    #endregion

    private readonly HashSet<int> _decryptedEntries = new();
    
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

        ToggleEditCommand = new AsyncRelayCommand<VaultEntry>(ToggleEditAsync);
        DeleteEntryCommand = new AsyncRelayCommand<VaultEntry>(DeleteEntryAsync);
        DecryptSelectedEntryCommand = new RelayCommand<VaultEntry>(DecryptSelectedEntry);
        CopyToClipboardCommand = new AsyncRelayCommand<string>(CopyToClipboardAsync);

        WelcomeMessage = $"Willkommen {_sessionService.CurrentUsername}";
        
        EditNotAllowed = true;

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

    private async Task ToggleEditAsync(VaultEntry? entry)
    {
        if (entry is null) return;

        if (EditNotAllowed)
        {
            EditNotAllowed = false;
            EditButtonText = "Speichern";
        }
        else
        {
            await SaveAsync(entry);
            EditButtonText = "Bearbeiten";
        }
    }

    private async Task SaveAsync(VaultEntry? entry)
    {
        if (entry is null) return;

        await Task.Run(() =>
        {
            using var context = new KeyForgeDbContext();
            var existingEntry = context.VaultEntries.Find(entry.Id);

            if (existingEntry is null)
                return;

            existingEntry.Website = entry.Website;
            existingEntry.Websiteusername = entry.Websiteusername;

            if (entry.IsPasswordDecrypted)
            {
                existingEntry.Password = _cryptoService.EncryptPassword(entry.Password);
                
                entry.Password = existingEntry.Password;
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
        });
    }

    private void DecryptSelectedEntry(VaultEntry? entry)
    {
        if (entry is null) return;

        using var context = new KeyForgeDbContext();
        var password = _vaultService.GetUserSpecifcWebsitePassword(entry.Id, _sessionService.CurrentUserId);

        if (entry.IsPasswordDecrypted)
        {
            entry.Password = password;
            entry.IsPasswordDecrypted = false;
        }
        else
        {
            entry.Password = _cryptoService.DecryptPassword(password);
            entry.IsPasswordDecrypted = true;
        }

        CanSave = true;
    }

    public event EventHandler<string>? ClipboardCopyHappened;
    private static System.Timers.Timer? _clipboardTimer;

    private async Task CopyToClipboardAsync(string? text)
    {
        if (string.IsNullOrEmpty(text)) return;

        var topLevel = App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;

        if (topLevel?.Clipboard is { } clipboard)
        {
            await clipboard.SetTextAsync(text);
            ClipboardCopyHappened?.Invoke(this, "In die Zwischenablage kopiert");

            // Clear after 20 seconds
            _clipboardTimer?.Stop();
            _clipboardTimer?.Dispose();
            
            _clipboardTimer = new System.Timers.Timer(20000); // 20 seconds
            _clipboardTimer.AutoReset = false;
            _clipboardTimer.Elapsed += async (_, _) =>
            {
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    if (topLevel?.Clipboard is { } cb)
                    {
                        await cb.SetTextAsync(string.Empty);
                    }
                });
            };
            _clipboardTimer.Start();
        }
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
