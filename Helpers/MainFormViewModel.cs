using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ImageProcessor.Helpers
{
    public class MainFormViewModel
    {
        public MainFormViewModel()
        {
            PopulateOperationModeList();

            Icon = CreateIcon();
        }

        public Icon Icon { get; set; }

        public List<OperationMode> OperationModeList
        {
            get;
            set; 
        }

        public string SourceFilePath { get; set; }
        public string TargetFilePath { get; set; }

        #region private helpers
        private void PopulateOperationModeList()
        {
            if (OperationModeList == null)
                OperationModeList = new List<OperationMode>();

            foreach (var enumItem in Enum.GetValues(typeof(OperationMode.WhichOperation)))
            {
                var om = new OperationMode();
                om.Operation = (OperationMode.WhichOperation)enumItem;
                OperationModeList.Add(om);
            }
        }

        private static string GetAppFolder()
        {
            return new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;
        }

        Icon CreateIcon()
        {
            Bitmap bitmap = new Bitmap(Path.Combine(GetAppFolder(), @"Resources\processingBlue.gif"));
            Icon icon = null;
            using (Graphics gfx = Graphics.FromImage(bitmap))
            {
                gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                //gfx.FillEllipse(Brushes.YellowGreen, 0, 0, bitmap.Width, bitmap.Height);
                icon = Icon.FromHandle(bitmap.GetHicon());
            }
            return icon;
        }
        #endregion
    }
}
