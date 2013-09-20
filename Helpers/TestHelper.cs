using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using ImageProcessor.Helpers;
using ImageProcessor.Interfaces;
using Ninject;
using Ninject.Parameters;
using Excel = Microsoft.Office.Interop.Excel;

namespace ImageProcessor.Helpers
{
    public class TestHelper
    {
        #region private fields
        StandardKernel kernel = null;
        
        IFileSaver fileSaver = null;
        IFileGetter fileGetter = null;
     
        IGrayscaleTransformer atalasoftGrayscaleTransformer = null;
        IGrayscaleTransformer dotNetGrayscaleTransformer = null;
        IGrayscaleTransformer leadToolsGrayscaleTransformer = null;

        ITifPdfTransformer leadToolsTifPdfTransformer = null;

        IDpiTransformer leadToolsDpiTransformer = null;

        //IITextSharpImageTransformer iTextSharpImageTransformer = null;
        //IDotnetImageTransformer dotNetImageTransformer = null;
        //ILeadToolsImageTransformer leadToolsImageTransformer = null;

        string sizeUOM = " megabytes";
        int sizeDivisor = 1048576;

        OperationMode.WhichOperation _operation = OperationMode.WhichOperation.GrayScaleConversion;
        #endregion

        public TestHelper(OperationMode.WhichOperation operation)
        {
            InitializeTestHelper(operation);
        }

        private void InitializeTestHelper(OperationMode.WhichOperation operation)
        {
            kernel = new StandardKernel(new RunnerModule());
            
            fileSaver = kernel.Get<FileSaver>();
            fileGetter = kernel.Get<FileGetter>();
          
            atalasoftGrayscaleTransformer = kernel.Get<AtalasoftGrayscaleTransformer>();
            dotNetGrayscaleTransformer = kernel.Get<DotnetGrayscaleTransformer>();
            leadToolsGrayscaleTransformer = kernel.Get<LeadtoolsGrayscaleTransformer>();

            leadToolsTifPdfTransformer = kernel.Get<ITifPdfTransformer>();

            leadToolsDpiTransformer = kernel.Get<LeadtoolsDpiTransformer>();

            //dotNetImageTransformer = kernel.Get<DotnetImageTransformer>();
            //leadToolsImageTransformer = kernel.Get<ILeadToolsImageTransformer>();
            
            _operation = operation;
        }

        #region public properties
        public string FileInformation { get; set; }      

        public string FileSummary { get; set; }
        public string SourceFilePath { get; set; }
        public string TargetFilePath { get; set; }
        #endregion


        #region public test methods
        public void TestITextSharp()
        {
            fileSaver.SourceFilePath = SourceFilePath;
            fileSaver.TargetFilePath = TargetFilePath;

            foreach (var file in fileSaver.FilesToProcess)
            {
                Bitmap bm = (Bitmap)Image.FromFile(file);
                //iTextSharpImageTransformer.SaveEachImageAsOwnPdf(file);
            }
        }

        public void TestDotnet()
        {
            DateTime startTime;
            int totalSourceSize;
            int totalTargetSize;
            long totalMemoryUsed;
            long startWorkingSet;
            SetupStatsCollection(out startTime, out totalSourceSize, out totalTargetSize, out totalMemoryUsed, out startWorkingSet);

            TestDotnet(ref totalSourceSize, ref totalTargetSize);

            TimeSpan timeTaken = GetStats(startTime, ref totalMemoryUsed, startWorkingSet);
            GetSummary(totalSourceSize, totalTargetSize, totalMemoryUsed, timeTaken);
            WriteToExcel("Dotnet", totalSourceSize.ToString(), totalTargetSize.ToString(), (totalSourceSize - totalTargetSize).ToString(), totalMemoryUsed.ToString(), timeTaken.TotalSeconds.ToString(), _operation.ToString());
        }

