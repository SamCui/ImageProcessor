using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageProcessor.Helpers
{
    public class LibraryInUse
    {
        public enum WhichLibrary
        {
            Leadtools = 1,
            Dotnet = 2,
            Atalasoft = 3, 
            ImageMagick = 4
        }

        public LibraryInUse()
        {
        }

        public WhichLibrary Library
        {
            get;
            set;
        }
    }
}
