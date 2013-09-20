using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageProcessor.Interfaces
{
    public interface ITifPdfTransformer
    {
        void ConvertPdfToTif(string SourceFileName, string DestFileName);
        void ConvertTifToPdf(string SourceFileName, string DestFileName);
    }
}
