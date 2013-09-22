using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImageProcessor.Interfaces;
using Ninject.Modules;

namespace ImageProcessor
{
    public class RunnerModule: NinjectModule
    {
        public override void Load()
        {
            string pdfInitialPath = @"C:\CodeStore\LeadToolsLibrary";

            Bind<IFileSaver>().To<FileSaver>();
            Bind<IFileGetter>().To<FileGetter>();
            
            Bind<IGrayscaleTransformer>().To<AtalasoftGrayscaleTransformer>();
            Bind<IGrayscaleTransformer>().To<DotnetGrayscaleTransformer>();
            Bind<IGrayscaleTransformer>().To<LeadtoolsGrayscaleTransformer>();
            Bind<IGrayscaleTransformer>().To<ImageMagickGrayscaleTransformer>();

            Bind<ITifPdfTransformer>().To<LeadtoolsTifPdfTransformer>().WithConstructorArgument("pdfInitialPath", pdfInitialPath);

            Bind<IDpiTransformer>().To<LeadtoolsDpiTransformer>();

            //Bind<IDotnetImageTransformer>().To<DotnetImageTransformer>();
            //Bind<ILeadToolsImageTransformer>().To<LeadToolsImageTransformer>();
            //Bind<IITextSharpImageTransformer>().To<ITextSharpImageTransformer>();
        }
    }
}
