namespace Investissement_WebClient.Application.Services.Encrypt
{
    public interface ICryptService
    {
        string Encrypt(string plainText, string masterKey);

        string Decrypt(string cipherText, string masterKey);
    }
}
