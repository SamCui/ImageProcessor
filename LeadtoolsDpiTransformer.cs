using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImageProcessor.Interfaces;
using Leadtools;
using Leadtools.Codecs;
using Leadtools.ImageProcessing;

namespace ImageProcessor
{
    public class LeadtoolsDpiTransformer: IDpiTransformer
    {
        public void ConvertDpi(string srcFileName, string destFileName)
        {
            RasterCodecs codecs = new RasterCodecs();

            RasterImage image = codecs.Load(srcFileName);

            int newResolution = 300; //BR says all files need to be 300 DPI. This should probably be an enum.

            image.XResolution = newResolution;
            image.YResolution = newResolution;
            SizeCommand command = new SizeCommand();
            command.Width = image.Width;
            command.Height = image.Height;
            command.Flags = RasterSizeFlags.Resample;
            command.Run(image);

            codecs.Save(image, destFileName, RasterImageFormat.Tif, image.BitsPerPixel);

            image.Dispose();
            codecs.Dispose();
        }
    }
}
