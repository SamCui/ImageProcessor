using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace ImageProcessor
{
    public interface IImageProcessor
    {
        string SourceDirectory { get; set; }
        ImageFormat TargetImageFormat { get; set; }
        string TargetImageExtension { get; set; }
        string TargetDirectory { get; set; }
        string TargetImageName { get; set; }
        long EncoderParameterValue { get; set; }
        void CreateImage();
    }
}
