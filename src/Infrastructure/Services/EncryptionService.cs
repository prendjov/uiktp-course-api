using Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services;

public class EncryptionService : IEncryption
{
    public string Encrypt(string clearText)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = DeriveKey("CourseEducationSecretKey");
            aes.Mode = CipherMode.CBC;

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(clearText);
                }

                var iv = aes.IV;
                var encrypted = ms.ToArray();
                var result = new byte[iv.Length + encrypted.Length];
                Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                Buffer.BlockCopy(encrypted, 0, result, iv.Length, encrypted.Length);

                return Convert.ToBase64String(result);
            }
        }
    }

    public string Decrypt(string cipherText)
    {
        var fullCipher = Convert.FromBase64String(cipherText);

        using (var aes = Aes.Create())
        {
            aes.Key = DeriveKey("CourseEducationSecretKey");
            aes.Mode = CipherMode.CBC;

            var iv = new byte[aes.BlockSize / 8];
            var cipher = new byte[fullCipher.Length - iv.Length];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            aes.IV = iv;

            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream(cipher))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }
    }

    public byte[] DeriveKey(string password, int keySize = 256)
    {
        using (var deriveBytes = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes("CourseEducationSaltValue"), 10000))
        {
            return deriveBytes.GetBytes(keySize / 8);
        }
    }
}
