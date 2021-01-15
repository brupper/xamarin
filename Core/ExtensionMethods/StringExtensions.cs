using System;
using System.IO;
using System.Security.Cryptography;
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
}
