using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ImageProcessor.Interfaces
{
    public interface IITextSharpImageTransformer
    {
        string SourceFileName { get; set; }
        string SourceFilePath { get; set; }
        string TargetFilePath { get; set; }
        string TargetFileName { get; set; }
        string[] FilesToProcess { get; }

        void CombineMultipleImagesInOnePdf();
        void SaveEachImageAsOwnPdf(string fileName);
        void SaveAsPDFs();
        void SaveAsPDF();
    }
}
