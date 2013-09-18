using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ImageProcessor.Helpers;
using ImageProcessor.Interfaces;
using Ninject;

namespace ImageProcessor
{
    class MainForm : Form
    {      
        MainFormViewModel vm = null;

        private Button btnLeadtools;
        private Button btnDotnet;
        private Button btnITextSharp;
        private Label label1;
        private GroupBox gbResults;
        private TextBox txtResults;
        private TextBox txtSummary;
        private FolderBrowserDialog sourceFilePathDialog;
        private FolderBrowserDialog targetFilePathDialog;
        private Button btnSelectSourcePath;
        private Button btnSelectTargetPath;
        private Label lblSourceFilePath;
        private Label lblTargetFilePath;
        private Button btnAtalasoft;
        private Label lblProcessing;
        private ComboBox cbOperationMode;
    
        MainForm()
        {
            InitializeComponent();

            vm = new MainFormViewModel();

            PopulateOperationModeComboBox();
        }

        static void Main()
        {
            Application.EnableVisualStyles();                
            Application.Run(new MainForm());
        }

        private void InitializeComponent()
        {
            this.cbOperationMode = new System.Windows.Forms.ComboBox();
            this.btnLeadtools = new System.Windows.Forms.Button();
            this.btnDotnet = new System.Windows.Forms.Button();
            this.btnITextSharp = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.gbResults = new System.Windows.Forms.GroupBox();
            this.txtSummary = new System.Windows.Forms.TextBox();
            this.txtResults = new System.Windows.Forms.TextBox();
            this.sourceFilePathDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.targetFilePathDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.btnSelectSourcePath = new System.Windows.Forms.Button();
            this.btnSelectTargetPath = new System.Windows.Forms.Button();
            this.lblSourceFilePath = new System.Windows.Forms.Label();
            this.lblTargetFilePath = new System.Windows.Forms.Label();
            this.btnAtalasoft = new System.Windows.Forms.Button();
            this.lblProcessing = new System.Windows.Forms.Label();
            this.gbResults.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbOperationMode
            // 
            this.cbOperationMode.FormattingEnabled = true;
            this.cbOperationMode.Location = new System.Drawing.Point(21, 49);
            this.cbOperationMode.Name = "cbOperationMode";
            this.cbOperationMode.Size = new System.Drawing.Size(154, 21);
            this.cbOperationMode.TabIndex = 0;
            // 
            // btnLeadtools
            // 
            this.btnLeadtools.Location = new System.Drawing.Point(20, 167);
            this.btnLeadtools.Name = "btnLeadtools";
            this.btnLeadtools.Size = new System.Drawing.Size(75, 25);
            this.btnLeadtools.TabIndex = 1;
            this.btnLeadtools.Text = "Leadtools";
            this.btnLeadtools.UseVisualStyleBackColor = true;
            this.btnLeadtools.Click += new System.EventHandler(this.btnLeadtools_Click);
            // 
            // btnDotnet
            // 
            this.btnDotnet.Location = new System.Drawing.Point(101, 167);
            this.btnDotnet.Name = "btnDotnet";
            this.btnDotnet.Size = new System.Drawing.Size(75, 25);
            this.btnDotnet.TabIndex = 2;
            this.btnDotnet.Text = ". Net";
            this.btnDotnet.UseVisualStyleBackColor = true;
            this.btnDotnet.Click += new System.EventHandler(this.btnDotnet_Click);
            // 
            // btnITextSharp
            // 
            this.btnITextSharp.Location = new System.Drawing.Point(645, 167);
            this.btnITextSharp.Name = "btnITextSharp";
            this.btnITextSharp.Size = new System.Drawing.Size(75, 25);
            this.btnITextSharp.TabIndex = 3;
            this.btnITextSharp.Text = "ITextSharp";
            this.btnITextSharp.UseVisualStyleBackColor = true;
            this.btnITextSharp.Click += new System.EventHandler(this.btnITextSharp_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(18, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(185, 17);
            this.label1.TabIndex = 7;
            this.label1.Text = "Type of image operation";
            // 
            // gbResults
            // 
            this.gbResults.Controls.Add(this.txtSummary);
            this.gbResults.Controls.Add(this.txtResults);
            this.gbResults.Location = new System.Drawing.Point(19, 198);
            this.gbResults.Name = "gbResults";
            this.gbResults.Size = new System.Drawing.Size(710, 486);
            this.gbResults.TabIndex = 8;
            this.gbResults.TabStop = false;
            this.gbResults.Text = "Results";
            // 
            // txtSummary
            // 
            this.txtSummary.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSummary.ForeColor = System.Drawing.Color.Red;
            this.txtSummary.Location = new System.Drawing.Point(6, 272);
            this.txtSummary.Multiline = true;
            this.txtSummary.Name = "txtSummary";
            this.txtSummary.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtSummary.Size = new System.Drawing.Size(695, 214);
            this.txtSummary.TabIndex = 6;
            // 
            // txtResults
            // 
            this.txtResults.Location = new System.Drawing.Point(6, 18);
            this.txtResults.Multiline = true;
            this.txtResults.Name = "txtResults";
            this.txtResults.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResults.Size = new System.Drawing.Size(695, 248);
            this.txtResults.TabIndex = 5;
            // 
            // btnSelectSourcePath
            // 
            this.btnSelectSourcePath.Location = new System.Drawing.Point(21, 76);
            this.btnSelectSourcePath.Name = "btnSelectSourcePath";
            this.btnSelectSourcePath.Size = new System.Drawing.Size(131, 23);
            this.btnSelectSourcePath.TabIndex = 11;
            this.btnSelectSourcePath.Text = "Select Source File Path";
            this.btnSelectSourcePath.UseVisualStyleBackColor = true;
            this.btnSelectSourcePath.Click += new System.EventHandler(this.btnSelectSourcePath_Click);
            // 
            // btnSelectTargetPath
            // 
            this.btnSelectTargetPath.Location = new System.Drawing.Point(21, 105);
            this.btnSelectTargetPath.Name = "btnSelectTargetPath";
            this.btnSelectTargetPath.Size = new System.Drawing.Size(131, 23);
            this.btnSelectTargetPath.TabIndex = 12;
            this.btnSelectTargetPath.Text = "Select Target File Path";
            this.btnSelectTargetPath.UseVisualStyleBackColor = true;
            this.btnSelectTargetPath.Click += new System.EventHandler(this.btnSelectTargetPath_Click);
            // 
            // lblSourceFilePath
            // 
            this.lblSourceFilePath.AutoSize = true;
            this.lblSourceFilePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSourceFilePath.Location = new System.Drawing.Point(201, 81);
            this.lblSourceFilePath.Name = "lblSourceFilePath";
            this.lblSourceFilePath.Size = new System.Drawing.Size(97, 13);
            this.lblSourceFilePath.TabIndex = 13;
            this.lblSourceFilePath.Text = "Source file path";
            // 
            // lblTargetFilePath
            // 
            this.lblTargetFilePath.AutoSize = true;
            this.lblTargetFilePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTargetFilePath.Location = new System.Drawing.Point(201, 110);
            this.lblTargetFilePath.Name = "lblTargetFilePath";
            this.lblTargetFilePath.Size = new System.Drawing.Size(94, 13);
            this.lblTargetFilePath.TabIndex = 14;
            this.lblTargetFilePath.Text = "Target file path";
            // 
            // btnAtalasoft
            // 
            this.btnAtalasoft.Location = new System.Drawing.Point(182, 167);
            this.btnAtalasoft.Name = "btnAtalasoft";
            this.btnAtalasoft.Size = new System.Drawing.Size(75, 25);
            this.btnAtalasoft.TabIndex = 15;
            this.btnAtalasoft.Text = "Atalasoft";
            this.btnAtalasoft.UseVisualStyleBackColor = true;
            this.btnAtalasoft.Click += new System.EventHandler(this.btnAtalasoft_Click);
            // 
            // lblProcessing
            // 
            this.lblProcessing.AutoSize = true;
            this.lblProcessing.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProcessing.ForeColor = System.Drawing.Color.Red;
            this.lblProcessing.Location = new System.Drawing.Point(373, 29);
            this.lblProcessing.Name = "lblProcessing";
            this.lblProcessing.Size = new System.Drawing.Size(165, 29);
            this.lblProcessing.TabIndex = 16;
            this.lblProcessing.Text = "Processing...";
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(761, 696);
            this.Controls.Add(this.lblProcessing);
            this.Controls.Add(this.btnAtalasoft);
            this.Controls.Add(this.lblTargetFilePath);
            this.Controls.Add(this.lblSourceFilePath);
            this.Controls.Add(this.btnSelectTargetPath);
            this.Controls.Add(this.btnSelectSourcePath);
            this.Controls.Add(this.gbResults);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnITextSharp);
            this.Controls.Add(this.btnDotnet);
            this.Controls.Add(this.btnLeadtools);
            this.Controls.Add(this.cbOperationMode);
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.gbResults.ResumeLayout(false);
            this.gbResults.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            HideTextAndIcon();
        }

        private void pbProcessing_LoadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            HideTextAndIcon();            
        }

