using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace ImageProcessor
{
    public class PDFSharpConverter
    {
        string source = @"C:\TestImage\Source\article.tif";
        string target = @"C:\TestImage\target\articlePDFSharp.pdf";

        public void tiff2PDF()
        {
            TiffImageSplitter tiff = new TiffImageSplitter(); 
            PdfDocument doc = new PdfDocument();

            Image sourceImage = Bitmap.FromFile(source);
            var pageCount = tiff.getPageCount(sourceImage);

            for (int i = 0; i < pageCount; i++)
            {
                PdfPage page = new PdfPage();

                Image tiffImg = tiff.getTiffImage(sourceImage, i);

                XImage img = XImage.FromGdiPlusImage(tiffImg);

                page.Width = img.PointWidth;
                page.Height = img.PointHeight;
                doc.Pages.Add(page);

                XGraphics xgr = XGraphics.FromPdfPage(doc.Pages[i]);

                xgr.DrawImage(img, 0, 0);
            }

            doc.Save(target);

            doc.Close();

            sourceImage.Dispose();
        }

    }


    class TiffImageSplitter
    {       
        // Retrive PageCount of a multi-page tiff image
        public int getPageCount(String fileName)
        {
            int pageCount = -1;
            try
            {
                Image img = Bitmap.FromFile(fileName);
                pageCount = img.GetFrameCount(FrameDimension.Page);
                img.Dispose();

            }
            catch (Exception ex)
            {
                pageCount = 0;
            }
            return pageCount;
        }

        public int getPageCount(Image img)
        {
            int pageCount = -1;
            try
            {
                pageCount = img.GetFrameCount(FrameDimension.Page);
            }
            catch (Exception ex)
            {
                pageCount = 0;
            }
            return pageCount;
        }

        public Image getTiffImage(Image sourceImage, int pageNumber)
        {
            MemoryStream ms = null;
            Image returnImage = null;

            try
            {
                ms = new MemoryStream();
                Guid objGuid = sourceImage.FrameDimensionsList[0];
                FrameDimension objDimension = new FrameDimension(objGuid);
                sourceImage.SelectActiveFrame(objDimension, pageNumber);
                sourceImage.Save(ms, ImageFormat.Tiff);
                returnImage = Image.FromStream(ms);
            }
            catch (Exception ex)
            {
                returnImage = null;
            }
            return returnImage;
        }
    }
}
