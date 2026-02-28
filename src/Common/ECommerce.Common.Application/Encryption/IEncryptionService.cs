namespace ECommerce.Common.Application.Encryption;

public interface IEncryptionService
{
    EncryptionResult Encrypt(string plainText);

    string Decrypt(EncryptionResult encryptionResult);
}
