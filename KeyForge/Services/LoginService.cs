using System.Linq;
using KeyForge.Data;
using Microsoft.EntityFrameworkCore;

namespace KeyForge.Services;

public interface ILoginService
{
    bool LoginServiceLogin(string username, string password);
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

        var success = _cryptoService.VerifyPassword(password, user.MasterPassword);
        if (!success) return false;

        _sessionService.SetCurrentUser(user.Id, user.Name, user.MasterPassword);
        return true;
    }

    public bool HasUsers()
    {
        return _dbContext.Users.Any();
    }
}