using Brupper.Maui.Services.Interfaces;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Brupper.Maui.Platforms.Windows.Services;

public class ImageResizer : IImageResizer
{
    public const int IMAGE_QUALITY = 50;

    public byte[] Resize(byte[] imageData, float width, float height, int imageQuality = IMAGE_QUALITY)
    {
        // Load the bitmap
        var img = Image.FromStream(new MemoryStream(imageData));
        Bitmap originalImage = new Bitmap(img);

        float oldWidth = (float)originalImage.Width;
        float oldHeight = (float)originalImage.Height;
        float scaleFactor = 0f;

        if (oldWidth > oldHeight)
        {
            scaleFactor = width / oldWidth;
        }
        else
        {
            scaleFactor = height / oldHeight;
        }

        float newHeight = oldHeight * scaleFactor;
        float newWidth = oldWidth * scaleFactor;

        using (Bitmap resizedImage = new Bitmap(originalImage, new Size((int)newWidth, (int)newHeight)))
        using (MemoryStream ms = new MemoryStream())
        {
            resizedImage.Save(ms, ImageFormat.Jpeg);
            return ms.ToArray();
        }

    }

    public static Image ResizeImage(Image img, SizeF targetSize)
    {
        float scale = Math.Min(targetSize.Width / img.Width, targetSize.Height / img.Height);
        Size newSize = new Size((int)(img.Width * scale) - 1, (int)(img.Height * scale) - 1);
        return new Bitmap(img, newSize);
    }
}