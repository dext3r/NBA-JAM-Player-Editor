namespace SimplePaletteQuantizer
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.dialogOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.panelStatistics = new System.Windows.Forms.Panel();
            this.splitContainerPngSizes = new System.Windows.Forms.SplitContainer();
            this.editProjectedPngSize = new System.Windows.Forms.TextBox();
            this.labelProjectedPngSize = new System.Windows.Forms.Label();
            this.editNewPngSize = new System.Windows.Forms.TextBox();
            this.labelNewPngSize = new System.Windows.Forms.Label();
            this.splitContainerGifSizes = new System.Windows.Forms.SplitContainer();
            this.editProjectedGifSize = new System.Windows.Forms.TextBox();
            this.labelProjectedGifSize = new System.Windows.Forms.Label();
            this.editNewGifSize = new System.Windows.Forms.TextBox();
            this.labelNewGifSize = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.splitterMain = new System.Windows.Forms.Splitter();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.panelSource = new System.Windows.Forms.Panel();
            this.listSource = new System.Windows.Forms.ComboBox();
            this.labelSource = new System.Windows.Forms.Label();
            this.pictureSource = new System.Windows.Forms.PictureBox();
            this.panelSourceInfo = new System.Windows.Forms.Panel();
            this.editSourceInfo = new System.Windows.Forms.TextBox();
            this.panelRight = new System.Windows.Forms.Panel();
            this.panelMethod = new System.Windows.Forms.Panel();
            this.listMethod = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.listColors = new System.Windows.Forms.ComboBox();
            this.labelMethod = new System.Windows.Forms.Label();
            this.pictureTarget = new System.Windows.Forms.PictureBox();
            this.panelTargetInfo = new System.Windows.Forms.Panel();
            this.editTargetInfo = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.panelStatistics.SuspendLayout();
            this.splitContainerPngSizes.Panel1.SuspendLayout();
            this.splitContainerPngSizes.Panel2.SuspendLayout();
            this.splitContainerPngSizes.SuspendLayout();
            this.splitContainerGifSizes.Panel1.SuspendLayout();
            this.splitContainerGifSizes.Panel2.SuspendLayout();
            this.splitContainerGifSizes.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.panelSource.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureSource)).BeginInit();
            this.panelSourceInfo.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.panelMethod.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTarget)).BeginInit();
            this.panelTargetInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonBrowse.Location = new System.Drawing.Point(5, 323);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(607, 30);
            this.buttonBrowse.TabIndex = 0;
            this.buttonBrowse.Text = "Browse for a file image...";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.ButtonBrowseClick);
            // 
            // dialogOpenFile
            // 
            this.dialogOpenFile.Filter = "Supported images|*.png;*.jpg;*.gif;*.jpeg;*.bmp;*.tiff";
            // 
            // panelStatistics
            // 
            this.panelStatistics.AutoSize = true;
            this.panelStatistics.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelStatistics.Controls.Add(this.splitContainerPngSizes);
            this.panelStatistics.Controls.Add(this.splitContainerGifSizes);
            this.panelStatistics.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelStatistics.Location = new System.Drawing.Point(5, 263);
            this.panelStatistics.Name = "panelStatistics";
            this.panelStatistics.Size = new System.Drawing.Size(607, 60);
            this.panelStatistics.TabIndex = 4;
            // 
            // splitContainerPngSizes
            // 
            this.splitContainerPngSizes.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitContainerPngSizes.Location = new System.Drawing.Point(0, 30);
            this.splitContainerPngSizes.Name = "splitContainerPngSizes";
            // 
            // splitContainerPngSizes.Panel1
            // 
            this.splitContainerPngSizes.Panel1.Controls.Add(this.editProjectedPngSize);
            this.splitContainerPngSizes.Panel1.Controls.Add(this.labelProjectedPngSize);
            this.splitContainerPngSizes.Panel1.Padding = new System.Windows.Forms.Padding(5);
            // 
            // splitContainerPngSizes.Panel2
            // 
            this.splitContainerPngSizes.Panel2.Controls.Add(this.editNewPngSize);
            this.splitContainerPngSizes.Panel2.Controls.Add(this.labelNewPngSize);
            this.splitContainerPngSizes.Panel2.Padding = new System.Windows.Forms.Padding(5);
            this.splitContainerPngSizes.Size = new System.Drawing.Size(607, 30);
            this.splitContainerPngSizes.SplitterDistance = 298;
            this.splitContainerPngSizes.TabIndex = 2;
            // 
            // editProjectedPngSize
            // 
            this.editProjectedPngSize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editProjectedPngSize.Location = new System.Drawing.Point(155, 5);
            this.editProjectedPngSize.Name = "editProjectedPngSize";
            this.editProjectedPngSize.ReadOnly = true;
            this.editProjectedPngSize.Size = new System.Drawing.Size(138, 20);
            this.editProjectedPngSize.TabIndex = 2;
            // 
            // labelProjectedPngSize
            // 
            this.labelProjectedPngSize.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelProjectedPngSize.Location = new System.Drawing.Point(5, 5);
            this.labelProjectedPngSize.Name = "labelProjectedPngSize";
            this.labelProjectedPngSize.Size = new System.Drawing.Size(150, 20);
            this.labelProjectedPngSize.TabIndex = 1;
            this.labelProjectedPngSize.Text = "Source file size (in bytes):";
            this.labelProjectedPngSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // editNewPngSize
            // 
            this.editNewPngSize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editNewPngSize.Location = new System.Drawing.Point(185, 5);
            this.editNewPngSize.Name = "editNewPngSize";
            this.editNewPngSize.ReadOnly = true;
            this.editNewPngSize.Size = new System.Drawing.Size(115, 20);
            this.editNewPngSize.TabIndex = 2;
            // 
            // labelNewPngSize
            // 
            this.labelNewPngSize.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelNewPngSize.Location = new System.Drawing.Point(5, 5);
            this.labelNewPngSize.Name = "labelNewPngSize";
            this.labelNewPngSize.Size = new System.Drawing.Size(180, 20);
            this.labelNewPngSize.TabIndex = 1;
            this.labelNewPngSize.Text = "New projected PNG size (in bytes):";
            this.labelNewPngSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // splitContainerGifSizes
            // 
            this.splitContainerGifSizes.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitContainerGifSizes.Location = new System.Drawing.Point(0, 0);
            this.splitContainerGifSizes.Name = "splitContainerGifSizes";
            // 
            // splitContainerGifSizes.Panel1
            // 
            this.splitContainerGifSizes.Panel1.Controls.Add(this.editProjectedGifSize);
            this.splitContainerGifSizes.Panel1.Controls.Add(this.labelProjectedGifSize);
            this.splitContainerGifSizes.Panel1.Padding = new System.Windows.Forms.Padding(5);
            // 
            // splitContainerGifSizes.Panel2
            // 
            this.splitContainerGifSizes.Panel2.Controls.Add(this.editNewGifSize);
            this.splitContainerGifSizes.Panel2.Controls.Add(this.labelNewGifSize);
            this.splitContainerGifSizes.Panel2.Padding = new System.Windows.Forms.Padding(5);
            this.splitContainerGifSizes.Size = new System.Drawing.Size(607, 30);
            this.splitContainerGifSizes.SplitterDistance = 298;
            this.splitContainerGifSizes.TabIndex = 1;
            // 
            // editProjectedGifSize
            // 
            this.editProjectedGifSize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editProjectedGifSize.Location = new System.Drawing.Point(155, 5);
            this.editProjectedGifSize.Name = "editProjectedGifSize";
            this.editProjectedGifSize.ReadOnly = true;
            this.editProjectedGifSize.Size = new System.Drawing.Size(138, 20);
            this.editProjectedGifSize.TabIndex = 2;
            // 
            // labelProjectedGifSize
            // 
            this.labelProjectedGifSize.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelProjectedGifSize.Location = new System.Drawing.Point(5, 5);
            this.labelProjectedGifSize.Name = "labelProjectedGifSize";
            this.labelProjectedGifSize.Size = new System.Drawing.Size(150, 20);
            this.labelProjectedGifSize.TabIndex = 1;
            this.labelProjectedGifSize.Text = "Projected GIF size (in bytes):";
            this.labelProjectedGifSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // editNewGifSize
            // 
            this.editNewGifSize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editNewGifSize.Location = new System.Drawing.Point(185, 5);
            this.editNewGifSize.Name = "editNewGifSize";
            this.editNewGifSize.ReadOnly = true;
            this.editNewGifSize.Size = new System.Drawing.Size(115, 20);
            this.editNewGifSize.TabIndex = 2;
            // 
            // labelNewGifSize
            // 
            this.labelNewGifSize.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelNewGifSize.Location = new System.Drawing.Point(5, 5);
            this.labelNewGifSize.Name = "labelNewGifSize";
            this.labelNewGifSize.Size = new System.Drawing.Size(180, 20);
            this.labelNewGifSize.TabIndex = 1;
            this.labelNewGifSize.Text = "New projected GIF size (in bytes):";
            this.labelNewGifSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.splitterMain);
            this.panelMain.Controls.Add(this.panelLeft);
            this.panelMain.Controls.Add(this.panelRight);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(5, 5);
            this.panelMain.Name = "panelMain";
            this.panelMain.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.panelMain.Size = new System.Drawing.Size(607, 258);
            this.panelMain.TabIndex = 5;
            // 
            // splitterMain
            // 
            this.splitterMain.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.splitterMain.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitterMain.Location = new System.Drawing.Point(295, 0);
            this.splitterMain.Name = "splitterMain";
            this.splitterMain.Size = new System.Drawing.Size(5, 253);
            this.splitterMain.TabIndex = 2;
            this.splitterMain.TabStop = false;
            // 
            // panelLeft
            // 
            this.panelLeft.Controls.Add(this.panelSource);
            this.panelLeft.Controls.Add(this.pictureSource);
            this.panelLeft.Controls.Add(this.panelSourceInfo);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(300, 253);
            this.panelLeft.TabIndex = 0;
            // 
            // panelSource
            // 
            this.panelSource.Controls.Add(this.listSource);
            this.panelSource.Controls.Add(this.labelSource);
            this.panelSource.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSource.Location = new System.Drawing.Point(0, 25);
            this.panelSource.Name = "panelSource";
            this.panelSource.Padding = new System.Windows.Forms.Padding(0, 0, 8, 5);
            this.panelSource.Size = new System.Drawing.Size(300, 25);
            this.panelSource.TabIndex = 9;
            // 
            // listSource
            // 
            this.listSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listSource.Enabled = false;
            this.listSource.FormattingEnabled = true;
            this.listSource.Items.AddRange(new object[] {
            "Original",
            "GIF (default)"});
            this.listSource.Location = new System.Drawing.Point(150, 0);
            this.listSource.Name = "listSource";
            this.listSource.Size = new System.Drawing.Size(142, 21);
            this.listSource.TabIndex = 9;
            this.listSource.SelectedIndexChanged += new System.EventHandler(this.ListSourceSelectedIndexChanged);
            // 
            // labelSource
            // 
            this.labelSource.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelSource.Location = new System.Drawing.Point(0, 0);
            this.labelSource.Name = "labelSource";
            this.labelSource.Size = new System.Drawing.Size(150, 20);
            this.labelSource.TabIndex = 8;
            this.labelSource.Text = "Select a source image:";
            this.labelSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureSource
            // 
            this.pictureSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureSource.Location = new System.Drawing.Point(0, 25);
            this.pictureSource.Name = "pictureSource";
            this.pictureSource.Size = new System.Drawing.Size(300, 228);
            this.pictureSource.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureSource.TabIndex = 8;
            this.pictureSource.TabStop = false;
            // 
            // panelSourceInfo
            // 
            this.panelSourceInfo.Controls.Add(this.editSourceInfo);
            this.panelSourceInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSourceInfo.Location = new System.Drawing.Point(0, 0);
            this.panelSourceInfo.Name = "panelSourceInfo";
            this.panelSourceInfo.Padding = new System.Windows.Forms.Padding(0, 0, 8, 5);
            this.panelSourceInfo.Size = new System.Drawing.Size(300, 25);
            this.panelSourceInfo.TabIndex = 7;
            // 
            // editSourceInfo
            // 
            this.editSourceInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editSourceInfo.Location = new System.Drawing.Point(0, 0);
            this.editSourceInfo.Name = "editSourceInfo";
            this.editSourceInfo.ReadOnly = true;
            this.editSourceInfo.Size = new System.Drawing.Size(292, 20);
            this.editSourceInfo.TabIndex = 8;
            this.editSourceInfo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.button1);
            this.panelRight.Controls.Add(this.panelMethod);
            this.panelRight.Controls.Add(this.pictureTarget);
            this.panelRight.Controls.Add(this.panelTargetInfo);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(300, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(307, 253);
            this.panelRight.TabIndex = 1;
            // 
            // panelMethod
            // 
            this.panelMethod.Controls.Add(this.listMethod);
            this.panelMethod.Controls.Add(this.label1);
            this.panelMethod.Controls.Add(this.listColors);
            this.panelMethod.Controls.Add(this.labelMethod);
            this.panelMethod.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelMethod.Location = new System.Drawing.Point(0, 25);
            this.panelMethod.Name = "panelMethod";
            this.panelMethod.Padding = new System.Windows.Forms.Padding(0, 0, 5, 5);
            this.panelMethod.Size = new System.Drawing.Size(307, 25);
            this.panelMethod.TabIndex = 10;
            // 
            // listMethod
            // 
            this.listMethod.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listMethod.Enabled = false;
            this.listMethod.FormattingEnabled = true;
            this.listMethod.Items.AddRange(new object[] {
            "HSB distinct filtering",
            "Uniform quantization",
            "Popularity algorithm",
            "Median cut algorithm",
            "Octree quantization"});
            this.listMethod.Location = new System.Drawing.Point(90, 0);
            this.listMethod.Name = "listMethod";
            this.listMethod.Size = new System.Drawing.Size(122, 21);
            this.listMethod.TabIndex = 13;
            this.listMethod.SelectedIndexChanged += new System.EventHandler(this.ListMethodSelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Right;
            this.label1.Location = new System.Drawing.Point(212, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 20);
            this.label1.TabIndex = 12;
            this.label1.Text = " Colors:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // listColors
            // 
            this.listColors.Dock = System.Windows.Forms.DockStyle.Right;
            this.listColors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listColors.Enabled = false;
            this.listColors.FormattingEnabled = true;
            this.listColors.Items.AddRange(new object[] {
            "2",
            "4",
            "8",
            "16",
            "32",
            "64",
            "128",
            "256"});
            this.listColors.Location = new System.Drawing.Point(257, 0);
            this.listColors.Name = "listColors";
            this.listColors.Size = new System.Drawing.Size(45, 21);
            this.listColors.TabIndex = 10;
            this.listColors.SelectedIndexChanged += new System.EventHandler(this.listColors_SelectedIndexChanged);
            // 
            // labelMethod
            // 
            this.labelMethod.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelMethod.Location = new System.Drawing.Point(0, 0);
            this.labelMethod.Name = "labelMethod";
            this.labelMethod.Size = new System.Drawing.Size(90, 20);
            this.labelMethod.TabIndex = 8;
            this.labelMethod.Text = "Select a method:";
            this.labelMethod.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureTarget
            // 
            this.pictureTarget.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureTarget.Location = new System.Drawing.Point(0, 25);
            this.pictureTarget.Name = "pictureTarget";
            this.pictureTarget.Size = new System.Drawing.Size(307, 228);
            this.pictureTarget.TabIndex = 7;
            this.pictureTarget.TabStop = false;
            // 
            // panelTargetInfo
            // 
            this.panelTargetInfo.Controls.Add(this.editTargetInfo);
            this.panelTargetInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTargetInfo.Location = new System.Drawing.Point(0, 0);
            this.panelTargetInfo.Name = "panelTargetInfo";
            this.panelTargetInfo.Padding = new System.Windows.Forms.Padding(4, 0, 0, 5);
            this.panelTargetInfo.Size = new System.Drawing.Size(307, 25);
            this.panelTargetInfo.TabIndex = 6;
            // 
            // editTargetInfo
            // 
            this.editTargetInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.editTargetInfo.Location = new System.Drawing.Point(4, 0);
            this.editTargetInfo.Name = "editTargetInfo";
            this.editTargetInfo.ReadOnly = true;
            this.editTargetInfo.Size = new System.Drawing.Size(303, 20);
            this.editTargetInfo.TabIndex = 4;
            this.editTargetInfo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(15, 198);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(617, 358);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelStatistics);
            this.Controls.Add(this.buttonBrowse);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Simple palette quantizer";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.panelStatistics.ResumeLayout(false);
            this.splitContainerPngSizes.Panel1.ResumeLayout(false);
            this.splitContainerPngSizes.Panel1.PerformLayout();
            this.splitContainerPngSizes.Panel2.ResumeLayout(false);
            this.splitContainerPngSizes.Panel2.PerformLayout();
            this.splitContainerPngSizes.ResumeLayout(false);
            this.splitContainerGifSizes.Panel1.ResumeLayout(false);
            this.splitContainerGifSizes.Panel1.PerformLayout();
            this.splitContainerGifSizes.Panel2.ResumeLayout(false);
            this.splitContainerGifSizes.Panel2.PerformLayout();
            this.splitContainerGifSizes.ResumeLayout(false);
            this.panelMain.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            this.panelSource.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureSource)).EndInit();
            this.panelSourceInfo.ResumeLayout(false);
            this.panelSourceInfo.PerformLayout();
            this.panelRight.ResumeLayout(false);
            this.panelMethod.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureTarget)).EndInit();
            this.panelTargetInfo.ResumeLayout(false);
            this.panelTargetInfo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.OpenFileDialog dialogOpenFile;
        private System.Windows.Forms.Panel panelStatistics;
        private System.Windows.Forms.SplitContainer splitContainerPngSizes;
        private System.Windows.Forms.SplitContainer splitContainerGifSizes;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Splitter splitterMain;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.PictureBox pictureSource;
        private System.Windows.Forms.Panel panelSourceInfo;
        private System.Windows.Forms.PictureBox pictureTarget;
        private System.Windows.Forms.Panel panelTargetInfo;
        private System.Windows.Forms.TextBox editTargetInfo;
        private System.Windows.Forms.TextBox editSourceInfo;
        private System.Windows.Forms.TextBox editProjectedPngSize;
        private System.Windows.Forms.Label labelProjectedPngSize;
        private System.Windows.Forms.TextBox editProjectedGifSize;
        private System.Windows.Forms.Label labelProjectedGifSize;
        private System.Windows.Forms.Label labelNewPngSize;
        private System.Windows.Forms.Label labelNewGifSize;
        private System.Windows.Forms.TextBox editNewPngSize;
        private System.Windows.Forms.TextBox editNewGifSize;
        private System.Windows.Forms.Panel panelSource;
        private System.Windows.Forms.ComboBox listSource;
        private System.Windows.Forms.Label labelSource;
        private System.Windows.Forms.Panel panelMethod;
        private System.Windows.Forms.Label labelMethod;
        private System.Windows.Forms.ComboBox listMethod;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox listColors;
        private System.Windows.Forms.Button button1;
    }
}

