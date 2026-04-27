namespace KeyForge.Services;

public class SessionService
{
    public int CurrentUserId { get; private set; }
    public string? CurrentUsername { get;  private set; }
    
    public string? HashedMasterPassword { get; private set; }

    public byte[]? EncryptionKey { get; set; }
    public bool IsLoggedIn => CurrentUserId > -1;
    
    /// <summary>
    /// Sets the current user in the Session.
    /// </summary>
    /// <param name="userId"><see cref="int"/></param>
    /// <param name="username"><see cref="string"/></param>
    /// <param name="hashedMasterPassword"><see cref="string"/></param>
    /// <param name="theme">
    /// <see cref="string"/> or <see langword="null"/>
    /// </param>
    public void SetCurrentUser(int userId, string username, string hashedMasterPassword, string? theme)
    {
        CurrentUserId = userId;
        CurrentUsername = username;
        HashedMasterPassword = hashedMasterPassword;
    }

    /// <summary>
    /// Logs the user out.
    /// </summary>
    public void Logout()
    {
        CurrentUserId = -1;
        CurrentUsername = null;
        HashedMasterPassword = null;   
    }
}