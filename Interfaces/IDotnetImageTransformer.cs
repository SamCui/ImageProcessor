using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ImageProcessor.Interfaces
{
    public interface IDotnetImageTransformer
    {
        //Bitmap ConvertToGrayScaleSingle(Bitmap bitMap);
        //List<Bitmap> ConvertToGrayScale(Bitmap bitMap);
        List<Bitmap> ConvertToGrayScale(Bitmap bitMap, string fileName, float percentOfOriginal);
    }
}
