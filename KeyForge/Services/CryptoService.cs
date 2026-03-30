using System;
using System.Linq;
using System.Security.Cryptography;
using KeyForge.Data;
using KeyForge.Models;
using Microsoft.EntityFrameworkCore;

namespace KeyForge.Services;

public interface ICryptoService
{
    string HashPassword(string password);

    void InsertUserData(string username, string hashedPassword);

    bool VerifyPassword(string password, string storedHash);

    void InsertVaultData(string website, string username, string encryptedPassword);
    
    string EncryptPassword(string password, string masterPassword);
    
    string DecryptPassword(string encryptedPassword, string masterPassword);
}

public class CryptoService : ICryptoService
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int NonceSize = 12;
    private const int TagSize = 16;
    private const string HashVersion = "v1";
    private const string EncVersion = "v1";

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
    private readonly SessionService _sessionService;

    public CryptoService(KeyForgeDbContext dbContext, SessionService sessionService)
    {
        _dbContext = dbContext;
        _sessionService = sessionService;
    }

    
    /// <summary>
    /// Hashes a password using the PBKDF2 algorithm.
    /// </summary>
    /// <param name="password"><see cref="string"/></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty :3", nameof(password));

        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize);

        return $"{HashVersion}.{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    
    /// <summary>
    /// Inserts a new user into the database.
    /// </summary>
    /// <param name="username"><see cref="string"/></param>
    /// <param name="hashedPassword"><see cref="string"/></param>
    public void InsertUserData(string username, string hashedPassword)
    {
        var user = new User(username, hashedPassword);

        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();
    }

    /// <summary>
    /// Inserts a new vault entry into the database.
    /// </summary>
    /// <param name="website"><see cref="string"/></param>
    /// <param name="username"><see cref="string"/></param>
    /// <param name="encryptedPassword"><see cref="string"/></param>
    public void InsertVaultData(string website, string username, string encryptedPassword)
    {
        var userid = _dbContext.Users.FirstOrDefault(u => u.Id == _sessionService.CurrentUserId);
        if (userid is null)
        {
            Console.WriteLine("DEBUG: No user with id " + _sessionService.CurrentUserId + " found");
            return;
        }

        var vaultEntry = new VaultEntry(website, username, _sessionService.CurrentUserId, encryptedPassword);

        _dbContext.VaultEntries.Add(vaultEntry);
        _dbContext.SaveChanges();
    }

    
    /// <summary>
    /// Verifies a password against a stored hash.
    /// </summary>
    /// <param name="password"><see cref="string"/></param>
    /// <param name="storedHash"><see cref="string"/></param>
    /// <returns></returns>
    public bool VerifyPassword(string password, string storedHash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHash))
            return false;

        var parts = storedHash.Split('.');
        if (parts.Length != 4)
            return false;

        if (!string.Equals(parts[0], HashVersion, StringComparison.Ordinal))
            return false;

        if (!int.TryParse(parts[1], out int iterations))
            return false;

        byte[] salt;
        byte[] expectedHash;

        try
        {
            salt = Convert.FromBase64String(parts[2]);
            expectedHash = Convert.FromBase64String(parts[3]);
        }
        catch (FormatException)
        {
            return false;
        }

        if (salt.Length == 0 || expectedHash.Length == 0)
            return false;

        byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            expectedHash.Length);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }

    
    /// <summary>
    /// Encrypts a password using the provided master password.
    /// </summary>
    /// <param name="password"></param>
    /// <param name="masterPassword"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public string EncryptPassword(string password, string masterPassword)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty :3", nameof(password));

        if (string.IsNullOrWhiteSpace(masterPassword))
            throw new ArgumentException("Master password cannot be empty :3", nameof(masterPassword));

        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] key = Rfc2898DeriveBytes.Pbkdf2(
            masterPassword,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize);

        byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);
        byte[] plaintext = System.Text.Encoding.UTF8.GetBytes(password);
        byte[] ciphertext = new byte[plaintext.Length];
        byte[] tag = new byte[TagSize];

        using var aes = new AesGcm(key, TagSize);
        aes.Encrypt(nonce, plaintext, ciphertext, tag);

        return $"{EncVersion}.{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(nonce)}.{Convert.ToBase64String(tag)}.{Convert.ToBase64String(ciphertext)}";
    }

    /// <summary>
    /// Decrypts a password using the provided master password.
    /// </summary>
    /// <param name="encryptedPassword" ><see cref="string"/></param>
    /// <param name="masterPassword"><see cref="string"/></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="FormatException"></exception>
    public string DecryptPassword(string encryptedPassword, string masterPassword)
    {
        if (string.IsNullOrWhiteSpace(encryptedPassword))
            throw new ArgumentException("Encrypted password cannot be empty :3", nameof(encryptedPassword));

        if (string.IsNullOrWhiteSpace(masterPassword))
            throw new ArgumentException("Master password cannot be empty :3", nameof(masterPassword));

        var parts = encryptedPassword.Split('.');
        if (parts.Length != 6)
            throw new FormatException("Invalid encrypted password format :3");

        if (!string.Equals(parts[0], EncVersion, StringComparison.Ordinal))
            throw new FormatException("Unsupported encryption version :3");

        if (!int.TryParse(parts[1], out _))
            throw new FormatException("Invalid iteration count.");

        byte[] salt = Convert.FromBase64String(parts[2]);
        byte[] nonce = Convert.FromBase64String(parts[3]);
        byte[] tag = Convert.FromBase64String(parts[4]);
        byte[] ciphertext = Convert.FromBase64String(parts[5]);

        byte[] key = Rfc2898DeriveBytes.Pbkdf2(
            masterPassword,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize);

        byte[] plaintext = new byte[ciphertext.Length];

        using var aes = new AesGcm(key, TagSize);
        aes.Decrypt(nonce, ciphertext, tag, plaintext);

        return System.Text.Encoding.UTF8.GetString(plaintext);
    }
}