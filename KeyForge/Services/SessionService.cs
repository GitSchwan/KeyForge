using System;
using Avalonia.Controls;

namespace KeyForge.Services;

public class SessionService
{
    public int CurrentUserId { get; private set; }
    public string? CurrentUsername { get;  private set; }
    
    public string? HashedMasterPassword { get; private set; }

    public byte[]? EncryptionKey { get; set; }
    
    public bool SavePossible { get; set; }
    public bool IsLoggedIn => CurrentUserId > -1;

    public void SetCurrentUser(int userId, string username, string hashedMasterPassword)
    {
        CurrentUserId = userId;
        CurrentUsername = username;
        HashedMasterPassword = hashedMasterPassword;
    }

    public void Clear()
    {
        CurrentUserId = -1;
        CurrentUsername = null;
    }
}