using System;
using System.Globalization;
using System.IO;
using System.Linq;
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

    public static Func<string, bool> CreateStringComparisonFilter(this string filterTextParam) =>  new Func<string, bool>(x =>
    {
        return !string.IsNullOrEmpty(x)
                && (x.IndexOf(filterTextParam, 0, StringComparison.CurrentCultureIgnoreCase) != -1
                || (string.Compare(x, filterTextParam, CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0));
    });

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

    /// <summary>
    /// This extension is very simple yet powerful. With this extension, you remove all characters from a text string that are not numeric. Very useful for when you want to get the phone number clean of characters, for example.
    /// "+1 (712) 412-5236" => 17124125236
    /// </summary>
    public static string JustNumberFormat(this string value)
    {
        return string.Join("", value.ToCharArray().Where(Char.IsDigit));
    }

    public static string AsAscii(this string originalString)
    {
        if (string.IsNullOrEmpty(originalString))
        {
            return string.Empty;
        }

        var asAscii = new StringBuilder();
        // store final ascii string and Unicode points
        foreach (var c in originalString)
        {
            // test if char is ascii, otherwise convert to Unicode Code Point
            var cint = Convert.ToInt32(c);
            if (cint is <= 127 and >= 0)
            {
                asAscii.Append(c);
            }
            else
            {
                asAscii.Append(string.Format("\\u{0:x4} ", cint).Trim());
            }
        }

        return asAscii.ToString();
    }

    public static double ConvertToDouble(this string value, double defaultValue)
    {
        double result;

        if (string.IsNullOrWhiteSpace(value))
        {
            return defaultValue;
        }

        //Try parsing in the current culture
        if (!double.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out result) &&
            //Then try in US english
            !double.TryParse(value, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out result) &&
            //Then in neutral language
            !double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
        {
            result = defaultValue;
        }

        //if (double.TryParse(value.Replace(",", ".")))

        return result;
    }
}
