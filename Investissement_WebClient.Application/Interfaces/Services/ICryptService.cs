namespace Investissement_WebClient.Application.Interfaces.Services
{
    public interface ICryptService
    {
        string Encrypt(string plainText, string masterKey);

        string Decrypt(string cipherText, string masterKey);
    }
}
