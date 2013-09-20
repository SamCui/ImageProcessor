using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageProcessor.Interfaces
{
    public interface IGrayscaleTransformer
    {
        void ConvertToGrayScale(string sourceFile, string destFile);
    }
}
