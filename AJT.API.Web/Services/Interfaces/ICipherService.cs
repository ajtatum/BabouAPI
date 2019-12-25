namespace Babou.API.Web.Services.Interfaces
{
    public interface ICipherService
    {
        string Encrypt(string input);
        string Decrypt(string cipherText);
    }
}
