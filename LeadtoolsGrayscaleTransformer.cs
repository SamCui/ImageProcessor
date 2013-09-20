using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImageProcessor.Interfaces;
using Leadtools;
using Leadtools.Codecs;

namespace ImageProcessor
{
    public class LeadtoolsGrayscaleTransformer: IGrayscaleTransformer
    {
        public void ConvertToGrayScale(string SourceFileName, string DestFileName)
        {
            RasterImage image = null;
            RasterCodecs codecs = new RasterCodecs();
            int pageCount = 0;

            image = codecs.Load(SourceFileName);
            pageCount = image.PageCount;

            //RasterImage resizedImage = ResizeImage(image, codecs);
            var bitsPerPixel = 12;
            //if (convertToGrayScale)
            //    bitsPerPixel = 12;

            codecs.Save(image, DestFileName, RasterImageFormat.Tif, bitsPerPixel, 1, pageCount, 1, CodecsSavePageMode.Insert);

            image.Dispose();
            codecs.Dispose();
        }
    }
}
