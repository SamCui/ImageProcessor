using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace ImageProcessor
{
    public class DotnetImageProcessor : IImageProcessor
    {
        //image format
        //extension
        //source directory
        //target directory
        //EncodeValue //unsigned integer

        public string SourceDirectory { get; set; }
        public ImageFormat TargetImageFormat { get; set; }
        public string TargetImageExtension { get; set; }
        public string TargetDirectory { get; set; }
        public string TargetImageName { get; set; }
        public long EncoderParameterValue { get; set; }

        public void CreateImage()
        {
            var bmp = new Bitmap(SourceDirectory);
            var imageCodecInfo = GetImageCodecInfo(TargetImageFormat);

            var encoder = System.Drawing.Imaging.Encoder.Quality;

            var encoderParameters = new EncoderParameters(1);

            var encoderParameter = new EncoderParameter(encoder, EncoderParameterValue);
            encoderParameters.Param[0] = encoderParameter;
            bmp.Save(TargetDirectory + TargetImageName + TargetImageExtension, imageCodecInfo, encoderParameters);
        }

        private ImageCodecInfo GetImageCodecInfo(ImageFormat format)
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
