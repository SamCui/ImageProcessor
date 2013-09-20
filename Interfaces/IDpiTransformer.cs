using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageProcessor.Interfaces
{
    public interface IDpiTransformer
    {
        void ConvertDpi(string srcFileName, string destFileName);
    }
}
