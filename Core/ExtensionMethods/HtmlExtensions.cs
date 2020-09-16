using System;
using System.IO;

public static class HtmlExtensions
{
    public static string MakeImageSrcData(this string filename)
    {
        if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
        {
            return "data:image/png;base64," + filename; // probably already a base64 string
        }

        using (var fs = new FileStream(filename, FileMode.Open))
        {
            return MakeImageSrcData(fs);
        }
    }

    public static string MakeImageSrcData(this Stream fs)
    {
        var filebytes = new byte[fs.Length];
        fs.Read(filebytes, 0, Convert.ToInt32(fs.Length));
        return "data:image/png;base64," + Convert.ToBase64String(filebytes, Base64FormattingOptions.None);
    }
}

