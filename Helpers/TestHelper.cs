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
using Excel=Microsoft.Office.Interop.Excel;

namespace ImageProcessor.Helpers
{
    public class TestHelper
    {
        #region private fields
        StandardKernel kernel = null;
        IITextSharpImageTransformer iTextSharpImageTransformer = null;
        IFileSaver fileSaver = null;
        IFileGetter fileGetter = null;
        IDotnetImageTransformer dotNetImageTransformer = null;
        ILeadToolsImageTransformer leadToolsImageTransformer = null;
        IAtalasImageTransformer atalasImageTransformer = null;
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
            iTextSharpImageTransformer = kernel.Get<IITextSharpImageTransformer>();
            fileSaver = kernel.Get<IFileSaver>();
            fileGetter = kernel.Get<IFileGetter>();
            dotNetImageTransformer = kernel.Get<IDotnetImageTransformer>();
            leadToolsImageTransformer = kernel.Get<ILeadToolsImageTransformer>();
            atalasImageTransformer = kernel.Get<IAtalasImageTransformer>();

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
            //var fileSaver = new FileSaver();
            fileSaver.SourceFilePath = SourceFilePath;
            fileSaver.TargetFilePath = TargetFilePath;

            foreach (var file in fileSaver.FilesToProcess)
            {
                Bitmap bm = (Bitmap)Image.FromFile(file);
                iTextSharpImageTransformer.SaveEachImageAsOwnPdf(file);
            }
        }

        public void TestDotnet()
        {
            var startTime = DateTime.Now;
            var totalSourceSize = 0;
            var totalTargetSize = 0;

            long totalMemoryUsed = 0;

            long startWorkingSet = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;

            TestDotnetConvertToGrayScale(ref totalSourceSize, ref totalTargetSize);

            var stopTime = DateTime.Now;
            TimeSpan timeTaken = stopTime - startTime;

            long endWorkingSet = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;
            totalMemoryUsed = (endWorkingSet - startWorkingSet) / sizeDivisor;

            GetSummary(totalSourceSize, totalTargetSize, totalMemoryUsed, timeTaken);

            WriteToExcel("Dotnet", totalSourceSize.ToString(), totalTargetSize.ToString(), (totalSourceSize - totalTargetSize).ToString(), totalMemoryUsed.ToString(), timeTaken.TotalSeconds.ToString(), _operation.ToString());
        }

        public void TestLeadTools()
        {
            var totalSourceSize = 0;
            var totalTargetSize = 0;
            long totalMemoryUsed = 0;
            
            long startWorkingSet = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;

            var startTime = DateTime.Now;
            leadToolsImageTransformer.PdfInitialPath = @"C:\CodeStore\LeadToolsLibrary\";

            TestLeadtoolsConvertToGraySacle(ref totalSourceSize, ref totalTargetSize);

            var stopTime = DateTime.Now;
            TimeSpan timeTaken = stopTime - startTime;

            long endWorkingSet = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;
            totalMemoryUsed = (endWorkingSet - startWorkingSet) / sizeDivisor;

            GetSummary(totalSourceSize, totalTargetSize, totalMemoryUsed, timeTaken);
           
            WriteToExcel("Leadtools", totalSourceSize.ToString(), totalTargetSize.ToString(), (totalSourceSize - totalTargetSize).ToString(), totalMemoryUsed.ToString(), timeTaken.TotalSeconds.ToString(), _operation.ToString());
        }

        public void TestAtalasoft()
        {
            var totalSourceSize = 0;
            var totalTargetSize = 0;
            long totalMemoryUsed = 0;

            long startWorkingSet = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;

            var startTime = DateTime.Now;
            
            TestAtalasoftConvertToGrayScale(ref totalSourceSize, ref totalTargetSize);

            var stopTime = DateTime.Now;
            TimeSpan timeTaken = stopTime - startTime;

            long endWorkingSet = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;
            totalMemoryUsed = (endWorkingSet - startWorkingSet) / sizeDivisor;

            GetSummary(totalSourceSize, totalTargetSize, totalMemoryUsed, timeTaken);

            WriteToExcel("Atalasfot", totalSourceSize.ToString(), totalTargetSize.ToString(), (totalSourceSize - totalTargetSize).ToString(), totalMemoryUsed.ToString(), timeTaken.TotalSeconds.ToString(), _operation.ToString());
        }
        #endregion

        #region Dotnet private methods
        private void TestDotnetConvertToGrayScale(ref int totalSourceSize, ref int totalTargetSize)
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
                    Bitmap bm = (Bitmap)Image.FromFile(file);

                    var destFile = fileSaver.TargetFilePath + "Dotnet" + fileName;
                    List<Bitmap> bitMaps = dotNetImageTransformer.ConvertToGrayScale(bm, destFile, ResizeScale.HundredPercent);

                    fileSaver.TargetFileName = "Dotnet"+fileName;
                    fileSaver.SaveImages(bitMaps);


                    var targetFileInfo = new FileInformation(fileSaver.TargetFilePath + fileSaver.TargetFileName);
                    var targetFileSize = targetFileInfo.GetFileSize();
                    var targetPageCount = targetFileInfo.GetPageCount();
                    var targetExt = targetFileInfo.GetFileExtension();

                    totalTargetSize += targetFileSize;
                    FileInformation += "Target file: " + fileSaver.TargetFileName + " has " + targetPageCount.ToString() + " pages and " + targetFileSize.ToString() + sizeUOM + Environment.NewLine;
                }
            }
        }
        #endregion
     
        #region Leadtools private methods
        private void TestLeadtoolsConvertToGraySacle(ref int totalSourceSize, ref int totalTargetSize)
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
                    var destFile = fileSaver.TargetFilePath + "LeadTools" + fileName;
                    leadToolsImageTransformer.CopyTiffImage(file, destFile, true);
                    //leadToolsImageTransformer.ResizeTiffImage(file, destFile, true, ResizeScale.HundredPercent);

                    var targetFileInfo = new FileInformation(destFile);
                    var targetFileSize = targetFileInfo.GetFileSize();
                    var targetPageCount = targetFileInfo.GetPageCount();
                    var targetExt = targetFileInfo.GetFileExtension();

                    totalTargetSize += targetFileSize;
                    FileInformation += "Target file: LeadTools" + fileName + " has " + targetPageCount.ToString() + " pages and " + targetFileSize.ToString() + sizeUOM + Environment.NewLine;
                }
            }
        }          
        #endregion

        #region Atalasoft private methods
        private void TestAtalasoftConvertToGrayScale(ref int totalSourceSize, ref int totalTargetSize)
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
                    //leadToolsImageTransformer.CopyTiffImage(file, destFile, true);
                    atalasImageTransformer.ConvertToGrayScale(file, destFile);

                    var targetFileInfo = new FileInformation(destFile);
                    var targetFileSize = targetFileInfo.GetFileSize();
                    var targetPageCount = targetFileInfo.GetPageCount();
                    var targetExt = targetFileInfo.GetFileExtension();

                    totalTargetSize += targetFileSize;
                    FileInformation += "Target file: Atalasoft" + fileName + " has " + targetPageCount.ToString() + " pages and " + targetFileSize.ToString() + sizeUOM + Environment.NewLine;
                }
            }
        } 
        #endregion

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
