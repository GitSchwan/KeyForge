using System.Security.Cryptography;

namespace KeyForge.Services;

public class CryptoService
{
    public static string Hash(string password, byte[] key)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        return "AAA";
    }
}