        public void TestLeadTools()
        {
            DateTime startTime;
            int totalSourceSize;
            int totalTargetSize;
            long totalMemoryUsed;
            long startWorkingSet;
            SetupStatsCollection(out startTime, out totalSourceSize, out totalTargetSize, out totalMemoryUsed, out startWorkingSet);
            
            //leadToolsImageTransformer.PdfInitialPath = @"C:\CodeStore\LeadToolsLibrary\";
            TestLeadtools(ref totalSourceSize, ref totalTargetSize);

            TimeSpan timeTaken = GetStats(startTime, ref totalMemoryUsed, startWorkingSet);
            GetSummary(totalSourceSize, totalTargetSize, totalMemoryUsed, timeTaken);           
            WriteToExcel("Leadtools", totalSourceSize.ToString(), totalTargetSize.ToString(), (totalSourceSize - totalTargetSize).ToString(), totalMemoryUsed.ToString(), timeTaken.TotalSeconds.ToString(), _operation.ToString());
        }

        public void TestAtalasoft()
        {
            DateTime startTime;
            int totalSourceSize;
            int totalTargetSize;
            long totalMemoryUsed;
            long startWorkingSet;
            SetupStatsCollection(out startTime, out totalSourceSize, out totalTargetSize, out totalMemoryUsed, out startWorkingSet);
            
            TestAtalasoft(ref totalSourceSize, ref totalTargetSize);

            TimeSpan timeTaken = GetStats(startTime, ref totalMemoryUsed, startWorkingSet);
            GetSummary(totalSourceSize, totalTargetSize, totalMemoryUsed, timeTaken);
            WriteToExcel("Atalasfot", totalSourceSize.ToString(), totalTargetSize.ToString(), (totalSourceSize - totalTargetSize).ToString(), totalMemoryUsed.ToString(), timeTaken.TotalSeconds.ToString(), _operation.ToString());
        }
        #endregion

        #region Dotnet private methods
        private void TestDotnet(ref int totalSourceSize, ref int totalTargetSize)
        {
            if (_operation == OperationMode.WhichOperation.GrayScaleConversion)
            {
                FileInformation = string.Empty;
                totalSourceSize = 0;
                totalTargetSize = 0;

                fileSaver.SourceFilePath = SourceFilePath;
                fileSaver.TargetFilePath = TargetFilePath;

                foreach (var file in fileSaver.FilesToProcess)
                {
                    var fileInfo = new FileInformation(file);
                    var fileSize = fileInfo.GetFileSize();
                    var pageCount = fileInfo.GetPageCount();
                    var ext = fileInfo.GetFileExtension();
                    var fileName = Path.GetFileName(file);

                    totalSourceSize += fileSize;
                    FileInformation += "Source file:  " + fileName + " has " + pageCount.ToString() + " pages and " + fileSize.ToString() + sizeUOM + Environment.NewLine;

                    //Conversion
                    var destFile = fileSaver.TargetFilePath + "Dotnet" + fileName;
                    dotNetGrayscaleTransformer.ConvertToGrayScale(file, destFile);
                    
                    var targetFileInfo = new FileInformation(destFile);
                    var targetFileSize = targetFileInfo.GetFileSize();
                    var targetPageCount = targetFileInfo.GetPageCount();
                    var targetExt = targetFileInfo.GetFileExtension();

                    totalTargetSize += targetFileSize;
                    FileInformation += "Target file: " + destFile + " has " + targetPageCount.ToString() + " pages and " + targetFileSize.ToString() + sizeUOM + Environment.NewLine;
                }
            }
        }
        #endregion
     
