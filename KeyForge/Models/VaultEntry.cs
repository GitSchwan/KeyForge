using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;
using KeyForge.Data;

namespace KeyForge.Models;

public class VaultEntry : INotifyPropertyChanged
{
    #region Private Fields

    private bool _isLoading;
    private bool _isPasswordDecrypted;
    private bool _isModified = false;

    #endregion

    #region Constructors

    public VaultEntry(string website, string websiteusername, int userId, string password)
    {
        _isLoading = true;
        IsModified = false;

        Website = website;
        Websiteusername = websiteusername;
        UserId = userId;
        Password = password;

        _isLoading = false;
        IsModified = false;
        IsPasswordDecrypted = false;
    }

    #endregion

    #region Properties

    public int Id { get; set; }

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

    public bool IsPasswordDecrypted
    {
        get => _isPasswordDecrypted;
        set
        {
            if (_isPasswordDecrypted != value)
            {
                _isPasswordDecrypted = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsPasswordReadOnly));
            }
        }
    }

    public bool IsPasswordReadOnly => !IsPasswordDecrypted;

    public User? User { get; set; }

    public bool IsModified
    {
        get => _isModified;
        set
        {
            _isModified = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Events

    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion

    #region Methods

    /// <summary>
    /// Raises the PropertyChanged event
    /// </summary>
    /// <param name="name"><see cref="string"/></param>
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    #endregion
}
