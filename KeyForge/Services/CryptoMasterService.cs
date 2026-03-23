using System;
using System.Security.Cryptography;
using KeyForge.Data;
using KeyForge.Models;

namespace KeyForge.Services;

public interface ICryptoMasterService
{
    string HashMasterPassword(string password);
    
    void InsertUserData(string username, byte[] hashedPassword);
}

public class CryptoMasterService : ICryptoMasterService
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

    public CryptoMasterService(KeyForgeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public string HashMasterPassword(string password)
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

    public void InsertUserData(string username, byte[] hashedPassword)
    {
        var user = new User(username, hashedPassword);
        
        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();
    }
}