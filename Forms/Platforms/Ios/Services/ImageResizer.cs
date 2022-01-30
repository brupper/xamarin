using Brupper.Forms.Services.Interfaces;
using CoreGraphics;
using System;
using System.Drawing;
using UIKit;

namespace Brupper.Forms.Platforms.iOS.Services
{
    public class ImageResizer : IImageResizer
    {
        public const int IMAGE_QUALITY = 50;

        public byte[] Resize(byte[] imageData, float width, float height, int imageQuality = IMAGE_QUALITY)
        {
            UIImage originalImage = ImageFromByteArray(imageData);


            UIKit.UIImage resizedImage = Resize(originalImage, width, height, imageQuality);

            // save the image as a jpeg
            return resizedImage.AsJPEG(imageQuality).ToArray();
        }

        public UIImage Resize(UIImage originalImage, float width, float height, int imageQuality = IMAGE_QUALITY)
        {
            float oldWidth = (float)originalImage.Size.Width;
            float oldHeight = (float)originalImage.Size.Height;
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

            //create a 24bit RGB image
            using (CGBitmapContext context = new CGBitmapContext(IntPtr.Zero,
                (int)newWidth, (int)newHeight, 8,
                (int)(4 * newWidth), CGColorSpace.CreateDeviceRGB(),
                CGImageAlphaInfo.PremultipliedFirst))
            {

                RectangleF imageRect = new RectangleF(0, 0, newWidth, newHeight);

                // draw the image
                context.DrawImage(imageRect, originalImage.CGImage);

                UIKit.UIImage resizedImage = UIKit.UIImage.FromImage(context.ToImage());

                return resizedImage;
            }
        }

        public UIImage ImageFromByteArray(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            UIImage image;
            try
            {
                image = new UIImage(Foundation.NSData.FromArray(data));
            }
            catch
            {
                return null;
            }
            return image;
        }
    }
}