        #region Leadtools private methods
        private void TestLeadtools(ref int totalSourceSize, ref int totalTargetSize)
        {
            FileInformation = string.Empty;
            totalSourceSize = 0;
            totalTargetSize = 0;

            fileSaver.SourceFilePath = SourceFilePath;
            fileSaver.TargetFilePath = TargetFilePath;

            foreach (var file in fileSaver.FilesToProcess)
            {
                var fileInfo = new FileInformation(file);
                var fileSize = fileInfo.GetFileSize();
                var pageCount = fileInfo.GetPageCount();
                var ext = fileInfo.GetFileExtension();
                var fileName = Path.GetFileName(file);

                totalSourceSize += fileSize;

                FileInformation += "Source file: " + fileName + " has " + pageCount.ToString() + " pages and " + fileSize.ToString() + sizeUOM + Environment.NewLine;

                //Conversion

                var destFile = string.Empty;

                if (_operation == OperationMode.WhichOperation.GrayScaleConversion)
                {
                    destFile = fileSaver.TargetFilePath + "LeadTools" + fileName;
                    leadToolsGrayscaleTransformer.ConvertToGrayScale(file, destFile);
                }
                else if (_operation == OperationMode.WhichOperation.TifToPDFConversion)
                {
                    destFile = fileSaver.TargetFilePath + "LeadTools" + fileName+".pdf";                   
                    leadToolsTifPdfTransformer.ConvertTifToPdf(file, destFile);
                }
                else if (_operation == OperationMode.WhichOperation.PDFToTifConversion)
                {
                    destFile = fileSaver.TargetFilePath + "LeadTools" + fileName + ".tif";
                    leadToolsTifPdfTransformer.ConvertPdfToTif(file, destFile);
                }
                else if (_operation == OperationMode.WhichOperation.DpiConversion)
                {
                    destFile = fileSaver.TargetFilePath + "LeadToolsDpi" + fileName;
                    leadToolsDpiTransformer.ConvertDpi(file, destFile);
                }

                var targetFileInfo = new FileInformation(destFile);
                var targetFileSize = targetFileInfo.GetFileSize();
                var targetPageCount = targetFileInfo.GetPageCount();
                var targetExt = targetFileInfo.GetFileExtension();

                totalTargetSize += targetFileSize;
                FileInformation += "Target file: " + destFile + " has " + targetPageCount.ToString() + " pages and " + targetFileSize.ToString() + sizeUOM + Environment.NewLine;
            }
        }          
        #endregion

        #region Atalasoft private methods
        private void TestAtalasoft(ref int totalSourceSize, ref int totalTargetSize)
        {
            if (_operation == OperationMode.WhichOperation.GrayScaleConversion)
            {
                FileInformation = string.Empty;
                totalSourceSize = 0;
                totalTargetSize = 0;

                fileSaver.SourceFilePath = SourceFilePath;
                fileSaver.TargetFilePath = TargetFilePath;

                foreach (var file in fileSaver.FilesToProcess)
                {
                    var fileInfo = new FileInformation(file);
                    var fileSize = fileInfo.GetFileSize();
                    var pageCount = fileInfo.GetPageCount();
                    var ext = fileInfo.GetFileExtension();
                    var fileName = Path.GetFileName(file);

                    totalSourceSize += fileSize;

                    FileInformation += "Source file: " + fileName + " has " + pageCount.ToString() + " pages and " + fileSize.ToString() + sizeUOM + Environment.NewLine;

                    //Conversion
                    var destFile = fileSaver.TargetFilePath + "Atalasoft" + fileName;
                    atalasoftGrayscaleTransformer.ConvertToGrayScale(file, destFile);

                    var targetFileInfo = new FileInformation(destFile);
                    var targetFileSize = targetFileInfo.GetFileSize();
                    var targetPageCount = targetFileInfo.GetPageCount();
                    var targetExt = targetFileInfo.GetFileExtension();

                    totalTargetSize += targetFileSize;
                    FileInformation += "Target file: " + destFile + " has " + targetPageCount.ToString() + " pages and " + targetFileSize.ToString() + sizeUOM + Environment.NewLine;
                }
            }
        } 
        #endregion

        #region general private help methods
        private TimeSpan GetStats(DateTime startTime, ref long totalMemoryUsed, long startWorkingSet)
        {
            var stopTime = DateTime.Now;
            TimeSpan timeTaken = stopTime - startTime;

            long endWorkingSet = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;
            totalMemoryUsed = (endWorkingSet - startWorkingSet) / sizeDivisor;
            return timeTaken;
        }

        private static void SetupStatsCollection(out DateTime startTime, out int totalSourceSize, out int totalTargetSize, out long totalMemoryUsed, out long startWorkingSet)
        {
            startTime = DateTime.Now;
            totalSourceSize = 0;
            totalTargetSize = 0;

            totalMemoryUsed = 0;

            startWorkingSet = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;
        }

