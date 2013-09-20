using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using ImageProcessor.Helpers;
using ImageProcessor.Interfaces;

namespace ImageProcessor
{
    public class DotnetGrayscaleTransformer: IGrayscaleTransformer
    {
        IFileSaver _fileSaver = null;

        public DotnetGrayscaleTransformer(IFileSaver fileSaver)
        {
            _fileSaver = fileSaver;
        }

        public void ConvertToGrayScale(string SourceFileName, string DestFileName)
        {
            Bitmap bm = (Bitmap)Image.FromFile(SourceFileName);

            List<Bitmap> bitMaps = ConvertToGrayScale(bm, DestFileName, ResizeScale.HundredPercent);

            _fileSaver.TargetFileName = DestFileName;
            _fileSaver.SaveImages(bitMaps, DestFileName);
        }

        #region private methods
        private List<Bitmap> ConvertToGrayScale(Bitmap bitMap, string fileName, float percentOfOriginal)
        {
            List<Bitmap> bitMaps = new List<Bitmap>();

            var ext = fileName.Substring(fileName.LastIndexOf(".") + 1);
            if (ext.ToLower() != "gif")
            {
                ConvertToGrayScaleMultiplePage(bitMap, bitMaps, percentOfOriginal);
            }
            else
            {
                ConvertToGrayScaleSinglePage(bitMap, bitMaps, percentOfOriginal);
            }

            return bitMaps;
        }

        private static void ConvertToGrayScaleSinglePage(Bitmap bitMap, List<Bitmap> bitMaps, float percentOfOriginal)
        {
            var destWidth = Convert.ToInt32((bitMap.Width) * percentOfOriginal);
            var destHeight = Convert.ToInt32((bitMap.Height) * percentOfOriginal);

            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(destWidth, destHeight);

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
            g.DrawImage(bitMap, new System.Drawing.Rectangle(0, 0, destWidth, destHeight),
               0, 0, destWidth, destHeight, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();

            bitMaps.Add(newBitmap);
        }

        private static void ConvertToGrayScaleMultiplePage(Bitmap bitMap, List<Bitmap> bitMaps, float percentOfOriginal)
        {
            var pageCount = bitMap.GetFrameCount(FrameDimension.Page);

            var destWidth = Convert.ToInt32((bitMap.Width) * percentOfOriginal);
            var destHeight = Convert.ToInt32((bitMap.Height) * percentOfOriginal);

            for (int i = 0; i < pageCount; i++)
            {
                Bitmap newBitmap = new Bitmap(destWidth, destHeight);
                bitMap.SelectActiveFrame(FrameDimension.Page, i);
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
                g.DrawImage(bitMap, new System.Drawing.Rectangle(0, 0, destWidth, destHeight),
                   0, 0, destWidth, destHeight, GraphicsUnit.Pixel, attributes);

                //dispose the Graphics object
                g.Dispose();
                bitMaps.Add(newBitmap);
            }
        }
        #endregion
    }
}
