using System.ComponentModel;
using System.Runtime.CompilerServices;

using System.ComponentModel.DataAnnotations;

namespace KeyForge.Models;


public class VaultEntry : INotifyPropertyChanged
{
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
                IsModified = true;
            }
        }
    }

    public User? User { get; set; }

    private bool _isModified;

    public bool IsModified
    {
        get => _isModified;
        set{ 
            _isModified = value; 
            OnPropertyChanged();
        }
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
        Website = website;
        Websiteusername = websiteusername;
        UserId = userId;
        Password = password;
    }
}
