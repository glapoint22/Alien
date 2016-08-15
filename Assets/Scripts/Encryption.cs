using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class Encryption
{
    public static string Encrypt(string text)
    {
        RijndaelManaged rijAlg = new RijndaelManaged();

        rijAlg.BlockSize = 256;
        rijAlg.Key = Convert.FromBase64String(GameManager.key);
        rijAlg.IV = Convert.FromBase64String(GameManager.iv);
        rijAlg.Padding = PaddingMode.Zeros;

        ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

        MemoryStream msEncrypt = new MemoryStream();
        CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

        byte[] toEncrypt = Encoding.ASCII.GetBytes(text);

        csEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
        csEncrypt.FlushFinalBlock();


        byte[] encrypted = msEncrypt.ToArray();

        return Convert.ToBase64String(encrypted);
    }



    public static string Decrypt(string text)
    {
        try
        {
            RijndaelManaged dijAlg = new RijndaelManaged();
            dijAlg.BlockSize = 256;
            dijAlg.Key = Convert.FromBase64String(GameManager.key);
            dijAlg.IV = Convert.FromBase64String(GameManager.iv);
            dijAlg.Padding = PaddingMode.Zeros;

            // Create a decrytor to perform the stream transform.
            ICryptoTransform decryptor = dijAlg.CreateDecryptor(dijAlg.Key, dijAlg.IV);



            byte[] encrypted = Convert.FromBase64String(text);


            MemoryStream msDecrypt = new MemoryStream(encrypted);

            CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

            byte[] fromEncrypt = new byte[encrypted.Length];

            csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

            return Encoding.ASCII.GetString(fromEncrypt).Trim('\0');
        }
        catch (Exception)
        {
            return "DECRYPTION ERROR";
        }
    }

}
