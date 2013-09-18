using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageProcessor.Helpers
{
    public class OperationMode
    {
        public enum WhichOperation
        {
            GrayScaleConversion = 1,
            PDFConversion = 2
        }

        public OperationMode()
        {
        }

        public WhichOperation Operation
        {
            get;
            set;
        }
    }
}
