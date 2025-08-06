using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class AesEncryptor
{

    private static readonly byte[] key = Encoding.UTF8.GetBytes("tak_khal_secret!");     //
    private static readonly byte[] iv = Encoding.UTF8.GetBytes("tak_khal_iv__1234");     // 

    public static string EncryptString(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentNullException("plainText");

        using (AesManaged aes = new AesManaged())
        {
            aes.Key = key;
            aes.IV = iv;
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;

            ICryptoTransform encryptor = aes.CreateEncryptor();

            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (StreamWriter sw = new StreamWriter(cs, Encoding.UTF8))
            {
                sw.Write(plainText);
                sw.Flush();
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }
}