using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class HashExtensions
{
    public static string CreateHashFromFile(this string path)
    {
        if (File.Exists(path))
        {
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return stream.CreateHashFromStream();
            }
        }
        return string.Empty;
    }

    public static string CreateHashFromStream(this Stream stream)
    {
        using (var hashAlgorithm = MD5.Create())
        {
            var result = hashAlgorithm.ComputeHash(stream);
            return result.ToHexString();
        }
    }

    public static string ToHexString(this byte[] bytes)
    {
        var sb = new StringBuilder();
        foreach (byte b in bytes)
        {
            sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }

    public static string GetSha1(this string literalToHash)
    {
        using (var algorithm = SHA1.Create())
        {
            var hash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(literalToHash));
            return hash.GetStringFromHash();
        }
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
