using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ImageProcessor.Interfaces
{
    public interface IFileSaver
    {
        string SourceFilePath { get; set; }
        string TargetFilePath { get; set; }
        string TargetFileName { get; set; }
        string[] FilesToProcess { get; }

        void SaveImages(Bitmap bitMap);
        void SaveImages(List<Bitmap> bitMaps);
    }
}
