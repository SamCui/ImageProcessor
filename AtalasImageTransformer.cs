using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atalasoft.Imaging;
using Atalasoft.Imaging.Codec;
using Atalasoft.Imaging.ImageProcessing.Document;
using ImageProcessor.Interfaces;

using Atalasoft.Imaging.Codec.Tiff;
using Atalasoft.Imaging.ImageProcessing;
//using Atalasoft.Imaging.ImageProcessing.Effects;
using Atalasoft.Imaging.ImageProcessing.Filters;
using Atalasoft.Imaging.ImageProcessing.Transforms;
using Atalasoft.Imaging.ImageProcessing.Channels;
using Atalasoft.Imaging.Metadata;
using Atalasoft.Imaging.Drawing;
using System.IO;

namespace ImageProcessor
{
    public class AtalasImageTransformer: IAtalasImageTransformer
    {

        public void ConvertToGrayScale(string sourceFile, string targetFile)
        {
            MultiFramedImageEncoder encoder = new TiffEncoder();

            using (ImageSource src = new FileSystemImageSource(sourceFile, true))
            {
                ImageCollection images = new ImageCollection();

                while (src.HasMoreImages())
                {
                    AtalaImage img = src.AcquireNext();

                    img = img.GetChangedPixelFormat(PixelFormat.Pixel8bppGrayscale);
                    images.Add(img);
                    //img.Dispose();
                }

                using (Stream s = File.Create(targetFile))
                {
                    encoder.Save(s, images, null);
                }
            }
        }
        
    }
}