        private void btnLeadtools_Click(object sender, EventArgs e)
        {
            ShowTextAndIcon();

            var operation = GetOperationMode();
         
            var testHelper = new TestHelper(operation);
            testHelper.SourceFilePath = vm.SourceFilePath;
            testHelper.TargetFilePath = vm.TargetFilePath;
            testHelper.TestLeadTools();

            HideTextAndIcon();

            txtResults.Text = testHelper.FileInformation;
            txtSummary.Text = testHelper.FileSummary;
        }

        private void btnDotnet_Click(object sender, EventArgs e)
        {
            ShowTextAndIcon();

            var operation = GetOperationMode();
            var testHelper = new TestHelper(operation);
            testHelper.SourceFilePath = vm.SourceFilePath;
            testHelper.TargetFilePath = vm.TargetFilePath;
            testHelper.TestDotnet();

            HideTextAndIcon();

            txtResults.Text = testHelper.FileInformation;
            txtSummary.Text = testHelper.FileSummary;
        }


        private void btnAtalasoft_Click(object sender, EventArgs e)
        {
            ShowTextAndIcon();
 
            var operation = GetOperationMode();
            var testHelper = new TestHelper(operation);
            testHelper.SourceFilePath = vm.SourceFilePath;
            testHelper.TargetFilePath = vm.TargetFilePath;
            testHelper.TestAtalasoft();

            HideTextAndIcon();

            txtResults.Text = testHelper.FileInformation;
            txtSummary.Text = testHelper.FileSummary;
        }

