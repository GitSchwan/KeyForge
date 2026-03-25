using System;
using Avalonia.Controls;

namespace KeyForge.Services;

public class SessionService
{
    public int CurrentUserId { get; private set; }
    public string? CurrentUsername { get;  private set; }

    public bool IsLoggedIn => CurrentUserId > -1;

    public void SetCurrentUser(int userId, string username)
    {
        CurrentUserId = userId;
        CurrentUsername = username;
    }

    public void Clear()
    {
        CurrentUserId = -1;
        CurrentUsername = null;
    }
}