using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace Sparkle.LinkedInNET.DemoMvc5.Utils
{
    public static class ImageUtils
    {
        private const int DEFAULT_JPEG_QUALITY = 96;
        public static readonly int PXLS_360p = 640 * 360;
        public static readonly int PXLS_540p = 960 * 540;
        public static readonly int PXLS_720p = 1280 * 720;
        public static readonly int PXLS_1080p = 1920 * 1080;
        public static readonly int PXLS_4K = 3840 * 2160;
        public static readonly int PXLS_8K = 7680 * 4320;

        public static readonly ImageCodecInfo JPEG_ENCODER = GetEncoderInfo("image/jpeg");

        private static readonly ImageConverter imageConverter = new ImageConverter();

        public static string ExtractFileExtensionFromName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return string.Empty;
            try
            {
                //regex matcher doesn't work for long extensions like .pages or .numbers
                //return Regex.Match(fileName, "[.](?<fileExtension>\\w{1,4}\\z)").Groups["fileExtension"].Value;
                return Path.GetExtension(fileName).Replace(".", "");
            }
            catch (ArgumentException)
            {
                //Invalid char detected, fallback
                return fileName.Substring(fileName.LastIndexOf(".") + 1);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static ImageFormat ImageFormatFromFileExtension(string extension)
        {
            switch (extension.ToLower())
            {
                case "bmp":
                    return ImageFormat.Bmp;

                case "emf":
                    return ImageFormat.Emf;

                case "exif":
                    return ImageFormat.Exif;

                case "gif":
                    return ImageFormat.Gif;

                case "icon":
                    return ImageFormat.Icon;

                case "jpeg":
                    return ImageFormat.Jpeg;

                case "jpg":
                    return ImageFormat.Jpeg;

                case "png":
                    return ImageFormat.Png;

                case "tiff":
                    return ImageFormat.Tiff;

                case "wmf":
                    return ImageFormat.Wmf;

                default: throw new NotSupportedException(extension + " is not a supported image type");
            }
        }

        public static ImageFormat ImageFormatFromContentType(string contentType)
        {
            switch (contentType.ToLower())
            {
                case "image/jpg":
                case "image/jpeg":
                    return ImageFormat.Jpeg;

                case "image/gif":
                    return ImageFormat.Gif;

                case "image/png":
                case "image/x-png":
                    return ImageFormat.Png;

                case "image/bmp":
                    return ImageFormat.Bmp;

                case "image/tiff":
                    return ImageFormat.Tiff;

                default: throw new NotSupportedException(contentType + " is not a supported image content type");
            }
        }

        public static bool IsImageByExtension(string extension)
        {
            switch (extension.ToLower())
            {
                case "bmp":
                case "gif":
                case "ico":
                case "jpeg":
                case "jpg":
                case "png":
                case "tiff":
                    return true;

                default:
                    return false;
            }
        }

        public static ImageFormat ImageFormatFromFileName(string fileName)
        {
            return ImageFormatFromFileExtension(ExtractFileExtensionFromName(fileName));
        }

        public static bool IsImageExisting(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "HEAD";
            try
            {
                request.GetResponse();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// http://stackoverflow.com/questions/1922040/resize-an-image-c-sharp
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Image ResizeIfLargerThan(Image original, int maxPixels)
        {
            if (original.Width * original.Height <= maxPixels)
                return original;
            return Resize(original, maxPixels);
        }

        public static Image Resize(Image original, int maxPixels)
        {
            if (original.Width * original.Height <= maxPixels)
                return ResizeTo(original, original.Width, original.Height);
            // the factor by which each dimension will be scaled
            double factor = Math.Sqrt((double)maxPixels / (original.Width * original.Height));
            // the final image dimensions
            int w = (int)Math.Floor(original.Width * factor);
            int h = (int)Math.Floor(original.Height * factor);
            return ResizeTo(original, w, h);
        }

        public static Image ResizeTo(Image original, int w, int h)
        {
            var dst = new Bitmap(w, h);
            dst.SetResolution(original.HorizontalResolution, original.VerticalResolution);
            using (var graphics = Graphics.FromImage(dst))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(original, new Rectangle(0, 0, w, h), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return dst;
        }

        /// http://stackoverflow.com/questions/3801275/how-to-convert-image-in-byte-array
        public static byte[] ImageToByteArray(Image imageIn, ImageFormat format)
        {
            using (var ms = new MemoryStream())
            {
                var imgEncoder = ImageCodecInfo.GetImageEncoders().Where(x => x.FormatID == format.Guid).Single();
                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;

                // Create an EncoderParameters object.
                // An EncoderParameters object has an array of EncoderParameter
                // objects. In this case, there is only one
                // EncoderParameter object in the array.
                EncoderParameters myEncoderParameters = new EncoderParameters(1);

                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100L);
                myEncoderParameters.Param[0] = myEncoderParameter;
                imageIn.Save(ms, imgEncoder, myEncoderParameters);
                return ms.ToArray();
            }
        }

        #region reduce image size

        public static Bitmap ScaleImage(Image image, double scale)
        {
            int newWidth = (int)(image.Width * scale);
            int newHeight = (int)(image.Height * scale);

            var result = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);
            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var g = Graphics.FromImage(result))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                g.DrawImage(image, 0, 0, result.Width, result.Height);
            }
            return result;
        }

        public static MemoryStream DownscaleImage(Image img, long maxByteSize, byte quality = DEFAULT_JPEG_QUALITY)
        {
            var ms = new MemoryStream();
            ms.SetLength(0);
            img.SaveJpeg(ms, quality);

            while (ms.Length > maxByteSize)
            {
                var scale = Math.Sqrt((double)maxByteSize / ms.Length);
                img = ScaleImage(img, scale);
                ms.SetLength(0);
                img.SaveJpeg(ms, quality);
            }
            ms.Position = 0;
            return ms;
        }

        public static void SaveJpeg(this Image img, Stream target, byte quality = DEFAULT_JPEG_QUALITY)
        {
            var eps = new EncoderParameters(1)
            {
                Param =
                {
                    [0] = new EncoderParameter(Encoder.Quality, (long) quality)
                }
            };
            img.Save(target, JPEG_ENCODER, eps);
        }

        public static MemoryStream DownscaleImage(byte[] img, long maxByteSize)
        {
            var photo = (Bitmap)(new ImageConverter().ConvertFrom(img));
            try
            {
                return DownscaleImage(photo, maxByteSize);
            }
            finally
            {
                photo.Dispose();
            }
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static Bitmap ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            return newImage;
        }

        public static byte[] ImageToByteArray(Image imageIn)
        {
            return (byte[])imageConverter.ConvertTo(imageIn, typeof(byte[]));
        }

        public static Image Contain(Image image, int width, int height)
        {
            var result = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            using (var g = Graphics.FromImage(result))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                g.FillRectangle(Brushes.Transparent, new Rectangle(0, 0, width, height));

                var ratioX = (float)width / image.Width;
                var ratioY = (float)height / image.Height;
                // use whichever ratio is smaller
                var ratio = ratioX < ratioY ? ratioX : ratioY;

                // now we can get the new width, height, x and y
                var w = Convert.ToInt32(image.Width * ratio);
                var h = Convert.ToInt32(image.Height * ratio);
                var x = Convert.ToInt32((width - (image.Width * ratio)) / 2);
                var y = Convert.ToInt32((height - (image.Height * ratio)) / 2);

                g.DrawImage(image, x, y, w, h);
            }
            return result;
        }

        public static Image Cover(Image image, int width, int height)
        {
            var result = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            using (var g = Graphics.FromImage(result))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                g.FillRectangle(Brushes.Transparent, new Rectangle(0, 0, width, height));

                var ratioX = (float)width / image.Width;
                var ratioY = (float)height / image.Height;
                // use whichever ratio is larger
                var ratio = ratioX > ratioY ? ratioX : ratioY;

                // now we can get the new width, height, x and y
                var w = Convert.ToInt32(image.Width * ratio);
                var h = Convert.ToInt32(image.Height * ratio);
                var x = Convert.ToInt32((width - (image.Width * ratio)) / 2);
                var y = Convert.ToInt32((height - (image.Height * ratio)) / 2);

                g.DrawImage(image, x, y, w, h);
            }
            return result;
        }

        #endregion reduce image size

        public static async Task<byte[]> PrepareImageFromUrl(string url, long maxImgSize)
        {
            var webClient = new WebClient();
            var bytes = webClient.DownloadData(url);

            if (bytes.LongCount() > maxImgSize)
            {
                // reduce size
                var smallerImageStream = ImageUtils.DownscaleImage(bytes, maxImgSize);
                // convert back to byte[]
                bytes = ImageUtils.ReadFully(smallerImageStream);
            }

            return bytes;
        }

        public static byte[] DownladFromUrlToByte(string url)
        {
            HttpWebRequest req;
            HttpWebResponse res = null;

            try
            {
                req = (HttpWebRequest)WebRequest.Create(url);
                res = (HttpWebResponse)req.GetResponse();
                Stream stream = res.GetResponseStream();

                var buffer = new byte[4096];
                using (MemoryStream ms = new MemoryStream())
                {
                    var bytesRead = 0;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        ms.Write(buffer, 0, bytesRead);
                    }
                    return ms.ToArray();
                }
            }
            finally
            {
                if (res != null)
                    res.Close();
            }
        }
    }
}