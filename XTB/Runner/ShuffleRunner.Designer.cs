namespace Innofactor.Crm.Shuffle.Runner
{
    partial class ShuffleRunner
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShuffleRunner));
            this.btnFile = new System.Windows.Forms.Button();
            this.txtFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.rbExport = new System.Windows.Forms.RadioButton();
            this.rbImport = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.txtParams = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnShuffle = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtData = new System.Windows.Forms.TextBox();
            this.btnData = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.tsbCloseThisTab = new System.Windows.Forms.ToolStripButton();
            this.lbLog = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.pbBlocks = new System.Windows.Forms.ProgressBar();
            this.pbRecords = new System.Windows.Forms.ProgressBar();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.toolStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnFile
            // 
            this.btnFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFile.Location = new System.Drawing.Point(393, 34);
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(29, 23);
            this.btnFile.TabIndex = 2;
            this.btnFile.Text = "...";
            this.btnFile.UseVisualStyleBackColor = true;
            this.btnFile.Click += new System.EventHandler(this.btnFile_Click);
            // 
            // txtFile
            // 
            this.txtFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFile.Location = new System.Drawing.Point(75, 36);
            this.txtFile.Name = "txtFile";
            this.txtFile.Size = new System.Drawing.Size(312, 20);
            this.txtFile.TabIndex = 1;
            this.txtFile.TextChanged += new System.EventHandler(this.txtFile_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Definition";
            // 
            // rbExport
            // 
            this.rbExport.AutoSize = true;
            this.rbExport.Checked = true;
            this.rbExport.Location = new System.Drawing.Point(75, 88);
            this.rbExport.Name = "rbExport";
            this.rbExport.Size = new System.Drawing.Size(55, 17);
            this.rbExport.TabIndex = 5;
            this.rbExport.TabStop = true;
            this.rbExport.Text = "Export";
            this.rbExport.UseVisualStyleBackColor = true;
            this.rbExport.CheckedChanged += new System.EventHandler(this.rbExport_CheckedChanged);
            // 
            // rbImport
            // 
            this.rbImport.AutoSize = true;
            this.rbImport.Location = new System.Drawing.Point(136, 88);
            this.rbImport.Name = "rbImport";
            this.rbImport.Size = new System.Drawing.Size(54, 17);
            this.rbImport.TabIndex = 6;
            this.rbImport.Text = "Import";
            this.rbImport.UseVisualStyleBackColor = true;
            this.rbImport.CheckedChanged += new System.EventHandler(this.rbExport_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 118);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Params";
            // 
            // txtParams
            // 
            this.txtParams.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtParams.Location = new System.Drawing.Point(75, 115);
            this.txtParams.Multiline = true;
            this.txtParams.Name = "txtParams";
            this.txtParams.Size = new System.Drawing.Size(347, 57);
            this.txtParams.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Action";
            // 
            // btnShuffle
            // 
            this.btnShuffle.Enabled = false;
            this.btnShuffle.Location = new System.Drawing.Point(147, 178);
            this.btnShuffle.Name = "btnShuffle";
            this.btnShuffle.Size = new System.Drawing.Size(160, 36);
            this.btnShuffle.TabIndex = 9;
            this.btnShuffle.Text = "-- Shuffle --";
            this.btnShuffle.UseVisualStyleBackColor = true;
            this.btnShuffle.Click += new System.EventHandler(this.btnShuffle_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 293);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Log";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Data";
            // 
            // txtData
            // 
            this.txtData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtData.Location = new System.Drawing.Point(75, 62);
            this.txtData.Name = "txtData";
            this.txtData.Size = new System.Drawing.Size(312, 20);
            this.txtData.TabIndex = 3;
            this.txtData.TextChanged += new System.EventHandler(this.txtData_TextChanged);
            // 
            // btnData
            // 
            this.btnData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnData.Location = new System.Drawing.Point(393, 60);
            this.btnData.Name = "btnData";
            this.btnData.Size = new System.Drawing.Size(29, 23);
            this.btnData.TabIndex = 4;
            this.btnData.Text = "...";
            this.btnData.UseVisualStyleBackColor = true;
            this.btnData.Click += new System.EventHandler(this.btnData_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Cinteros 100 transp.png");
            // 
            // toolStripMain
            // 
            this.toolStripMain.AutoSize = false;
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbCloseThisTab});
            this.toolStripMain.Location = new System.Drawing.Point(0, 0);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.Size = new System.Drawing.Size(432, 25);
            this.toolStripMain.TabIndex = 22;
            this.toolStripMain.Text = "toolStrip1";
            // 
            // tsbCloseThisTab
            // 
            this.tsbCloseThisTab.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCloseThisTab.Image = ((System.Drawing.Image)(resources.GetObject("tsbCloseThisTab.Image")));
            this.tsbCloseThisTab.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCloseThisTab.Name = "tsbCloseThisTab";
            this.tsbCloseThisTab.Size = new System.Drawing.Size(23, 22);
            this.tsbCloseThisTab.Text = "Close this tab";
            this.tsbCloseThisTab.Click += new System.EventHandler(this.tsbCloseThisTab_Click);
            // 
            // lbLog
            // 
            this.lbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbLog.FormattingEnabled = true;
            this.lbLog.Location = new System.Drawing.Point(75, 293);
            this.lbLog.Name = "lbLog";
            this.lbLog.Size = new System.Drawing.Size(347, 121);
            this.lbLog.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(211, 90);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 13);
            this.label6.TabIndex = 24;
            this.label6.Text = "Type";
            // 
            // cmbType
            // 
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new System.Drawing.Point(249, 88);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(118, 21);
            this.cmbType.TabIndex = 7;
            this.cmbType.SelectedIndexChanged += new System.EventHandler(this.cmbType_SelectedIndexChanged);
            // 
            // pbBlocks
            // 
            this.pbBlocks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbBlocks.Location = new System.Drawing.Point(75, 228);
            this.pbBlocks.Name = "pbBlocks";
            this.pbBlocks.Size = new System.Drawing.Size(347, 23);
            this.pbBlocks.Step = 1;
            this.pbBlocks.TabIndex = 25;
            // 
            // pbRecords
            // 
            this.pbRecords.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbRecords.Location = new System.Drawing.Point(75, 258);
            this.pbRecords.Name = "pbRecords";
            this.pbRecords.Size = new System.Drawing.Size(347, 23);
            this.pbRecords.Step = 1;
            this.pbRecords.TabIndex = 26;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 232);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(39, 13);
            this.label7.TabIndex = 27;
            this.label7.Text = "Blocks";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 262);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(47, 13);
            this.label8.TabIndex = 28;
            this.label8.Text = "Records";
            // 
            // ShuffleRunner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.pbRecords);
            this.Controls.Add(this.pbBlocks);
            this.Controls.Add(this.cmbType);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lbLog);
            this.Controls.Add(this.toolStripMain);
            this.Controls.Add(this.btnData);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtData);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnShuffle);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtParams);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.rbImport);
            this.Controls.Add(this.rbExport);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtFile);
            this.Controls.Add(this.btnFile);
            this.Name = "ShuffleRunner";
            this.Size = new System.Drawing.Size(432, 423);
            this.TabIcon = ((System.Drawing.Image)(resources.GetObject("$this.TabIcon")));
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFile;
        private System.Windows.Forms.TextBox txtFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbExport;
        private System.Windows.Forms.RadioButton rbImport;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtParams;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnShuffle;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtData;
        private System.Windows.Forms.Button btnData;
        private System.Windows.Forms.ImageList imageList1;
        internal System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripButton tsbCloseThisTab;
        private System.Windows.Forms.ListBox lbLog;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.ProgressBar pbBlocks;
        private System.Windows.Forms.ProgressBar pbRecords;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
    }
}
