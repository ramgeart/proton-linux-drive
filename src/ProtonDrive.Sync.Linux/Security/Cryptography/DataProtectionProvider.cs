using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ProtonDrive.Shared.Security.Cryptography;

namespace ProtonDrive.Sync.Linux.Security.Cryptography;

/// <inheritdoc cref="IDataProtectionProvider"/>
/// <summary>
/// Linux implementation using AES encryption with user-specific key derived from system info.
/// For production use, this should integrate with GNOME Keyring or KDE Wallet.
/// </summary>
public class DataProtectionProvider : IDataProtectionProvider
{
    private static readonly byte[] Salt =
    {
        0x42, 0xD0, 0xA5, 0x24, 0x15, 0x92, 0x3C, 0x78,
        0x7A, 0x1D, 0xFE, 0x11, 0x39, 0xF4, 0x5B, 0x72,
    };

    private readonly byte[] _key;

    public DataProtectionProvider()
    {
        // Generate a user-specific key from system information
        // In production, this should use libsecret/KWallet
        _key = DeriveKey();
    }

    public string Protect(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            return data;
        }

        var unprotectedData = Encoding.UTF8.GetBytes(data);
        var protectedData = Protect(unprotectedData);

        return Convert.ToBase64String(protectedData.Span);
    }

    public ReadOnlyMemory<byte> Protect(ReadOnlyMemory<byte> data)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        using var msEncrypt = new MemoryStream();
        
        // Write IV first
        msEncrypt.Write(aes.IV, 0, aes.IV.Length);
        
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        {
            csEncrypt.Write(data.Span);
        }

        return msEncrypt.ToArray();
    }

    public string Unprotect(string data)
    {
        try
        {
            var protectedData = Convert.FromBase64String(data);
            var unprotectedData = Unprotect(protectedData);

            return Encoding.UTF8.GetString(unprotectedData.Span);
        }
        catch (FormatException)
        {
            throw new CryptographicException();
        }
        catch (ArgumentException)
        {
            throw new CryptographicException();
        }
    }

    public ReadOnlyMemory<byte> Unprotect(ReadOnlyMemory<byte> data)
    {
        using var aes = Aes.Create();
        aes.Key = _key;

        var iv = new byte[aes.IV.Length];
        data.Span[..iv.Length].CopyTo(iv);
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        using var msDecrypt = new MemoryStream(data.ToArray(), iv.Length, data.Length - iv.Length);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var msResult = new MemoryStream();
        
        csDecrypt.CopyTo(msResult);
        return msResult.ToArray();
    }

    private static byte[] DeriveKey()
    {
        // Derive key from user name and machine ID
        // TODO: In production, integrate with libsecret (GNOME) or KWallet (KDE)
        var userName = Environment.UserName;
        var machineId = GetMachineId();
        var keySource = $"{userName}:{machineId}";

        using var pbkdf2 = new Rfc2898DeriveBytes(
            Encoding.UTF8.GetBytes(keySource),
            Salt,
            10000,
            HashAlgorithmName.SHA256);

        return pbkdf2.GetBytes(32); // 256-bit key
    }

    private static string GetMachineId()
    {
        // Try to read /etc/machine-id (systemd)
        try
        {
            if (File.Exists("/etc/machine-id"))
            {
                return File.ReadAllText("/etc/machine-id").Trim();
            }
        }
        catch
        {
            // Fall back to hostname
        }

        return Environment.MachineName;
    }
}