        private void btnITextSharp_Click(object sender, EventArgs e)
        {
            ShowTextAndIcon();
        
            var operation = GetOperationMode();
            var testHelper = new TestHelper(operation);
            testHelper.SourceFilePath = vm.SourceFilePath;
            testHelper.TargetFilePath = vm.TargetFilePath;
            testHelper.TestITextSharp();

            HideTextAndIcon();

            txtResults.Text = testHelper.FileInformation;
            txtSummary.Text = testHelper.FileSummary;
        }



        #region private methods
        private OperationMode.WhichOperation GetOperationMode()
        {
            var selectedItem = cbOperationMode.SelectedItem;
            var operation = ((OperationMode)selectedItem).Operation;
            return operation;
        }

        private void PopulateOperationModeComboBox()
        {
            cbOperationMode.DataSource = vm.OperationModeList;
            cbOperationMode.DisplayMember = "Operation";
        }

        private void HideTextAndIcon()
        {
            this.Text = "";
            this.Icon = null;
            lblProcessing.Hide();
            lblProcessing.Refresh();
        }

        private void ShowTextAndIcon()
        {
            this.Text = "Processing....";
            this.Icon = vm.Icon;
            lblProcessing.Show();
            lblProcessing.Refresh();
        }

        #endregion

        private void btnSelectSourcePath_Click(object sender, EventArgs e)
        {
            var folderPathDialog = new FolderBrowserDialog();
            folderPathDialog.SelectedPath = @"c:\testImage\source\";
            var result = STAShowDialog(folderPathDialog);
            if (result == DialogResult.OK)
            {
                vm.SourceFilePath = folderPathDialog.SelectedPath + @"\";
                lblSourceFilePath.Text = vm.SourceFilePath;
            }
        }

        private void btnSelectTargetPath_Click(object sender, EventArgs e)
        {
            var folderPathDialog = new FolderBrowserDialog();
            folderPathDialog.SelectedPath = @"c:\testImage\target\";
            var result = STAShowDialog(folderPathDialog);
  
            if (result == DialogResult.OK)
            {
                vm.TargetFilePath = folderPathDialog.SelectedPath+@"\";
                lblTargetFilePath.Text = vm.TargetFilePath;
            }
        }

        #region Thread safe dialog
        public class DialogState
        {
            public DialogResult result;
            public FolderBrowserDialog dialog;

            public void ThreadProcShowDialog()
            {
                result = dialog.ShowDialog();
            }
        }

        private DialogResult STAShowDialog(FolderBrowserDialog dialog)
        {
            DialogState state = new DialogState();
            state.dialog = dialog;
            System.Threading.Thread t = new System.Threading.Thread(state.ThreadProcShowDialog);
            t.SetApartmentState(System.Threading.ApartmentState.STA);
            t.Start();
            t.Join();
            return state.result;
        }
        #endregion



        //PDFFileSaver_SaveEachImageAsOwnPdf(pdfFileSaver);
        //PDFFileSaver_CombineMultipleImagesInOnePdf(pdfFileSaver); // 33 secs    --   8 jpegs, 1 18-page tiff and 2 -1-page tiffs   

        //FileSaver_SaveImages(fileSaver); // 5 secs
        //ImageTransfromer_ConvertToGrayScale(fileSaver, imageTransformer); //11 secs -- --8 jpegs, 1 1-page tiff, and 1 18-page tiff

        //new PDFSharpConverter().tiff2PDF(); //slow -- 65 secs
    }
}
