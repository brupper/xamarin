using System;
using System.Globalization;
using System.IO;
using System.Text;

public static class StringExtensions
{
    //   - The following reserved characters:
    //
    //    < (less than)
    //    > (greater than)
    //    : (colon)
    //    " (double quote)
    //    / (forward slash)
    //    \ (backslash)
    //    | (vertical bar or pipe)
    //    ? (question mark)
    //    * (asterisk)
    public static string MakeValidFileName(this string name)
    {
        var illegal = "\"\\()/<>;:*|?"; //<>:"/\|?*
        var invalidChars = System.Text.RegularExpressions.Regex.Escape(illegal + (new string(Path.GetInvalidFileNameChars()) + (new string(Path.GetInvalidPathChars()))));
        var invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

        return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
    }

    private static char[] nonDiacritics = { 'a', 'e', 'i', 'o', 'ö', 'ú', 'ü' };
    private static char[] diacritics = { 'á', 'é', 'í', 'ó', 'ő', 'ú', 'ű' };

    public static string Remove(this string str)
    {
        for (int i = 0; i < str.Length; i++)
        {
            for (int j = 0; j < diacritics.Length; j++)
            {
                if (str[i] == diacritics[j])
                    str.Replace(diacritics[j], nonDiacritics[j]);
            }
        }
        return str;
    }

    public static string RemoveDiacritics(this string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        for (int ich = 0; ich < normalizedString.Length; ich++)
        {
            var uc = CharUnicodeInfo.GetUnicodeCategory(normalizedString[ich]);
            if (uc == UnicodeCategory.UppercaseLetter || uc == UnicodeCategory.LowercaseLetter || uc == UnicodeCategory.DashPunctuation)
            // if (unicodeCategory != UnicodeCategory.NonSpacingMark) 
            {
                sb.Append(normalizedString[ich]);
            }
        }
        return (sb.ToString().Normalize(NormalizationForm.FormC));
    }

    public static string Base64Encode(this string plainText)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }

    public static string Base64Decode(this string base64EncodedData)
    {
        var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
        return Encoding.UTF8.GetString(base64EncodedBytes);
    }
}
