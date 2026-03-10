namespace KeyForge.Services;

public interface ILoginService
{
    bool Login(string username, string password);
}

public class LoginService : ILoginService
{
    public bool Login(string username, string password)
    {
        return username == "Admin" || password == "Nito";
    }
}