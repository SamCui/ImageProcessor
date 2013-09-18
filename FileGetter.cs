using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ImageProcessor.Interfaces;

namespace ImageProcessor
{
    public class FileGetter: IFileGetter
    {
        public string FileDirectory { get; set; }

        public string[] GetFiles()
        {
            return Directory.GetFiles(FileDirectory);
        }
    }
}
