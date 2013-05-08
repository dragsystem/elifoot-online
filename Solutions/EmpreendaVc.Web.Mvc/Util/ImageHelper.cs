using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace EmpreendaVc.Web.Mvc.Util
{
    public class ImageHelper
    {
        /// <summary>
        /// Find the right codec
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static ImageCodecInfo GetImageCodec(string extension) {
            extension = extension.ToUpperInvariant();
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs) {
                if (codec.FilenameExtension.Contains(extension)) {
                    return codec;
                }
            }
            return codecs[1];
        }

        /// <summary>
        /// Images to byte array.
        /// </summary>
        /// <param name="imageIn">The image in.</param>
        /// <returns></returns>
        public static byte[] ImageToByteArray(System.Drawing.Image image) {
            return ImageToByteArray(image, null, null);
        }

        /// <summary>
        /// Images to byte array.
        /// </summary>
        /// <param name="image">The image in.</param>
        /// <param name="extension">The extension.</param>
        /// <param name="encoderParameters">The encoder parameters.</param>
        /// <returns></returns>
        public static byte[] ImageToByteArray(System.Drawing.Image image, string extension, EncoderParameters encoderParameters) {
            MemoryStream ms = new MemoryStream();

            Size newSize = CalculateDimensions(image.Size, 535);

            using (Bitmap newImage = new Bitmap(newSize.Width, newSize.Height, PixelFormat.Format32bppRgb)) {
                newImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
                using (Graphics canvas = Graphics.FromImage(newImage)) {
                    canvas.SmoothingMode = SmoothingMode.AntiAlias;
                    canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    canvas.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    canvas.DrawImage(image, new Rectangle(new Point(0, 0), newSize));
                    
                    if (!string.IsNullOrEmpty(extension) && encoderParameters != null) {
                        newImage.Save(ms, GetImageCodec(extension), encoderParameters);
                    }
                    else {
                        newImage.Save(ms, image.RawFormat);
                    }
                }
            }

           

            return ms.ToArray();

        }


        public static void ImageToNewDimensions(System.Drawing.Image image, string imageFolder, int targetSize) {
            MemoryStream ms = new MemoryStream();

            Size newSize = CalculateDimensions(image.Size, targetSize);

            using (Bitmap newImage = new Bitmap(newSize.Width, newSize.Height, PixelFormat.Format32bppRgb)) {
                newImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
                using (Graphics canvas = Graphics.FromImage(newImage)) {
                    canvas.SmoothingMode = SmoothingMode.AntiAlias;
                    canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    canvas.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    canvas.DrawImage(image, new Rectangle(new Point(0, 0), newSize));
                    

                    newImage.Save(imageFolder);
                }
                
            }

        }

        public static byte[] ResizeImageFile(byte[] imageFile, int targetSize) {
            using (System.Drawing.Image oldImage = System.Drawing.Image.FromStream(new MemoryStream(imageFile))) {
                Size newSize = CalculateDimensions(oldImage.Size, targetSize);

                using (Bitmap newImage = new Bitmap(newSize.Width, newSize.Height, PixelFormat.Format32bppRgb)) {
                    newImage.SetResolution(oldImage.HorizontalResolution, oldImage.VerticalResolution);
                    using (Graphics canvas = Graphics.FromImage(newImage)) {
                        canvas.SmoothingMode = SmoothingMode.AntiAlias;
                        canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        canvas.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        canvas.DrawImage(oldImage, new Rectangle(new Point(0, 0), newSize));
                        MemoryStream m = new MemoryStream();
                        newImage.Save(m, ImageFormat.Jpeg);
                        return m.GetBuffer();
                    }
                }

            }
        }

        public static Size CalculateDimensions(Size oldSize, int targetSize) {
            Size newSize = new Size();
            if (oldSize.Width > oldSize.Height) {
                newSize.Width = targetSize;
                newSize.Height = (int)(oldSize.Height * (float)targetSize / (float)oldSize.Width);
            }
            else {
                newSize.Width = (int)(oldSize.Width * (float)targetSize / (float)oldSize.Height);
                newSize.Height = targetSize;
            }
            return newSize;
        } 

        /// <summary>
        /// Bytes the array to image.
        /// </summary>
        /// <param name="byteArrayIn">The byte array in.</param>
        /// <returns></returns>
        public static Image ByteArrayToImage(byte[] byteArrayIn) {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
    }
}