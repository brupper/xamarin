using Brupper.Forms.Services.Interfaces;
using global::Android.Graphics;
using System.IO;

namespace Brupper.Forms.Platforms.Android.Services
{
    public class ImageResizer : IImageResizer
    {
        public const int IMAGE_QUALITY = 50;

        public byte[] Resize(byte[] imageData, float width, float height, int imageQuality = IMAGE_QUALITY)
        {
            // Load the bitmap
            Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);

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

            Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)newWidth, (int)newHeight, false);

            using (MemoryStream ms = new MemoryStream())
            {
                resizedImage.Compress(Bitmap.CompressFormat.Jpeg, imageQuality, ms);
                return ms.ToArray();
            }
        }
    }
}