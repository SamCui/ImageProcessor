using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImageProcessor.Interfaces;
using Leadtools;
using Leadtools.Codecs;
using Leadtools.Pdf;

namespace ImageProcessor
{  
    public class LeadtoolsTifPdfTransformer: ITifPdfTransformer
    {
        string _pdfInitialPath = string.Empty;

        public LeadtoolsTifPdfTransformer(string pdfInitialPath)
        {
            _pdfInitialPath = pdfInitialPath;
        }

        public void ConvertTifToPdf(string SourceFileName, string DestFileName)
        {
            RasterCodecs CodecsCommand = new RasterCodecs();
            RasterImage LeadImage = null;

            try
            {
                UnlockLeadToolsPDFSupport();
                CodecsCommand.Options.Pdf.Save.UseImageResolution = true;
                CodecsCommand.Options.Tiff.Load.IgnoreViewPerspective = true;
                LeadImage = CodecsCommand.Load(SourceFileName);

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

        public void ConvertPdfToTif(string SourceFileName, string DestFileName)
        {
            using (RasterCodecs codecs = new RasterCodecs())
            {
                codecs.Options.Pdf.InitialPath = _pdfInitialPath; ;

                PDFDocument document = new PDFDocument(SourceFileName);
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

        #region private helper methods
        private void UnlockLeadToolsPDFSupport()
        {
            if (RasterSupport.IsLocked(RasterSupportType.PdfAdvanced))
                RasterSupport.Unlock(RasterSupportType.PdfAdvanced, "haDLeYrAE");
        }

        #endregion
    }
}
