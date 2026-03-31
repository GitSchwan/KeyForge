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
    
    string EncryptPassword(string password);
    
    string DecryptPassword(string encryptedPassword);
    
    byte[] DeriveEncryptionKey(string masterPassword, string storedHash);
    
    string GenerateSavePassword();
}

public class CryptoService : ICryptoService
{
    #region Constants
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int NonceSize = 12;
    private const int TagSize = 16;
    private const string HashVersion = "v1";
    private const string EncVersion = "v2";
 
    
    /// <summary>
    /// Represents the number of iterations used in the key derivation process
    /// for securely hashing the master password.
    /// </summary>
    /// <remarks>
    /// Increasing the number of iterations enhances security by making brute-force
    /// attacks more computationally expensive. However, it also impacts the
    /// performance of the hashing process. The default value in this implementation
    /// is set to 100 thousand iterations, which balances security and performance.
    /// </remarks>
    private const int Iterations = 100_000;
    
    #endregion

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
    /// Derives an encryption key from a master password and a stored hash.
    /// </summary>
    /// <param name="masterPassword"></param>
    /// <param name="storedHash"></param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    public byte[] DeriveEncryptionKey(string masterPassword, string storedHash)
    {
        var parts = storedHash.Split('.');
        if (parts.Length != 4)
            throw new FormatException("Invalid hash format");

        byte[] salt = Convert.FromBase64String(parts[2]);

        return Rfc2898DeriveBytes.Pbkdf2(
            masterPassword,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize);
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
    /// Encrypts a password using the encryption key stored in the session.
    /// </summary>
    /// <param name="password"><see cref="string"/></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public string EncryptPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));

        if (_sessionService.EncryptionKey == null)
            throw new InvalidOperationException("No encryption key in session");

        byte[] key = _sessionService.EncryptionKey;

        byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);
        byte[] plaintext = System.Text.Encoding.UTF8.GetBytes(password);
        byte[] ciphertext = new byte[plaintext.Length];
        byte[] tag = new byte[TagSize];

        using var aes = new AesGcm(key, TagSize);
        aes.Encrypt(nonce, plaintext, ciphertext, tag);

        return $"{EncVersion}.{Convert.ToBase64String(nonce)}.{Convert.ToBase64String(tag)}.{Convert.ToBase64String(ciphertext)}";
    }

    /// <summary>
    /// Decrypts an encrypted password using the encryption key stored in the session.
    /// </summary>
    /// <param name="encryptedPassword"><see cref="string"/></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="FormatException"></exception>
    public string DecryptPassword(string encryptedPassword)
    {
        if (string.IsNullOrWhiteSpace(encryptedPassword))
            throw new ArgumentException("Encrypted password cannot be empty", nameof(encryptedPassword));

        if (_sessionService.EncryptionKey == null)
            throw new InvalidOperationException("No encryption key in session");

        var parts = encryptedPassword.Split('.');
        if (parts.Length != 4)
            throw new FormatException("Invalid encrypted format");

        byte[] nonce = Convert.FromBase64String(parts[1]);
        byte[] tag = Convert.FromBase64String(parts[2]);
        byte[] ciphertext = Convert.FromBase64String(parts[3]);

        byte[] key = _sessionService.EncryptionKey;
        byte[] plaintext = new byte[ciphertext.Length];

        using var aes = new AesGcm(key, TagSize);
        aes.Decrypt(nonce, ciphertext, tag, plaintext);

        return System.Text.Encoding.UTF8.GetString(plaintext);
    }

    public string GenerateSavePassword()
    {
        const string validChars = "!#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_abcdefghijklmnopqrstuvwxyz{|}~";
        
        return string.Create(20, validChars, (span, chars) =>
        {
            for (int i = 0; i < span.Length; i++)
            {
                span[i] = chars[RandomNumberGenerator.GetInt32(chars.Length)];
            }
        });
    }
    
    #region Database Operations
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
    #endregion
    
}