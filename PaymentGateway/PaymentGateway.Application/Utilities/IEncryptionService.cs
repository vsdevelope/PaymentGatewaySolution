namespace PaymentGateway.Application.Utilities
{
    public interface IEncryptionService
    {
        string Encrypt(string clearText);
        string Decrypt(string cipherText);
    }
}