using System;
using System.Collections.Generic;
using System.Text;
using ImageProcessor.Interfaces;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.codec;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageProcessor
{
    public class ITextSharpImageTransformer: IITextSharpImageTransformer
    {       
        #region private fields
        private iTextSharp.text.Font _largeFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 18, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
        private iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 14, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
        private iTextSharp.text.Font _smallFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
        #endregion

        public string SourceFileName { get; set; }
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

        //this function embeds only images in the final saved pdf document       
        public void CombineMultipleImagesInOnePdf()
        {
            iTextSharp.text.Document doc = null;

            try
            {
                // Initialize the PDF document
                doc = new Document();
                iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc,
                    new System.IO.FileStream(TargetFilePath+TargetFileName,
                        System.IO.FileMode.Create));

                // Set the margins and page size
                this.SetStandardPageSize(doc);

 
                // Add Xmp metadata to the document.
                this.CreateXmpMetadata(writer);

                // Open the document for writing content
                doc.Open();


                foreach (var fileName in FilesToProcess)
                {
                    var ext = fileName.Substring(fileName.LastIndexOf(".") + 1);

                    if (ext == "tif")
                    {
                        AddTiff(doc, PageSize.A5, fileName);
                    }
                    else
                    {
                        AddPageWithImage(doc, fileName);
                    }
                }

                this.SetStandardPageSize(doc);  // Reset the margins and page size
            }
            catch (iTextSharp.text.DocumentException dex)
            {
                // Handle iTextSharp errors
            }
            finally
            {
                // Clean up
                doc.Close();
                doc = null;
            }
        }

        public void SaveEachImageAsOwnPdf(string sourceFileName)
        {
            iTextSharp.text.Document doc = null;

            try
            {
                // Initialize the PDF document
                doc = new Document();
                iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc,
                    new System.IO.FileStream(TargetFilePath + Guid.NewGuid().ToString()+".pdf",
                        System.IO.FileMode.Create));

                // Set the margins and page size
                this.SetStandardPageSize(doc);


                // Add Xmp metadata to the document.
                this.CreateXmpMetadata(writer);

                // Open the document for writing content
                doc.Open();

                var ext = sourceFileName.Substring(sourceFileName.LastIndexOf(".") + 1);

                if (ext == "tif")
                {
                    AddTiff(doc, PageSize.A5, sourceFileName);
                }
                else
                {
                    AddPageWithImage(doc, sourceFileName);
                }
                
                this.SetStandardPageSize(doc);  // Reset the margins and page size
            }
            catch (iTextSharp.text.DocumentException dex)
            {
                // Handle iTextSharp errors
            }
            finally
            {
                // Clean up
                doc.Close();
                doc = null;
            }
        }

        //this function takes multiple images from a directory
        //and saves each as a pdf document
        public void SaveAsPDFs()
        {
            foreach (var fileName in FilesToProcess)
            {
                CreatePDF(fileName);               
            }
        }
   
        //the function adds bells and whistles such as headers in the final saved pdf document
        public void SaveAsPDF()
        {
            iTextSharp.text.Document doc = null;

            try
            {
                // Initialize the PDF document
                doc = new Document();
                iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc,
                    new System.IO.FileStream(TargetFilePath + TargetFileName,
                        System.IO.FileMode.Create));

                // Set the margins and page size
                this.SetStandardPageSize(doc);

                // Add metadata to the document.  This information is visible when viewing the 
                // document properities within Adobe Reader.
                doc.AddTitle("CoreLogic Report");
                doc.AddHeader("title", "CoreLogic Report");
                doc.AddHeader("author", "Apex -- Docsolutions");
                doc.AddCreator("Apex -- Docsolutions");
                doc.AddKeywords("BOA mortgage letter");
                doc.AddHeader("subject", "BOA letter");

                // Add Xmp metadata to the document.
                this.CreateXmpMetadata(writer);

                // Open the document for writing content
                doc.Open();

                // Add pages to the document
                this.AddPageWithBasicFormatting(doc);
                this.AddPageWithInternalLinks(doc);
                this.AddPageWithBulletList(doc);


                foreach (var fileName in FilesToProcess)
                {
                    AddTiff(doc, PageSize.A5, fileName);                   
                }

                //AddPageWithImage(doc);

                // Add a final page
                this.SetStandardPageSize(doc);  // Reset the margins and page size
                this.AddPageWithExternalLinks(doc);

                // Add page labels to the document
                iTextSharp.text.pdf.PdfPageLabels pdfPageLabels = new iTextSharp.text.pdf.PdfPageLabels();
                pdfPageLabels.AddPageLabel(1, iTextSharp.text.pdf.PdfPageLabels.EMPTY, "Basic Formatting");
                pdfPageLabels.AddPageLabel(2, iTextSharp.text.pdf.PdfPageLabels.EMPTY, "Internal Links");
                pdfPageLabels.AddPageLabel(3, iTextSharp.text.pdf.PdfPageLabels.EMPTY, "Bullet List");
                pdfPageLabels.AddPageLabel(4, iTextSharp.text.pdf.PdfPageLabels.EMPTY, "Image");
                pdfPageLabels.AddPageLabel(5, iTextSharp.text.pdf.PdfPageLabels.EMPTY, "External Links");
                writer.PageLabels = pdfPageLabels;
            }
            catch (iTextSharp.text.DocumentException dex)
            {
                // Handle iTextSharp errors
            }
            finally
            {
                // Clean up
                doc.Close();
                doc = null;
            }
        }

        #region private methods
        private string[] GetFiles()
        {
            var fileGetter = new FileGetter();
            fileGetter.FileDirectory = SourceFilePath;
            return fileGetter.GetFiles();
        }

        //this function is capable of taking multiple tiff images from a directory 
        //and processing each tiff frame by frame
        private void AddTiff(Document pdfDocument, iTextSharp.text.Rectangle pdfPageSize, String tiffPath)
        {
            RandomAccessFileOrArray ra = new RandomAccessFileOrArray(tiffPath);
            int pageCount = TiffImage.GetNumberOfPages(ra);

            for (int i = 1; i <=pageCount; i++)
            {
                iTextSharp.text.Image img = TiffImage.GetTiffImage(ra, i);

                if (img.ScaledWidth > pdfPageSize.Width || img.ScaledHeight > pdfPageSize.Height)
                {
                    img.SetDpi(2, 2);

                    if (img.DpiX != 0 && img.DpiY != 0 && img.DpiX != img.DpiY)
                    {                    
                        float percentX = (pdfPageSize.Width * 100) / img.ScaledWidth;
                        float percentY = (pdfPageSize.Height * 100) / img.ScaledHeight;

                        img.ScalePercent(percentX, percentY);
                        img.WidthPercentage = 0;
                    }
                    else
                    {
                        img.ScaleToFit(pdfPageSize.Width, pdfPageSize.Height);
                    }
                }

                iTextSharp.text.Rectangle pageRect = new iTextSharp.text.Rectangle(0, 0, img.ScaledWidth, img.ScaledHeight);

                pdfDocument.SetPageSize(pageRect);
                pdfDocument.SetMargins(0, 0, 0, 0);
                pdfDocument.NewPage();
                pdfDocument.Add(img);
            }
        }

        //this function is capable of processing multiframe bitmap
        private List<Bitmap> SplitImages(string fileName)
        {
            Bitmap bitMap = (Bitmap)System.Drawing.Image.FromFile(fileName);

            List<Bitmap> bitMaps = new List<Bitmap>();

            var pageCount = bitMap.GetFrameCount(FrameDimension.Page);
            for (int i = 0; i < pageCount; i++)
            {
                Bitmap newBitmap = new Bitmap(bitMap.Width, bitMap.Height);
                bitMap.SelectActiveFrame(FrameDimension.Page, i);
                //get a graphics object from the new image
                Graphics g = Graphics.FromImage(newBitmap);

                //create the grayscale ColorMatrix
                //  ColorMatrix colorMatrix = new ColorMatrix(
                //     new float[][] 
                //{
                //   new float[] {.3f, .3f, .3f, 0, 0},
                //   new float[] {.59f, .59f, .59f, 0, 0},
                //   new float[] {.11f, .11f, .11f, 0, 0},
                //   new float[] {0, 0, 0, 1, 0},
                //   new float[] {0, 0, 0, 0, 1}
                //});

                //create some image attributes
                //ImageAttributes attributes = new ImageAttributes();

                //set the color matrix attribute
                // attributes.SetColorMatrix(colorMatrix);

                //draw the original image on the new image
                //using the grayscale color matrix
                //g.DrawImage(bitMap, new System.Drawing.Rectangle(0, 0, bitMap.Width, bitMap.Height),
                //   0, 0, bitMap.Width, bitMap.Height, GraphicsUnit.Pixel, attributes);

                g.DrawImage(bitMap, 0, 0);

                //dispose the Graphics object
                g.Dispose();
                bitMaps.Add(newBitmap);
            }

            return bitMaps;
        }

        private void SaveBitmaps(Document pdfDocument, iTextSharp.text.Rectangle pdfPageSize, List<Bitmap> bitMaps)
        {
            int pageCount = bitMaps.Count;

            for (int i = 0; i <pageCount; i++)
            {
                System.Drawing.Image bitImg= (System.Drawing.Image)bitMaps[i];

                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(bitImg, BaseColor.WHITE);

                if (img.ScaledWidth > pdfPageSize.Width || img.ScaledHeight > pdfPageSize.Height)
                {
                    if (img.DpiX != 0 && img.DpiY != 0 && img.DpiX != img.DpiY)
                    {
                        img.ScalePercent(100f);
                        float percentX = (pdfPageSize.Width * 100) / img.ScaledWidth;
                        float percentY = (pdfPageSize.Height * 100) / img.ScaledHeight;

                        img.ScalePercent(percentX, percentY);
                        img.WidthPercentage = 0;
                    }
                    else
                    {
                        img.ScaleToFit(pdfPageSize.Width, pdfPageSize.Height);
                    }
                }

                iTextSharp.text.Rectangle pageRect = new iTextSharp.text.Rectangle(0, 0, img.ScaledWidth, img.ScaledHeight);

                pdfDocument.SetPageSize(pageRect);
                pdfDocument.SetMargins(0, 0, 0, 0);
                pdfDocument.NewPage();
                pdfDocument.Add(img);
            }
        }



        private void CreatePDF(string fileName)
        {
            iTextSharp.text.Document doc = null;

            try
            {
                // Initialize the PDF document
                doc = new Document();
                iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc,
                    new System.IO.FileStream(TargetFilePath+Guid.NewGuid().ToString()+".pdf",
                        System.IO.FileMode.Create));

                // Set the margins and page size
                this.SetStandardPageSize(doc);


                // Add Xmp metadata to the document.
                this.CreateXmpMetadata(writer);

                // Open the document for writing content
                doc.Open();

                AddPageWithImage(doc, fileName);

                //var bitMaps = SplitImages(fileName);
                //SaveBitmaps(doc, PageSize.A5, bitMaps);

                this.SetStandardPageSize(doc);  // Reset the margins and page size
            }
            catch (iTextSharp.text.DocumentException dex)
            {
                // Handle iTextSharp errors
            }
            finally
            {
                // Clean up
                doc.Close();
                doc = null;
            }
        }

        /// <summary>
        /// Add the header page to the document.  This shows an example of a page containing
        /// both text and images.  The contents of the page are centered and the text is of
        /// various sizes.
        /// </summary>
        /// <param name="doc"></param>
        private void AddPageWithBasicFormatting(iTextSharp.text.Document doc)
        {
            // Add a logo
            String appPath = System.IO.Directory.GetCurrentDirectory();
            iTextSharp.text.Image logoImage = iTextSharp.text.Image.GetInstance(appPath + "\\corelogic.jpg");
            logoImage.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            doc.Add(logoImage);
            logoImage = null;

            // Write page content.  Note the use of fonts and alignment attributes.
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _largeFont, new iTextSharp.text.Chunk("\n\n"));
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _largeFont, new Chunk("Corelogic Demo Letter\n\n"));
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _standardFont, new Chunk("by Apex -- Docsolutions"));
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _largeFont, new Chunk("\n\n\n"));
          
            // Write additional page content
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _largeFont, new Chunk("\n\n\n"));
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _largeFont, new Chunk(""));
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _largeFont, new Chunk("\n\n\n\n\n"));
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _smallFont, new Chunk("Generated " +
                DateTime.Now.Day.ToString() + " " +
                System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " +
                DateTime.Now.Year.ToString() + " " +
                DateTime.Now.ToShortTimeString()));
        }

        /// <summary>
        /// Add a blank page to the document.
        /// </summary>
        /// <param name="doc"></param>
        private void AddPageWithInternalLinks(iTextSharp.text.Document doc)
        {
            // Generate links to be embedded in the page
            Anchor startAnchor = new Anchor("Beginning words.....\n\n", _standardFont);
            startAnchor.Reference = "#start"; // this link references a named anchor within the document
            Anchor graphAnchor = new Anchor("Graph\n\n", _standardFont);
            graphAnchor.Reference = "#graph";
            Anchor endAnchor = new Anchor("Ending words....", _standardFont);
            endAnchor.Reference = "#end";

            // Add a new page to the document
            doc.NewPage();

            // Add heading text to the page
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _largeFont, 
                new iTextSharp.text.Chunk("TABLE OF CONTENTS\n\n\n\n\n"));

            // Add the links to the page
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _standardFont, startAnchor);
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _standardFont, graphAnchor);
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _standardFont, endAnchor);
        }

        /// <summary>
        /// Add a page that includes a bullet list.
        /// </summary>
        /// <param name="doc"></param>
        private void AddPageWithBulletList(iTextSharp.text.Document doc)
        {
            // Add a new page to the document
            doc.NewPage();

            // The header at the top of the page is an anchor linked to by the table of contents.
            iTextSharp.text.Anchor contentsAnchor = new iTextSharp.text.Anchor("Beginning page\n\n", _largeFont);
            contentsAnchor.Name = "start";

            // Add the header anchor to the page
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _largeFont, contentsAnchor);

            // Create an unordered bullet list.  The 10f argument separates the bullet from the text by 10 points
            iTextSharp.text.List list = new iTextSharp.text.List(iTextSharp.text.List.UNORDERED, 10f);
            list.SetListSymbol("\u2022");   // Set the bullet symbol (without this a hypen starts each list item)
            list.IndentationLeft = 20f;     // Indent the list 20 points
            list.Add(new ListItem("Print document", _standardFont));
            list.Add(new ListItem("Route to mail room", _standardFont));
            list.Add(new ListItem("Route to accounting", _standardFont));
            list.Add(new ListItem("Check approval", _standardFont));
            list.Add(new ListItem("Send the check", _standardFont));
            doc.Add(list);  // Add the list to the page

            // Add some white space and another heading
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _largeFont, new Chunk("\n\n\n"));
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _largeFont, new Chunk("Error condition\n\n"));

            // Add some final text to the page
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_LEFT, _standardFont, new Chunk("In case of error, check will be manually approved"));
        }

        /// <summary>
        /// Add a page containing a single image.  Set the page size to match the image size.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="imagePath"></param>
        private void AddPageWithImage(iTextSharp.text.Document doc, string fileName)
        {           
            // Read the image file
            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(new Uri(fileName));

            // Set the page size to the dimensions of the image BEFORE adding a new page to the document.
            // Pad the height a bit to leave room for the page header.
            float imageWidth = image.Width;
            float imageHeight = image.Height;
            doc.SetMargins(0, 0, 0, 0);
            doc.SetPageSize(new iTextSharp.text.Rectangle(imageWidth, imageHeight + 100));

            // Add a new page
            doc.NewPage();

            // The header at the top of the page is an anchor linked to by the table of contents.
            iTextSharp.text.Anchor contentsAnchor = new iTextSharp.text.Anchor("", _largeFont);
            //contentsAnchor.Name = "graph";

            // Add the anchor and image to the page
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _largeFont, contentsAnchor);
            doc.Add(image);
            image = null;          
        }

        /// <summary>
        /// Add a page that contains embedded hyperlinks to external resources
        /// </summary>
        /// <param name="doc"></param>
        private void AddPageWithExternalLinks(Document doc)
        {
            // Generate external links to be embedded in the page
            Anchor linkAnchor1 = new Anchor("realtor.com", _standardFont);
            Anchor linkAnchor2 = new Anchor("cnn", _standardFont);
            Anchor linkAnchor3 = new Anchor("microsoft", _standardFont);
            linkAnchor1.Reference = "http://www.realtor.com";           
            linkAnchor2.Reference = "http://www.cnn.com/";
            linkAnchor3.Reference = "http://www.microsoft.com/";

            // The header at the top of the page is an anchor linked to by the table of contents.
            iTextSharp.text.Anchor contentsAnchor = new iTextSharp.text.Anchor("RESULTS\n\n", _largeFont);
            contentsAnchor.Name = "end";

            // Add a new page to the document
            doc.NewPage();

            // Add text to the page
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _largeFont, contentsAnchor);
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_LEFT, _standardFont, new Chunk("Document sent to client"));
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _largeFont, new Chunk("\n\n\n"));
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_CENTER, _largeFont, new Chunk("Links\n\n"));

            // Add the links to the page
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_LEFT, _standardFont, linkAnchor1);
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_LEFT, _standardFont, linkAnchor2);
            this.AddParagraph(doc, iTextSharp.text.Element.ALIGN_LEFT, _standardFont, linkAnchor3);
        }

        /// <summary>
        /// Set margins and page size for the document
        /// </summary>
        /// <param name="doc"></param>
        private void SetStandardPageSize(iTextSharp.text.Document doc)
        {
            // Set margins and page size for the document
            doc.SetMargins(50, 50, 50, 50);
            // There are a huge number of possible page sizes, including such sizes as
            // EXECUTIVE, POSTCARD, LEDGER, LEGAL, LETTER_LANDSCAPE, and NOTE
            doc.SetPageSize(new iTextSharp.text.Rectangle(iTextSharp.text.PageSize.LETTER.Width,
                iTextSharp.text.PageSize.LETTER.Height));
        }

        /// <summary>
        /// Add a paragraph object containing the specified element to the PDF document.
        /// </summary>
        /// <param name="doc">Document to which to add the paragraph.</param>
        /// <param name="alignment">Alignment of the paragraph.</param>
        /// <param name="font">Font to assign to the paragraph.</param>
        /// <param name="content">Object that is the content of the paragraph.</param>
        private void AddParagraph(Document doc, int alignment, iTextSharp.text.Font font, iTextSharp.text.IElement content)
        {
            Paragraph paragraph = new Paragraph();
            paragraph.SetLeading(0f, 1.2f);
            paragraph.Alignment = alignment;
            paragraph.Font = font;
            paragraph.Add(content);
            doc.Add(paragraph);
        }

        /// <summary>
        /// Use this method to write XMP data to a new PDF
        /// </summary>
        /// <param name="writer"></param>
        private void CreateXmpMetadata(iTextSharp.text.pdf.PdfWriter writer)
        {
            // Set up the buffer to hold the XMP metadata
            byte[] buffer = new byte[65536];
            System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer, true);

            try
            {
                // XMP supports a number of different schemas, which are made available by iTextSharp.
                // Here, the Dublin Core schema is chosen.
                iTextSharp.text.xml.xmp.XmpSchema dc = new iTextSharp.text.xml.xmp.DublinCoreSchema();

                // Add Dublin Core attributes
                iTextSharp.text.xml.xmp.LangAlt title = new iTextSharp.text.xml.xmp.LangAlt();
                title.Add("x-default", "Corelogic letter");
                dc.SetProperty(iTextSharp.text.xml.xmp.DublinCoreSchema.TITLE, title);

                // Dublin Core allows multiple authors, so we create an XmpArray to hold the values
                iTextSharp.text.xml.xmp.XmpArray author = new iTextSharp.text.xml.xmp.XmpArray(iTextSharp.text.xml.xmp.XmpArray.ORDERED);
                author.Add("Corelogic docsolutions");
                dc.SetProperty(iTextSharp.text.xml.xmp.DublinCoreSchema.CREATOR, author);

                // Multiple subjects are also possible, so another XmpArray is used
                iTextSharp.text.xml.xmp.XmpArray subject = new iTextSharp.text.xml.xmp.XmpArray(iTextSharp.text.xml.xmp.XmpArray.UNORDERED);
                subject.Add("paper airplanes");
                subject.Add("science project");
                dc.SetProperty(iTextSharp.text.xml.xmp.DublinCoreSchema.SUBJECT, subject);

                // Create an XmpWriter using the MemoryStream defined earlier
                iTextSharp.text.xml.xmp.XmpWriter xmp = new iTextSharp.text.xml.xmp.XmpWriter(ms);
                xmp.AddRdfDescription(dc);  // Add the completed metadata definition to the XmpWriter
                xmp.Close();    // This flushes the XMP metadata into the buffer

                //---------------------------------------------------------------------------------
                // Shrink the buffer to the correct size (discard empty elements of the byte array)
                int bufsize = buffer.Length;
                int bufcount = 0;
                foreach (byte b in buffer)
                {
                    if (b == 0) break;
                    bufcount++;
                }
                System.IO.MemoryStream ms2 = new System.IO.MemoryStream(buffer, 0, bufcount);
                buffer = ms2.ToArray();
                //---------------------------------------------------------------------------------

                // Add all of the XMP metadata to the PDF doc that we're building
                writer.XmpMetadata = buffer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ms.Close();
                ms.Dispose();
            }
        }
        #endregion
    }
}
