using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace ImageProcessor.Helpers
{
    public class FileInformation
    {
        string _file = string.Empty;

        public FileInformation(string file)
        {
            _file = file;
        }

        public int GetFileSize()
        {
            var fileInfo = new FileInfo(_file);
            var fileSize = fileInfo.Length;
            return Convert.ToInt32(fileSize / 1048576); //return Megabytes 

            //1 Byte = 8 Bits
            //1 Kilobyte = 1024 Bytes
            //1 Megabyte = 1048576 Bytes
            //1 Gigabyte = 1073741824 Bytes
        }

        public int GetPageCount()
        {
            Bitmap bm = (Bitmap)Image.FromFile(_file);
            var pageCount = bm.GetFrameCount(FrameDimension.Page);
            return pageCount;
        }

        public string GetFileExtension()
        {
            return Path.GetExtension(_file);
        }
    }
}
