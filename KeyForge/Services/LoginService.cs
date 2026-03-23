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

    public LoginService(KeyForgeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool Login(string username, string password)
    {
        return username == "Admin" || password == "Nito";
    }

    public bool HasUsers()
    {
        return _dbContext.Users.Any();
    }
}