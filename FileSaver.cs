using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using ImageProcessor.Interfaces;

namespace ImageProcessor
{
    public class FileSaver: IFileSaver
    {
        public string SourceFilePath { get; set; }
        public string TargetFilePath { get; set; }
        public string TargetFileName { get; set; }
        public string[] FilesToProcess
        {
            get
            {
                return GetFiles();
            }
        }

        //this function takes a bitmap and splits it into different frames if necessary
        public void SaveImages(Bitmap bitMap)
        {
            //Bitmap bitMap = (Bitmap)Image.FromFile(SourceFile);

            ImageCodecInfo info = null;

            info = GetEncoder(ImageFormat.Tiff); //tiff format works with other types 

            System.Drawing.Imaging.Encoder enc = System.Drawing.Imaging.Encoder.SaveFlag;
            EncoderParameters ep = new EncoderParameters(1);
            ep.Param[0] = new EncoderParameter(enc, (long)EncoderValue.MultiFrame);
            Bitmap pages = null;
            var frame = 0;

            //see if the image has more than one page
            var pageCount = bitMap.GetFrameCount(FrameDimension.Page);
            for (int i = 0; i < pageCount; i++)
            {
                if (i == 0)
                {
                    bitMap.SelectActiveFrame(FrameDimension.Page, i);

                    pages = bitMap;

                    pages.Save(TargetFilePath+TargetFileName, info, ep);
                }
                else
                {
                    ep.Param[0] = new EncoderParameter(enc, (long)Convert.ToDouble(EncoderValue.FrameDimensionPage));
                    bitMap.SelectActiveFrame(FrameDimension.Page, i);
                    Bitmap bm = bitMap;

                    pages.SaveAdd(bm, ep);
                }
                if (i == pageCount - 1)
                {
                    ep.Param[0] = new EncoderParameter(enc, (long)Convert.ToDouble(EncoderValue.Flush));
                    pages.SaveAdd(ep);
                }
            }
        }

        //this function takes an array of bitmaps and process each
        public void SaveImages(List<Bitmap> bitMaps)
        {
            //Bitmap bitMap = (Bitmap)Image.FromFile(SourceFile);

            ImageCodecInfo info = null;

            info = GetEncoder(ImageFormat.Tiff); //tiff format works with other types 

            System.Drawing.Imaging.Encoder enc = System.Drawing.Imaging.Encoder.SaveFlag;
            EncoderParameters ep = new EncoderParameters(1);
            ep.Param[0] = new EncoderParameter(enc, (long)EncoderValue.MultiFrame);
            Bitmap pages = null;
            var frame = 0;
            //var pageCount = bitMap.GetFrameCount(FrameDimension.Page);

            var pageCount = bitMaps.Count();

            for (int i = 0; i < pageCount; i++)
            {
                if (i == 0)
                {
                    //bitMap.SelectActiveFrame(FrameDimension.Page, i);

                    pages = bitMaps[i];

                    pages.Save(TargetFilePath + TargetFileName, info, ep);
                }
                else
                {
                    ep.Param[0] = new EncoderParameter(enc, (long)Convert.ToDouble(EncoderValue.FrameDimensionPage));
                    //bitMap.SelectActiveFrame(FrameDimension.Page, i);
                    Bitmap bm = bitMaps[i];

                    pages.SaveAdd(bm, ep);
                }
                if (i == pageCount - 1)
                {
                    ep.Param[0] = new EncoderParameter(enc, (long)Convert.ToDouble(EncoderValue.Flush));
                    pages.SaveAdd(ep);
                }
            }
        }

        #region private methods
        private string[] GetFiles()
        {
            var fileGetter = new FileGetter();
            fileGetter.FileDirectory = SourceFilePath;
            return fileGetter.GetFiles();
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        #endregion
    }
}
