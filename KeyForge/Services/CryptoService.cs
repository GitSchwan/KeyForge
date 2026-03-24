using System;
using System.Security.Cryptography;
using KeyForge.Data;
using KeyForge.Models;

namespace KeyForge.Services;

public interface ICryptoService
{
    string HashPassword(string password);
    
    void InsertUserData(string username, string hashedPassword);
    
    bool VerifyPassword(string password, string storedHash);
}

public class CryptoService : ICryptoService
{
    private const int SaltSize = 16;
    private const int KeySize = 32;

    /// <summary>
    /// Represents the number of iterations used in the key derivation process
    /// for securely hashing the master password.
    /// </summary>
    /// <remarks>
    /// Increasing the number of iterations enhances security by making brute-force
    /// attacks more computationally expensive. However, it also impacts the
    /// performance of the hashing process. The default value in this implementation
    /// is set to 100,000 iterations, which balances security and performance.
    /// </remarks>
    private const int Iterations = 100_000;

    private readonly KeyForgeDbContext _dbContext;

    public CryptoService(KeyForgeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty :3", nameof(password));

        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

        using var deriveBytes = new Rfc2898DeriveBytes(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256);

        byte[] hash = deriveBytes.GetBytes(KeySize);

        // Store: iterations.salt.hash
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public void InsertUserData(string username, string hashedPassword)
    {
        var user = new User(username, hashedPassword);
        
        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();
    }

    public bool VerifyPassword(string password, string storedHash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHash))
            return false;

        var parts = storedHash.Split('.');
        if (parts.Length != 3)
            return false;

        int iterations = int.Parse(parts[0]);
        byte[] salt = Convert.FromBase64String(parts[1]);
        byte[] expectedHash = Convert.FromBase64String(parts[2]);

        using var deriveBytes = new Rfc2898DeriveBytes(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256);

        byte[] actualHash = deriveBytes.GetBytes(expectedHash.Length);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}