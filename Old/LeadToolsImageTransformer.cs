using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImageProcessor.Interfaces;
using Leadtools;
using Leadtools.Codecs;
using Leadtools.Codecs.Tif;
using Leadtools.Pdf;
using Leadtools.Forms.Ocr;
using Leadtools.Workflow.Ocr;
using Leadtools.Forms.DocumentWriters;
using System.Runtime.InteropServices;
using Leadtools.ImageProcessing;
namespace ImageProcessor
{
    public class LeadToolsImageTransformer : IGrayscaleTransformer
    {
        public string PdfInitialPath { get; set; }

        public void ConvertPdfToTif(string SourceFileName, string DestFileName)
        {          
            using (RasterCodecs codecs = new RasterCodecs())
            {
                codecs.Options.Pdf.InitialPath = PdfInitialPath;

                PDFDocument document = new PDFDocument(SourceFileName);
                // Loop through all the pages in the document
                for (int pageNumber = 1; pageNumber <= document.Pages.Count; pageNumber++)
                {
                    // Render the page into a raster image
                    using (RasterImage image = document.GetPageImage(codecs, pageNumber))
                    {
                        // Append to (or create if it does not exist) a TIFF file
                        codecs.Save(image, DestFileName, RasterImageFormat.Tif, 24, 1, 1, -1, CodecsSavePageMode.Append);
                        image.Dispose();
                    }
                }

                codecs.Dispose();
            }
        }

        public void ConvertToGrayScale(string SourceFileName, string DestFileName)
        {
            RasterImage image = null;
            RasterCodecs codecs = new RasterCodecs();
            int pageCount = 0;

            image = codecs.Load(SourceFileName);
            pageCount = image.PageCount;

            //RasterImage resizedImage = ResizeImage(image, codecs);
            var bitsPerPixel = 12;
            //if (convertToGrayScale)
            //    bitsPerPixel = 12;

            codecs.Save(image, DestFileName, RasterImageFormat.Tif, bitsPerPixel, 1, pageCount, 1, CodecsSavePageMode.Insert);

            image.Dispose();
            codecs.Dispose();
        }


        public void ResizeTiffImage(string SourceFileName, string DestFileName, bool convertToGrayScale, float percentOfOriginal)
        {
            using (RasterCodecs codecs = new RasterCodecs())
            {
                // Load the source image
                using (RasterImage srcImage = codecs.Load(SourceFileName))
                {
                    //int destWidth = Convert.ToInt32((srcImage.Width)*percentOfOriginal);
                    //int destHeight = Convert.ToInt32((srcImage.Height)*percentOfOriginal);

                    int destWidth = Convert.ToInt32((srcImage.Width) * 0.5);
                    int destHeight = Convert.ToInt32((srcImage.Height) * 0.5);

                    RasterImage image = null;

                    // Create the destination image
                    //RasterImage destImage = new RasterImage(
                    //     RasterMemoryFlags.Conventional,
                    //     destWidth,
                    //     destHeight,
                    //     srcImage.BitsPerPixel,
                    //     srcImage.Order,
                    //     srcImage.ViewPerspective,
                    //     srcImage.GetPalette(),
                    //     IntPtr.Zero,
                    //     0);

                    if (image == null)
                    {
                        //Resize(destWidth, destHeight, srcImage);
                        image = srcImage;
                    }
                    else
                    {
                        //Resize(destWidth, destHeight, srcImage);
                        image.AddPage(srcImage);
                        srcImage.Dispose();
                    }

                    
                    var bitsPerPixel = 24;
                    if (convertToGrayScale)
                        bitsPerPixel = 12;
                    //Save the destination image
                    codecs.Save(image, DestFileName, RasterImageFormat.Tif, bitsPerPixel);   
                }
            }
        }

        private void Resize(int destWidth, int destHeight, RasterImage image)
        {
            RasterImageResize resize = new RasterImageResize();

            // Add Event Handler
            resize.Resize += new EventHandler<RasterImageResizeEventArgs>(Resize);

            byte[] buffer = new byte[image.BytesPerLine];

            resize.Start(
               image,
               destWidth,
               destHeight,
               image.BitsPerPixel,
               image.Order,
               image.DitheringMethod,
               RasterSizeFlags.None,
               image.GetPalette());

            image.Access();
            // get the rows for the resized image, one by one
            for (int row = 0; row < image.Height; row++)
            {
                resize.ResizeBuffer(row, 0, buffer, 0, image.BytesPerLine);
                image.SetRow(row, buffer, 0, image.BytesPerLine);
            }
            image.Release();
            resize.Stop();
        }




