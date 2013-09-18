using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using ImageProcessor.Interfaces;

namespace ImageProcessor
{
    public class DotnetImageTransformer: IDotnetImageTransformer
    {
        //ConvertToGraySCale functions is effective in reducing size of color images
    
        public List<Bitmap> ConvertToGrayScale(Bitmap bitMap, string fileName, float percentOfOriginal)
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

        #region private methods
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

        #region not used
        // this function is not capable of processing multiframe images
        public Bitmap ConvertToGrayScaleSingle(Bitmap bitMap)
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


        //this function is capable of processing multiframe bitmap
        public List<Bitmap> ConvertToGrayScale(Bitmap bitMap)
        {
            List<Bitmap> bitMaps = new List<Bitmap>();
            var pageCount = bitMap.GetFrameCount(FrameDimension.Page);

            for (int i = 0; i < pageCount; i++)
            {
                Bitmap newBitmap = new Bitmap(bitMap.Width, bitMap.Height);
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
                g.DrawImage(bitMap, new System.Drawing.Rectangle(0, 0, bitMap.Width, bitMap.Height),
                   0, 0, bitMap.Width, bitMap.Height, GraphicsUnit.Pixel, attributes);

                //dispose the Graphics object
                g.Dispose();
                bitMaps.Add(newBitmap);
            }

            return bitMaps;
        }

        private Image getTiffImage(Image sourceImage, int pageNumber)
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
        #endregion
    }
}
