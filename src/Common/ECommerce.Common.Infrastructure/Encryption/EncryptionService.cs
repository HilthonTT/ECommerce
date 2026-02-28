using ECommerce.Common.Application.Encryption;
using System.Security.Cryptography;

namespace ECommerce.Common.Infrastructure.Encryption;

internal sealed class EncryptionService : IEncryptionService
{
    private const int KeySize = 256;
    private const int BlockSize = 128;

    public EncryptionResult Encrypt(string plainText)
    {
        // Generate a random key and IV
        using var aes = Aes.Create();
        aes.KeySize = KeySize;
        aes.BlockSize = BlockSize;

        // Generate a random key and IV for each encryption operation
        aes.GenerateKey();
        aes.GenerateIV();

        byte[] encryptedData;

        // Create encryptor and encrypt the data
        using (var encryptor = aes.CreateEncryptor())
        using (var msEncrypt = new MemoryStream())
        {
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }

            encryptedData = msEncrypt.ToArray();
        }

        // Package everything together, storing IV with the encrypted data
        var result = EncryptionResult.CreateEncryptedData(
            encryptedData,
            aes.IV,
            Convert.ToBase64String(aes.Key)
        );

        return result;
    }

    public string Decrypt(EncryptionResult encryptionResult)
    {
        var key = Convert.FromBase64String(encryptionResult.Key);
        var (iv, encryptedData) = encryptionResult.GetIVAndEncryptedData();

        using var aes = Aes.Create();
        aes.KeySize = KeySize;
        aes.BlockSize = BlockSize;
        aes.Key = key;
        aes.IV = iv;

        // Create decryptor and decrypt the data
        using var decryptor = aes.CreateDecryptor();
        using var msDecrypt = new MemoryStream(encryptedData);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);

        try
        {
            return srDecrypt.ReadToEnd();
        }
        catch (CryptographicException ex)
        {
            // Log the error securely - avoid exposing details
            throw new CryptographicException("Decryption failed", ex);
        }
    }
}
