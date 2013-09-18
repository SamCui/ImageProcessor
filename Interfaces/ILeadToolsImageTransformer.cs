using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageProcessor.Interfaces
{
    public interface ILeadToolsImageTransformer
    {
        string PdfInitialPath { get; set; }
        //void ConvertTifToPdf(string SourceFileName, string DestFileName);
        //void ConvertPdfToTif(string SourceFileName, string DestFileName);
        //void CompressImage(string SourceFileName, string DestFileName);
        void CopyTiffImage(string SourceFileName, string DestFileName, bool convertToGrayScale);
        void ResizeTiffImage(string SourceFileName, string DestFileName, bool convertToGrayScale, float percentOfOriginal);
    }
}
