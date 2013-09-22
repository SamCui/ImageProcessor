using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImageProcessor.Interfaces;
using ImageMagick;
using System.Drawing;
using System.IO;
using System.Diagnostics;

namespace ImageProcessor
{
    public class ImageMagickGrayscaleTransformer: IGrayscaleTransformer
    {
        IFileSaver _fileSaver = null;
        public ImageMagickGrayscaleTransformer(IFileSaver fileSaver)
        {
            _fileSaver = fileSaver;
        }

        public void ConvertToGrayScale(string SourceFileName, string DestFileName)
        {
            List<Bitmap> bitMaps = new List<Bitmap>();

            using (MagickImageCollection images = new MagickImageCollection())
            {
                MagickReadSettings settings = new MagickReadSettings();
                images.Read(SourceFileName, settings);
                settings.FrameIndex = 0; // First page
                settings.FrameCount = images.Count; // Number of pages
                int count = images.Count;
               
                foreach (MagickImage image in images)
                {
                    image.ColorType = ColorType.Grayscale;
                    image.Quantize();

                    var bm = image.ToBitmap();
                    bitMaps.Add(bm);

                    //image.Write(DestFileName + count.ToString() + ".tif"); 

                    ++count;
                }
            }

            _fileSaver.TargetFileName = DestFileName;
            _fileSaver.SaveImages(bitMaps, DestFileName);

            //Process proc = new Process
            //{
            //    StartInfo = new ProcessStartInfo

            //    {
            //        FileName = @"C:\Program Files\ImageMagick-6.8.6-Q16\convert.exe",
            //        Arguments = @"c:\testimage\target\onegrayscaletiff\*.tif c:\testimage\target\onegrayscaletiff\"+Guid.NewGuid().ToString() +".tif",
            //        UseShellExecute = false,
            //        RedirectStandardError = true,
            //        CreateNoWindow = true
            //    }
            //};

            //proc.Start();
        }
    }
}
