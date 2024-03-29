﻿using Brupper.Forms.Models.Rendering;
using CoreGraphics;
using Foundation;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;

namespace Brupper.Forms.Platforms.iOS.Services
{
    /// <inheritdoc/>
    public class OutputRendererServices : Forms.Services.Concretes.AOutputRendererServices
    {
        /// <inheritdoc />
        public OutputRendererServices(IFileSystem fileSystem)
            : base(fileSystem) { }

        /// <inheritdoc />
        public override Task OpenPdfAsync(string filePath)
        {
            try
            {
                // ez itt nem OK await Launcher.OpenAsync(new OpenFileRequest { File = new ReadOnlyFile(filePath), });

                var url = NSUrl.FromFilename(filePath);
                var docControl = UIDocumentInteractionController.FromUrl(url);

                var window = UIApplication.SharedApplication.KeyWindow;
                docControl.Delegate = new DocInteractionControllerDelegate(window.RootViewController);
                docControl.PresentPreview(true);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                Debug.WriteLine(filePath);
                Microsoft.AppCenter.Crashes.Crashes.TrackError(e);
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override Task<string> SaveIntoPdfAsync(string html, string fileName, PaperKind kind, int numberOfPages = 1)
        {
            return InternalSave(html, fileName, kind, numberOfPages, (source, absolutePath) => new PdfExportWebViewCallBack(source, absolutePath, new PaperSize(kind)));
        }

        /// <inheritdoc />
        public override Task<string> SaveIntoPngAsync(string html, string fileName, PaperKind kind, int numberOfPages = 1)
        {
            return InternalSave(html, fileName, kind, numberOfPages, (source, absolutePath) => new PngExportWebViewCallBack(source, absolutePath, new PaperSize(kind)));
        }

        /// <inheritdoc />
        protected virtual Task<string> InternalSave(string htmlContent, string fileName, PaperKind kind, int numberOfPages, Func<TaskCompletionSource<string>, string, UIWebViewDelegate> webviewclientCreator)
        {
            var source = new TaskCompletionSource<string>();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    var w = 6.5;
                    var h = w * PaperSize.ExperimentalA4Ratio;
                    var webView = new UIWebView(new CGRect(0, 0, w * 72, h * 72));

                    try
                    {
                        var files = Directory.GetFiles(DocumentsFolder, $"*{fileName}*");
                        //if (files.Length > 1)
                        foreach (var item in files)
                        {
                            File.Delete(item);
                        }
                    }
                    catch (Exception e) { Microsoft.AppCenter.Crashes.Crashes.TrackError(e); }

                    var file = Path.Combine(DocumentsFolder, $"{DateTime.Now:yyyyMMdd}_{fileName}.png");

                    webView.Delegate = webviewclientCreator(source, file);
                    webView.ScalesPageToFit = true;
                    webView.UserInteractionEnabled = false;
                    webView.BackgroundColor = UIColor.White;
                    webView.LoadHtmlString(htmlContent, null);
                }
                catch (Exception e) { source.SetException(e); }
            });

            return source.Task;
        }

        // https://stackoverflow.com/questions/53505364/how-to-generate-the-thumbnail-of-the-pdf-first-page-in-xamarin-forms
        /// <inheritdoc />
        public void ConvertToImage(Stream pdfFileStream)
        {
            var stream = new MemoryStream();
            // Create memory stream from file stream.
            pdfFileStream.CopyTo(stream);
            // Create data provider from bytes.
            var provider = new CGDataProvider(stream.ToArray());

            //Load a PDF file.
            var m_pdfDcument = new CGPDFDocument(provider);

            UIImage pdfImage = null;

            //Get PDF's page and convert as image.
            using (CGPDFPage pdfPage = m_pdfDcument.GetPage(2))
            {
                //initialise image context.
                UIGraphics.BeginImageContext(pdfPage.GetBoxRect(CGPDFBox.Media).Size);
                // get current context.
                CGContext context = UIGraphics.GetCurrentContext();
                context.SetFillColor(1.0f, 1.0f, 1.0f, 1.0f);
                // Gets page's bounds.
                CGRect bounds = new CGRect(pdfPage.GetBoxRect(CGPDFBox.Media).X, pdfPage.GetBoxRect(CGPDFBox.Media).Y, pdfPage.GetBoxRect(CGPDFBox.Media).Width, pdfPage.GetBoxRect(CGPDFBox.Media).Height);
                if (pdfPage != null)
                {
                    context.FillRect(bounds);
                    context.TranslateCTM(0, bounds.Height);
                    context.ScaleCTM(1.0f, -1.0f);
                    context.ConcatCTM(pdfPage.GetDrawingTransform(CGPDFBox.Crop, bounds, 0, true));
                    context.SetRenderingIntent(CGColorRenderingIntent.Default);
                    context.InterpolationQuality = CGInterpolationQuality.Default;
                    // Draw PDF page in the context.
                    context.DrawPDFPage(pdfPage);
                    // Get image from current context.
                    pdfImage = UIGraphics.GetImageFromCurrentImageContext();
                    UIGraphics.EndImageContext();
                }
            }
            // Get bytes from UIImage object.
            using (var imageData = pdfImage.AsPNG())
            {
                //imageBytes = new byte[imageData.Length];
                //System.Runtime.InteropServices.Marshal.Copy(imageData.Bytes, imageBytes, 0, Convert.ToInt32(imageData.Length));
                //return bytes;

                //Create image from bytes.
                //imageStream = new MemoryStream(imageBytes);
                //Save the image. It is a custom method to save the image
                //Save("PDFtoImage.png", "image/png", imageStream);
            }
        }
    }
}