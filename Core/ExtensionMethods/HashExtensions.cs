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
}
