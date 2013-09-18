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
            Bind<IITextSharpImageTransformer>().To<ITextSharpImageTransformer>();
            Bind<IFileSaver>().To<FileSaver>();
            Bind<IFileGetter>().To<FileGetter>();
            Bind<IDotnetImageTransformer>().To<DotnetImageTransformer>();
            Bind<ILeadToolsImageTransformer>().To<LeadToolsImageTransformer>();
            Bind<IAtalasImageTransformer>().To<AtalasImageTransformer>();
        }
    }
}
