using System.Linq;
using KeyForge.Data;
using Microsoft.EntityFrameworkCore;

namespace KeyForge.Services;

public interface ILoginService
{
    bool Login(string username, string password);
    bool HasUsers();
}

public class LoginService : ILoginService
{
    private readonly KeyForgeDbContext _dbContext;
    private readonly ICryptoMasterService _cryptoMasterService;

    public LoginService(KeyForgeDbContext dbContext)
    {
        _dbContext = dbContext;
        _cryptoMasterService = new CryptoMasterService(dbContext);
    }

    public bool Login(string username, string password)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Name == username); // If user doesn't exist, this will return null'
        if (user is null) return false;                                                                        
        
        return _cryptoMasterService.VerifyMasterPassword(password, user.MasterPassword);
    }

    public bool HasUsers()
    {
        return _dbContext.Users.Any();
    }
}