        public void ConvertTifToPdf(string SourceFileName, string DestFileName)
        {
            RasterCodecs CodecsCommand = new RasterCodecs();
            RasterImage LeadImage = null;

            try
            {
                UnlockLeadToolsPDFSupport();
                //RasterCodecs.CodecsPath = codecsPath;
                //CodecsCommand.Options.Pdf.InitialPath = pdfInitialPath;
                CodecsCommand.Options.Pdf.Save.UseImageResolution = true;
                CodecsCommand.Options.Tiff.Load.IgnoreViewPerspective = true;
                LeadImage = CodecsCommand.Load(SourceFileName);
                //LeadImage.ChangeCompression((LeadImage.IsSuperCompressed == true) ? RasterCompression.None : RasterCompression.Super);
                if (String.IsNullOrEmpty(DestFileName))
                    DestFileName = System.IO.Path.Combine(System.IO.Path.GetTempPath().ToString(), Guid.NewGuid().ToString() + ".PDF");

                CodecsCommand.Save(LeadImage, DestFileName, Leadtools.RasterImageFormat.RasPdf, 1, 1,
                    LeadImage.PageCount, 1, Leadtools.Codecs.CodecsSavePageMode.Overwrite);

            }
            finally
            {
                if (LeadImage != null)
                    LeadImage.Dispose();
                if (CodecsCommand != null)
                    CodecsCommand.Dispose();
            }
        }

        public void ConvertDpi(string srcFileName, string destFileName) 
        {
            RasterCodecs codecs = new RasterCodecs();
  
            RasterImage image = codecs.Load(srcFileName);

            int newResolution = 300; //BR says all files need to be 300 DPI. This should probably be an enum.

            image.XResolution = newResolution;
            image.YResolution = newResolution;
            SizeCommand command = new SizeCommand();
            command.Width = image.Width;
            command.Height = image.Height;
            command.Flags = RasterSizeFlags.Resample;
            command.Run(image);
  
            codecs.Save(image, destFileName, RasterImageFormat.Tif, image.BitsPerPixel);

            image.Dispose();
            codecs.Dispose();
        }

        public void CompressImage(string SourceFileName, string DestFileName)
        {
            RasterCodecs codecs = new RasterCodecs();
            RasterImage sourceImage = codecs.Load(SourceFileName);
            RasterImage destinationImage = new RasterImage(
                RasterMemoryFlags.Conventional,
                sourceImage.Width,
                sourceImage.Height,
                sourceImage.BitsPerPixel,
                sourceImage.Order,
                sourceImage.ViewPerspective,
                sourceImage.GetPalette(),
                IntPtr.Zero,
                0);
            sourceImage.Access();
            destinationImage.Access();

            byte[] buffer = new byte[sourceImage.BytesPerLine];

            for (int y = 0; y < sourceImage.Height; y++)
            {
                sourceImage.GetRow(y, buffer, 0, buffer.Length);
                destinationImage.SetRow(y, buffer, 0, buffer.Length);
            }

            destinationImage.Release();
            sourceImage.Release();

            // We do not need the source image anymore
            sourceImage.Dispose();

            // save the destination image
            codecs.Save(destinationImage, DestFileName, RasterImageFormat.Tif, 24);

            // perform image processing on the image

            //FlipCommand flipCmd = new FlipCommand();
            //flipCmd.Horizontal = false;
            //flipCmd.Run(destinationImage);

            //// save it
            //codecs.Save(destinationImage, destFileName2, RasterImageFormat.Bmp, 24);

            // Clean up
            destinationImage.Dispose();
            codecs.Dispose();
            codecs.Dispose();
        }

        private void UnlockLeadToolsPDFSupport()
        {
            if (RasterSupport.IsLocked(RasterSupportType.PdfAdvanced))
                RasterSupport.Unlock(RasterSupportType.PdfAdvanced, "haDLeYrAE");
        }

        private void Resize(object sender, RasterImageResizeEventArgs e)
        {
            // e.Row should ALWAYS be less than e.Image.Height
            if (e.Row >= e.Image.Height)
            {
                e.Cancel = true; // abort the resize
                return;
            }

            byte[] buffer = new byte[e.Bytes];
            e.Image.Access();
            e.Image.GetRowColumn(e.Row, e.Column, buffer, 0, Convert.ToInt32(e.Bytes));
            e.Image.Release();
            Marshal.Copy(buffer, 0, e.Buffer, Convert.ToInt32(e.Bytes));
            //Console.WriteLine("{0}, {1}", e.Row, e.Column);
        }
    }
}