        private void WriteToExcel(string library, string sourceSize, string targetSize, string reductionAmount, string totalMemoryUsed, string timeTaken, string operation)
        {
            Excel.Application excel = new Excel.Application();
            excel.Visible = true;
            Excel.Workbook wb = excel.Workbooks.Open(@"c:\TestImage\Stats.xlsx");
            Excel.Worksheet sh = wb.Sheets[1];//wb.Sheets.Add();
            int fullRow = sh.Rows.Count;
            int lastRow = sh.Cells[fullRow, 1].End(Excel.XlDirection.xlUp).Row;
            int rowToInsert = lastRow + 1;
            sh.Name = "Stats";
            sh.Cells[rowToInsert, "A"].Value2 = library;
            sh.Cells[rowToInsert, "B"].Value2 = sourceSize;
            sh.Cells[rowToInsert, "C"].Value2 = targetSize;
            sh.Cells[rowToInsert, "D"].Value2 = reductionAmount;
            sh.Cells[rowToInsert, "E"].Value2 = totalMemoryUsed;
            sh.Cells[rowToInsert, "F"].Value2 = timeTaken;
            sh.Cells[rowToInsert, "G"].Value2 = operation;
            sh.Cells[rowToInsert, "H"].Value2 = DateTime.Now.ToString();
            wb.Save();
            wb.Close(true);
            excel.Quit();
        }

        private void GetSummary(int totalSourceSize, int totalTargetSize, long totalMemoryused, TimeSpan timeTaken)
        {
            FileSummary = string.Empty;
            FileSummary += "Conversion took " + timeTaken.TotalSeconds.ToString() + " seconds." + Environment.NewLine;
            FileSummary += "Reduced file size from  " + totalSourceSize.ToString() + " to " + totalTargetSize.ToString() + sizeUOM + Environment.NewLine;
            FileSummary += "Reduction of  " + (totalSourceSize - totalTargetSize).ToString() + sizeUOM + Environment.NewLine;
            FileSummary += "Total memory usage: " + totalMemoryused.ToString() + sizeUOM + Environment.NewLine;
        }
        #endregion

        #region not used

        //Given a directoy with images,  
        //CombineMultipleImagesInOnePdf() combines into into one pdf doc
        //has multiframe capability  
        public void iTextSharpImageTransformer_CombineMultipleImagesInOnePdf(IITextSharpImageTransformer iTextSharpImageTransformer)
        {
            // var iTextSharpImageTransformer = new iTextSharpImageTransformer();
            iTextSharpImageTransformer.SourceFilePath = @"c:\TestImage\Source\gifImages\";
            iTextSharpImageTransformer.TargetFilePath = @"C:\TestImage\target\pdfs\";
            iTextSharpImageTransformer.TargetFileName = "pdfFile.pdf";
            iTextSharpImageTransformer.CombineMultipleImagesInOnePdf();
        }


        //Given a directory with images, SaveImages() copies them to another directory
        //has multiframe capability
        public void FileSaver_SaveImages(IFileSaver fileSaver)
        {
            fileSaver.SourceFilePath = @"c:\testImage\Source\OneColorTiff\";
            fileSaver.TargetFilePath = @"c:\testImage\Target\OneGrayScaleTiff\";

            foreach (var file in fileSaver.FilesToProcess)
            {
                Bitmap bitMap = (Bitmap)Image.FromFile(file);
                fileSaver.TargetFileName = Path.GetFileName(file);
                fileSaver.SaveImages(bitMap);
            }
        }


        public void TestCreateSingleImage()
        {
            string sourceDirectory = @"c:\testImage\source\singleImage.jpg";
            string targetDirectory = @"c:\testImage\target\";
            string targetImageExtension = ".tif";
            ImageFormat targetImageFormat = ImageFormat.Tiff;
            string targetImageName = "singleImage";
            long encoderParameterValue = 50L;

            var processor = new DotnetImageProcessor();
            processor.SourceDirectory = sourceDirectory;
            processor.TargetDirectory = targetDirectory;
            processor.TargetImageExtension = targetImageExtension;
            processor.TargetImageFormat = targetImageFormat;
            processor.TargetImageName = targetImageName;
            processor.EncoderParameterValue = encoderParameterValue;

            processor.CreateImage();
        }

        public void DisplayFiles()
        {
            var fileGetter = new FileGetter();
            fileGetter.FileDirectory = @"c:\testImage\source\";
            foreach (var file in fileGetter.GetFiles())
            {
                Console.WriteLine(file);
            }
        }
        #endregion
    }
}
