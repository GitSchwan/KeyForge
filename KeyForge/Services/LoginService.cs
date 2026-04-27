using System.Linq;
using KeyForge.Data;
using Microsoft.EntityFrameworkCore;

namespace KeyForge.Services;

public interface ILoginService
{
    /// <summary>
    /// Checks if the user exists and if the password is correct.
    /// </summary>
    /// <param name="username"><see cref="string"/></param>
    /// <param name="password"><see cref="string"/></param>
    bool LoginServiceLogin(string username, string password);
    
    /// <summary>
    /// Checks if there are any users in the database.
    /// </summary>
    bool HasUsers();
}

public class LoginService : ILoginService
{
    private readonly KeyForgeDbContext _dbContext;
    private readonly ICryptoService _cryptoService;
    private readonly SessionService _sessionService;

    public LoginService(KeyForgeDbContext dbContext, SessionService sessionService)
    {
        _dbContext = dbContext;
        _cryptoService = new CryptoService(dbContext, sessionService);
        _sessionService = sessionService;
    }
    
    public bool LoginServiceLogin(string username, string password)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Name == username);
        if (user is null) return false;
        
        var userSettings = _dbContext.UserSettings.FirstOrDefault(x => x.UserId == user.Id);

        var success = _cryptoService.VerifyPassword(password, user.MasterPassword);
        if (!success) return false;

        _sessionService.SetCurrentUser(user.Id, user.Name, user.MasterPassword, userSettings?.PreferredThemeId);
        return true;
    }

    public bool HasUsers()
    {
        return _dbContext.Users.Any();
    }
}