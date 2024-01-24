﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Brupper;

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

    private static string GetStringFromHash(this byte[] hash)
    {
        var result = new StringBuilder();
        for (int i = 0; i < hash.Length; i++)
        {
            result.Append(hash[i].ToString("X2"));
        }
        return result.ToString();
    }
	
	    /// <summary> Compute the file's hash. </summary>
    public static string GetHashSha256OfFile(this string filename) => filename.ComputeHashSha256OfFile().BytesToString();

    /// <summary> Compute the file's hash. </summary>
    private static byte[] ComputeHashSha256OfFile(this string filename)
    {
        using (var stream = File.OpenRead(filename))
        {
            // The cryptographic service provider.
            var sha256 = SHA256.Create();

            return sha256.ComputeHash(stream);
        }
    }

    /// <summary> Return a byte array as a sequence of hex values. </summary>
    private static string BytesToString(this byte[] bytes)
    {
        var result = "";
        foreach (byte b in bytes) result += b.ToString("x2");
        return result;
    }

    public static bool NullOrEmpty(this string value)
    {
        return string.IsNullOrEmpty(value);
    }
    public static bool NotNullOrEmpty(this string value)
    {
        return !string.IsNullOrEmpty(value);
    }
    public static bool NullOrWhiteSpace(this string value)
    {
        return string.IsNullOrWhiteSpace(value);
    }
    public static bool NotNullOrWhiteSpace(this string value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }
}
