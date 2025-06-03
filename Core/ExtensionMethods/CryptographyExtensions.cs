using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Brupper;

public static class HashHelper
{
    public static string GetSha512(this string literalToHash)
    {
        if (string.IsNullOrEmpty(literalToHash)) return string.Empty;

        using (var algorithm = SHA512.Create())
        {
            var hash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(literalToHash));
            return hash.GetStringFromHash();
        }
    }

    //public static string NormalizeHashedValue(this string other)
    //    => other?.Trim()?.ToUpper() ?? string.Empty;

    public static string GetSha256(this string literalToHash)
    {
        using (var algorithm = SHA256.Create())
        {
            var hash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(literalToHash));
            return hash.GetStringFromHash();
        }
    }

    public static string GetSha1(this string literalToHash)
    {
        using (var algorithm = SHA1.Create())
        {
            var hash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(literalToHash));
            return hash.GetStringFromHash();
        }
    }

    public static byte[] EncryptStringToBytes_Aes(this string plainText, byte[] Key, byte[] IV)
    {
        // Check arguments.
        if (plainText == null || plainText.Length <= 0)
            throw new ArgumentNullException("plainText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");
        byte[] encrypted;

        // Create an Aes object
        // with the specified key and IV.
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            // Create an encryptor to perform the stream transform.
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for encryption.
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        //Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        // Return the encrypted bytes from the memory stream.
        return encrypted;

        //return System.Text.Encoding.UTF8.GetString(encrypted, 0, encrypted.Length);
    }

    public static string Aes128Encrypt(this string plainText, string key)
    {
        return Convert.ToBase64String(EncryptAes128(Encoding.UTF8.GetBytes(plainText), GetRijndaelManaged(key)));
    }

    public static string Aes128Decrypt(this string encryptedText, string key)
    {
        try
        {
            return Encoding.UTF8.GetString(DecryptAes128(Convert.FromBase64String(encryptedText), GetRijndaelManaged(key)));
        }
        catch (CryptographicException)
        {
            return null;
        }
    }

    private static byte[] EncryptAes128(byte[] plainBytes, RijndaelManaged rijndaelManaged)
    {
        return rijndaelManaged.CreateEncryptor().TransformFinalBlock(plainBytes, 0, plainBytes.Length);
    }

    private static byte[] DecryptAes128(byte[] encryptedData, RijndaelManaged rijndaelManaged)
    {
        return rijndaelManaged.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
    }

    private static RijndaelManaged GetRijndaelManaged(string secretKey)
    {
        byte[] numArray = new byte[16];
        byte[] bytes = Encoding.UTF8.GetBytes(secretKey);
        Array.Copy((Array)bytes, (Array)numArray, Math.Min(numArray.Length, bytes.Length));
        var rijndaelManaged = new RijndaelManaged();
        rijndaelManaged.Mode = CipherMode.ECB;
        rijndaelManaged.Padding = PaddingMode.PKCS7;
        rijndaelManaged.KeySize = 128;
        rijndaelManaged.BlockSize = 128;
        rijndaelManaged.Key = numArray;
        rijndaelManaged.IV = numArray;
        return rijndaelManaged;
    }

    public static string DecryptStringFromBytes_Aes(this byte[] cipherText, byte[] Key, byte[] IV)
    {
        // Check arguments.
        if (cipherText == null || cipherText.Length <= 0)
            throw new ArgumentNullException("cipherText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");

        // Declare the string used to hold
        // the decrypted text.
        string plaintext = null;

        // Create an AesManaged object
        // with the specified key and IV.
        using (var aesAlg = new AesManaged())
        {
            aesAlg.Mode = CipherMode.ECB;
            aesAlg.Padding = PaddingMode.PKCS7;
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            // Create a decryptor to perform the stream transform.
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for decryption.
            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        // Read the decrypted bytes from the decrypting stream
                        // and place them in a string.
                        plaintext = srDecrypt.ReadToEnd();
                        plaintext = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(plaintext));
                    }
                }
            }
        }

        return plaintext;
    }

    private static string GetStringFromHash(this byte[] hash)
    {
        var result = new StringBuilder();
        for (int i = 0; i < hash.Length; i++)
        {
            result.Append(hash[i].ToString("X2"));
        }
        return result.ToString();
    }

}
