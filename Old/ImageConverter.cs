using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace ImageProcessor
{
    public class ImageConverter
    {
        string source = @"C:\TestImage\Source\colorImagesGrayscale.tif";
        string target = @"C:\TestImage\target\convertMultipleImagesIntoSingleOne.tif";
        string filePath = @"c:\TestImage\Source\RandomImages\";
        ImageFormat imageFormat = ImageFormat.Tiff;



        //public void Load()
        //{
        //    var fileGetter = new FileGetter();
        //    fileGetter.FileDirectory = filePath;
        //    foreach (var fileName in fileGetter.GetFiles())
        //    {
                
        //    }
        //}

        //not ready for prime time
        public void CombineImages()
        {
            DirectoryInfo directory=new DirectoryInfo(filePath);
            FileInfo[] files = directory.GetFiles();

            string finalImage = target;
            List<int> imageWidths = new List<int>();
            int nIndex = 0;
            int height = 0;
            foreach (FileInfo file in files)
            {
                Image img = Image.FromFile(file.FullName);
                imageWidths.Add(img.Width);
                height += img.Height;
                img.Dispose();
            }
            imageWidths.Sort();
            int width = imageWidths[imageWidths.Count - 1];
            Bitmap img3 = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(img3);
            g.Clear(SystemColors.AppWorkspace);
            foreach (FileInfo file in files)
            {
                Image img = Image.FromFile(file.FullName);
                var pageCount = img.GetFrameCount(FrameDimension.Page);

                if (pageCount == 1)
                {
                    if (nIndex == 0)
                    {
                        g.DrawImage(img, new Point(0, 0));
                        nIndex++;
                        height = img.Height;
                    }
                    else
                    {
                        g.DrawImage(img, new Point(0, height));
                        height += img.Height;
                    }
                }

                else
                {
                    for (int x = 0; x < pageCount; x++)
                    {
                        img.SelectActiveFrame(FrameDimension.Page, x);
                        if (nIndex == 0)
                        {
                            g.DrawImage(img, new Point(0, 0));
                            nIndex++;
                            height = img.Height;
                        }
                        else
                        {
                            g.DrawImage(img, new Point(0, height));
                            height += img.Height;
                        }
                    }

                }               
                
                img.Dispose();
            }
            g.Dispose();
            img3.Save(finalImage, System.Drawing.Imaging.ImageFormat.Tiff);
            img3.Dispose();
            //imageLocation.Image = Image.FromFile(finalImage);
        }


        // this function is good for compressing black and white images.
        //it does not appear to work as well as convertToGrayscale() in terms of 
        //keeping the picture quality high and yet reducing the image size
        public void CompressTiff() 
        {
            Bitmap bitmap = (Bitmap)Image.FromFile(source);

            MemoryStream byteStream = new MemoryStream();
            bitmap.Save(byteStream, imageFormat);

            Image tiff = Image.FromStream(byteStream);

            ImageCodecInfo encoderInfo = GetEncoder(imageFormat);

            //EncoderParameters EncoderParams;
            //EncoderParameter SaveEncodeParam;
            //EncoderParameter CompressionEncodeParam;
            //GetEncoderParams(out EncoderParams, out SaveEncodeParam, out CompressionEncodeParam);
            //EncoderParams.Param[0] = CompressionEncodeParam;
            //EncoderParams.Param[1] = SaveEncodeParam;

            EncoderParameters EncoderParams = new EncoderParameters(2);
            EncoderParameter parameter = new EncoderParameter(
                Encoder.Compression, (long)EncoderValue.CompressionCCITT4);
            EncoderParams.Param[0] = parameter;
            parameter = new EncoderParameter(Encoder.SaveFlag,
                (long)EncoderValue.MultiFrame);
            EncoderParams.Param[1] = parameter;
            
            tiff.Save(target, encoderInfo, EncoderParams);

            var pageCount = bitmap.GetFrameCount(FrameDimension.Page);
            for (int i = 1; i < pageCount; i++)
            {
                Image tiffImg = getTiffImage(bitmap, i);
       
                EncoderParameters encoderParams = new EncoderParameters(2);
                EncoderParameter saveEncodeParam = new EncoderParameter
                    (Encoder.SaveFlag,
                     (long)EncoderValue.FrameDimensionPage);
                EncoderParameter compressionEncodeParam = new EncoderParameter(
                     Encoder.Compression, (long)EncoderValue.CompressionCCITT4);
                encoderParams.Param[0] = compressionEncodeParam;
                encoderParams.Param[1] = saveEncodeParam;
                //EncoderParameters encoderParams;
                //EncoderParameter saveEncodeParam;
                //EncoderParameter compressionEncodeParam;
                //GetEncoderParams(out encoderParams, out saveEncodeParam, out compressionEncodeParam);
                tiff.SaveAdd(tiffImg, encoderParams);
            }
            tiff.Dispose();
            bitmap.Dispose();
        }


        public void SaveImages()
        {               
            Bitmap bitMap = (Bitmap)Image.FromFile(source);

            ImageCodecInfo info = null;
            foreach (ImageCodecInfo ici in ImageCodecInfo.GetImageEncoders())
            {
                if (ici.MimeType == "image/tiff")
                    info = ici;
            }
            Encoder enc = Encoder.SaveFlag;
            EncoderParameters ep = new EncoderParameters(1);
            ep.Param[0] = new EncoderParameter(enc, (long)EncoderValue.MultiFrame);
            Bitmap pages = null;
            var frame = 0;
            var pageCount = bitMap.GetFrameCount(FrameDimension.Page);
            for (int i = 0; i < pageCount; i++)
            {

                if (i == 0)
                {
                    bitMap.SelectActiveFrame(FrameDimension.Page, i);
                    //pages = bitMap;

                    pages = ConvertToGrayScale(bitMap);

                    pages.Save(target, info, ep);
                }
                else
                {                       
                    ep.Param[0] = new EncoderParameter(enc, (long)Convert.ToDouble(EncoderValue.FrameDimensionPage));
                    bitMap.SelectActiveFrame(FrameDimension.Page, i);
                    //Bitmap bm = bitMap;

                    Bitmap bm = ConvertToGrayScale(bitMap);

                    //Bitmap bm = CompressImage(tmpBM);

                    pages.SaveAdd(bm, ep);
                }
                if (i == pageCount - 1)
                {
                    ep.Param[0] = new EncoderParameter(enc, (long)Convert.ToDouble(EncoderValue.Flush));
                    pages.SaveAdd(ep);
                }
            }         
        }

        public Bitmap CompressImage(Bitmap bitMap)
        {
            var pageCount = bitMap.GetFrameCount(FrameDimension.Page);
            Image tiffImg = null;
            for (int i = 0; i < pageCount; i++)
            {
                tiffImg = getTiffImage(bitMap, i);

                EncoderParameters encoderParams = new EncoderParameters(2);
                EncoderParameter saveEncodeParam = new EncoderParameter(
                     Encoder.SaveFlag,
                     (long)EncoderValue.FrameDimensionPage);
                EncoderParameter compressionEncodeParam = new EncoderParameter(
                     Encoder.Compression, (long)EncoderValue.CompressionCCITT4);
                encoderParams.Param[0] = compressionEncodeParam;
                encoderParams.Param[1] = saveEncodeParam;                
            }
            return (Bitmap)tiffImg;
        }


        // this function is good for converting color images to grayscale
        public Bitmap ConvertToGrayScale(Bitmap bitMap) 
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(bitMap.Width, bitMap.Height);

            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);

            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][] 
              {
                 new float[] {.3f, .3f, .3f, 0, 0},
                 new float[] {.59f, .59f, .59f, 0, 0},
                 new float[] {.11f, .11f, .11f, 0, 0},
                 new float[] {0, 0, 0, 1, 0},
                 new float[] {0, 0, 0, 0, 1}
              });

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(bitMap, new System.Drawing.Rectangle(0, 0, bitMap.Width, bitMap.Height),
               0, 0, bitMap.Width, bitMap.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();
            return newBitmap;
        }


        public void SaveImage(int maxWidth, int maxHeight, int quality, ImageFormat imageFormat, Bitmap image)
        {
            //Bitmap image = new Bitmap(rootPath);

            // Get the image's original width and height
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            // To preserve the aspect ratio
            float ratioX = (float)maxWidth / (float)originalWidth;
            float ratioY = (float)maxHeight / (float)originalHeight;
            float ratio = Math.Min(ratioX, ratioY);

            // New width and height based on aspect ratio
            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);

            // Convert other formats (including CMYK) to RGB.
            Bitmap newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);

            // Draws the image in the specified size with quality mode set to HighQuality
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            // Get an ImageCodecInfo object that represents the JPEG codec.
            ImageCodecInfo imageCodecInfo = this.GetEncoder(imageFormat);

            // Create an Encoder object for the Quality parameter.
            System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;

            // Create an EncoderParameters object. 
            EncoderParameters encoderParameters = new EncoderParameters(1);

            // Save the image as a JPEG file with quality level.
            EncoderParameter encoderParameter = new EncoderParameter(encoder, quality);
            encoderParameters.Param[0] = encoderParameter;
            newImage.Save(target, imageCodecInfo, encoderParameters);
        }

        private static void GetEncoderParams(out EncoderParameters EncoderParams, out EncoderParameter SaveEncodeParam, out EncoderParameter CompressionEncodeParam)
        {
            EncoderParams = new EncoderParameters(2);
            SaveEncodeParam = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.FrameDimensionPage);
            CompressionEncodeParam = new EncoderParameter(
                Encoder.Compression, (long)EncoderValue.CompressionCCITT4);
        }

        public Image getTiffImage(Image sourceImage, int pageNumber)
        {
            MemoryStream ms = null;
            Image returnImage = null;

            try
            {
                ms = new MemoryStream();
                Guid objGuid = sourceImage.FrameDimensionsList[0];
                FrameDimension objDimension = new FrameDimension(objGuid);
                sourceImage.SelectActiveFrame(objDimension, pageNumber);
                sourceImage.Save(ms, ImageFormat.Tiff);
                returnImage = Image.FromStream(ms);
            }
            catch (Exception ex)
            {
                returnImage = null;
            }
            return returnImage;
        }


        private ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}
