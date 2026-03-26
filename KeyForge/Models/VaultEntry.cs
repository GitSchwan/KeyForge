using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using System.ComponentModel.DataAnnotations;
using KeyForge.Data;

namespace KeyForge.Models;


public class VaultEntry : INotifyPropertyChanged
{
    public int Id { get; set; }

    private bool _isLoading;

    private string _website = string.Empty;

    [MaxLength(200)]
    public string Website
    {
        get => _website;
        set
        {
            if (_website != value)
            {
                _website = value;
                OnPropertyChanged();
                if (!_isLoading)
                    IsModified = true;
            }   
        }
    }

    private string _websiteusername = string.Empty;
    [MaxLength(200)]
    public string Websiteusername
    {
        get => _websiteusername;
        set
        {
            if (_websiteusername != value)
            {
                _websiteusername = value;
                OnPropertyChanged();
                if (!_isLoading)
                    IsModified = true;
            }   
        }
    }

    public int UserId { get; set; }

    private string _password = string.Empty;
    
    [MaxLength(500)]
    public string Password
    {
        get => _password;
        set
        {
            if (_password != value)
            {
                _password = value;
                OnPropertyChanged();
                if (!_isLoading)
                    IsModified = true;
            }
        }
    }

    public User? User { get; set; }

    private bool _isModified = false;

    public bool IsModified
    {
        get => _isModified;
        set{ 
            _isModified = value; 
            Console.WriteLine("Modified: " + _isModified);
            OnPropertyChanged();
        }
    }

    public void Save(VaultEntry? entry)
    {
        Console.WriteLine("Save");
        if (entry is null)
            return;

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

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the PropertyChanged event
    /// </summary>
    /// <param name="name"><see cref="string"/></param>
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


    public VaultEntry(string website, string websiteusername ,int userId, string password)
    {
        _isLoading = true; // prevent Save Button from being enabled
        IsModified = false;
        
        Website = website;
        Websiteusername = websiteusername;
        UserId = userId;
        Password = password;
        
        _isLoading = false;
        IsModified = false;
    }
}
