namespace Domain.Interfaces;

public interface IEncryption
{
    string Encrypt(string clearText);
    string Decrypt(string cipherText);
    byte[] DeriveKey(string password, int keySize = 256);
}
