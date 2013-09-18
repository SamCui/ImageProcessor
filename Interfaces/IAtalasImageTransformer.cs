using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageProcessor.Interfaces
{
    public interface IAtalasImageTransformer
    {
        void ConvertToGrayScale(string SourceFileName, string DestFileName);
    }
}
