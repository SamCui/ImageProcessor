using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageProcessor.Interfaces
{
    public interface IFileGetter
    {
        string FileDirectory { get; set; }

        string[] GetFiles();
    }
}
