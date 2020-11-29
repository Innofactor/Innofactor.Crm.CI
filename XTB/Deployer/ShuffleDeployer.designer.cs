using System;

namespace Innofactor.Crm.ShuffleDeployer
{
    partial class ShuffleDeployer
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShuffleDeployer));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPagePackage = new System.Windows.Forms.TabPage();
            this.lvModules = new System.Windows.Forms.ListView();
            this.hdrName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hdrDescr = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblModules = new System.Windows.Forms.Label();
            this.lblPackage = new System.Windows.Forms.Label();
            this.btnPackage = new System.Windows.Forms.Button();
            this.tabPageDeploy = new System.Windows.Forms.TabPage();
            this.lblDeploying = new System.Windows.Forms.Label();
            this.btnCancelDeploy = new System.Windows.Forms.Button();
            this.btnOpenLog = new System.Windows.Forms.Button();
            this.btnOpenWF = new System.Windows.Forms.Button();
            this.pbModule = new System.Windows.Forms.ProgressBar();
            this.pbBlock = new System.Windows.Forms.ProgressBar();
            this.pbRecord = new System.Windows.Forms.ProgressBar();
            this.lblDeployModules = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblTimer = new System.Windows.Forms.Label();
            this.lblDeployFailed = new System.Windows.Forms.Label();
            this.lblDeploySkipped = new System.Windows.Forms.Label();
            this.lblDeployDeleted = new System.Windows.Forms.Label();
            this.lblDeployUpdated = new System.Windows.Forms.Label();
            this.lblDeployCreated = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lbLog = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnStartDeploy = new System.Windows.Forms.Button();
            this.lblCurrItem = new System.Windows.Forms.Label();
            this.lblCurrBlock = new System.Windows.Forms.Label();
            this.lblCurrModule = new System.Windows.Forms.Label();
            this.tabPageBuild = new System.Windows.Forms.TabPage();
            this.cmbModuleType = new System.Windows.Forms.ComboBox();
            this.label22 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnShowDataFile = new System.Windows.Forms.Button();
            this.btnOpenDataFile = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.txtModuleDataFile = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtModuleDescr = new System.Windows.Forms.TextBox();
            this.btnZipIt = new System.Windows.Forms.Button();
            this.btnShowDefinition = new System.Windows.Forms.Button();
            this.btnOpenDefinition = new System.Windows.Forms.Button();
            this.chkModuleDefault = new System.Windows.Forms.CheckBox();
            this.btnDownModule = new System.Windows.Forms.Button();
            this.btnUpModule = new System.Windows.Forms.Button();
            this.btnSavePackage = new System.Windows.Forms.Button();
            this.lblBuildModules = new System.Windows.Forms.Label();
            this.chkModuleOptional = new System.Windows.Forms.CheckBox();
            this.btnDelModule = new System.Windows.Forms.Button();
            this.btnAddModule = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtModuleFile = new System.Windows.Forms.TextBox();
            this.txtModuleName = new System.Windows.Forms.TextBox();
            this.lbBuild = new System.Windows.Forms.ListBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.tabControl1.SuspendLayout();
            this.tabPagePackage.SuspendLayout();
            this.tabPageDeploy.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabPageBuild.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPagePackage);
            this.tabControl1.Controls.Add(this.tabPageDeploy);
            this.tabControl1.Controls.Add(this.tabPageBuild);
            this.tabControl1.ImageList = this.imageList1;
            this.tabControl1.ItemSize = new System.Drawing.Size(120, 40);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(717, 583);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPagePackage
            // 
            this.tabPagePackage.Controls.Add(this.lvModules);
            this.tabPagePackage.Controls.Add(this.lblModules);
            this.tabPagePackage.Controls.Add(this.lblPackage);
            this.tabPagePackage.Controls.Add(this.btnPackage);
            this.tabPagePackage.ImageIndex = 1;
            this.tabPagePackage.Location = new System.Drawing.Point(4, 44);
            this.tabPagePackage.Name = "tabPagePackage";
            this.tabPagePackage.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePackage.Size = new System.Drawing.Size(709, 535);
            this.tabPagePackage.TabIndex = 1;
            this.tabPagePackage.Text = "Select package and modules";
            this.tabPagePackage.UseVisualStyleBackColor = true;
            // 
            // lvModules
            // 
            this.lvModules.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvModules.CheckBoxes = true;
            this.lvModules.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.hdrName,
            this.hdrDescr});
            this.lvModules.HideSelection = false;
            this.lvModules.Location = new System.Drawing.Point(18, 75);
            this.lvModules.Name = "lvModules";
            this.lvModules.Size = new System.Drawing.Size(678, 407);
            this.lvModules.TabIndex = 4;
            this.lvModules.UseCompatibleStateImageBehavior = false;
            this.lvModules.View = System.Windows.Forms.View.Details;
            this.lvModules.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvModules_ItemChecked);
            // 
            // hdrName
            // 
            this.hdrName.Text = "Name";
            this.hdrName.Width = 217;
            // 
            // hdrDescr
            // 
            this.hdrDescr.Text = "Description";
            this.hdrDescr.Width = 390;
            // 
            // lblModules
            // 
            this.lblModules.AutoSize = true;
            this.lblModules.Location = new System.Drawing.Point(18, 56);
            this.lblModules.Name = "lblModules";
            this.lblModules.Size = new System.Drawing.Size(79, 13);
            this.lblModules.TabIndex = 3;
            this.lblModules.Text = "Select modules";
            // 
            // lblPackage
            // 
            this.lblPackage.AutoSize = true;
            this.lblPackage.Location = new System.Drawing.Point(99, 22);
            this.lblPackage.Name = "lblPackage";
            this.lblPackage.Size = new System.Drawing.Size(111, 13);
            this.lblPackage.TabIndex = 1;
            this.lblPackage.Text = "Package not selected";
            // 
            // btnPackage
            // 
            this.btnPackage.Location = new System.Drawing.Point(18, 17);
            this.btnPackage.Name = "btnPackage";
            this.btnPackage.Size = new System.Drawing.Size(75, 23);
            this.btnPackage.TabIndex = 0;
            this.btnPackage.Text = "Open...";
            this.btnPackage.UseVisualStyleBackColor = true;
            this.btnPackage.Click += new System.EventHandler(this.btnPackage_Click);
            // 
            // tabPageDeploy
            // 
            this.tabPageDeploy.Controls.Add(this.lblDeploying);
            this.tabPageDeploy.Controls.Add(this.btnCancelDeploy);
            this.tabPageDeploy.Controls.Add(this.btnOpenLog);
            this.tabPageDeploy.Controls.Add(this.btnOpenWF);
            this.tabPageDeploy.Controls.Add(this.pbModule);
            this.tabPageDeploy.Controls.Add(this.pbBlock);
            this.tabPageDeploy.Controls.Add(this.pbRecord);
            this.tabPageDeploy.Controls.Add(this.lblDeployModules);
            this.tabPageDeploy.Controls.Add(this.label3);
            this.tabPageDeploy.Controls.Add(this.pictureBox1);
            this.tabPageDeploy.Controls.Add(this.lblTimer);
            this.tabPageDeploy.Controls.Add(this.lblDeployFailed);
            this.tabPageDeploy.Controls.Add(this.lblDeploySkipped);
            this.tabPageDeploy.Controls.Add(this.lblDeployDeleted);
            this.tabPageDeploy.Controls.Add(this.lblDeployUpdated);
            this.tabPageDeploy.Controls.Add(this.lblDeployCreated);
            this.tabPageDeploy.Controls.Add(this.label11);
            this.tabPageDeploy.Controls.Add(this.label10);
            this.tabPageDeploy.Controls.Add(this.label9);
            this.tabPageDeploy.Controls.Add(this.label8);
            this.tabPageDeploy.Controls.Add(this.label7);
            this.tabPageDeploy.Controls.Add(this.lbLog);
            this.tabPageDeploy.Controls.Add(this.label6);
            this.tabPageDeploy.Controls.Add(this.label5);
            this.tabPageDeploy.Controls.Add(this.label4);
            this.tabPageDeploy.Controls.Add(this.btnStartDeploy);
            this.tabPageDeploy.Controls.Add(this.lblCurrItem);
            this.tabPageDeploy.Controls.Add(this.lblCurrBlock);
            this.tabPageDeploy.Controls.Add(this.lblCurrModule);
            this.tabPageDeploy.ImageIndex = 3;
            this.tabPageDeploy.Location = new System.Drawing.Point(4, 44);
            this.tabPageDeploy.Name = "tabPageDeploy";
            this.tabPageDeploy.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDeploy.Size = new System.Drawing.Size(709, 535);
            this.tabPageDeploy.TabIndex = 3;
            this.tabPageDeploy.Text = "Deploy package";
            this.tabPageDeploy.UseVisualStyleBackColor = true;
            // 
            // lblDeploying
            // 
            this.lblDeploying.AutoSize = true;
            this.lblDeploying.Location = new System.Drawing.Point(99, 22);
            this.lblDeploying.Name = "lblDeploying";
            this.lblDeploying.Size = new System.Drawing.Size(35, 13);
            this.lblDeploying.TabIndex = 73;
            this.lblDeploying.Text = "<idle>";
            // 
            // btnCancelDeploy
            // 
            this.btnCancelDeploy.Enabled = false;
            this.btnCancelDeploy.Location = new System.Drawing.Point(392, 17);
            this.btnCancelDeploy.Name = "btnCancelDeploy";
            this.btnCancelDeploy.Size = new System.Drawing.Size(75, 23);
            this.btnCancelDeploy.TabIndex = 44;
            this.btnCancelDeploy.Text = "Cancel";
            this.btnCancelDeploy.UseVisualStyleBackColor = true;
            this.btnCancelDeploy.Visible = false;
            this.btnCancelDeploy.Click += new System.EventHandler(this.btnCancelDeploy_Click);
            // 
            // btnOpenLog
            // 
            this.btnOpenLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenLog.Enabled = false;
            this.btnOpenLog.Location = new System.Drawing.Point(414, 501);
            this.btnOpenLog.Name = "btnOpenLog";
            this.btnOpenLog.Size = new System.Drawing.Size(113, 23);
            this.btnOpenLog.TabIndex = 72;
            this.btnOpenLog.Text = "Open log file";
            this.btnOpenLog.UseVisualStyleBackColor = true;
            this.btnOpenLog.Click += new System.EventHandler(this.btnOpenLog_Click);
            // 
            // btnOpenWF
            // 
            this.btnOpenWF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenWF.Enabled = false;
            this.btnOpenWF.Location = new System.Drawing.Point(533, 501);
            this.btnOpenWF.Name = "btnOpenWF";
            this.btnOpenWF.Size = new System.Drawing.Size(144, 23);
            this.btnOpenWF.TabIndex = 71;
            this.btnOpenWF.Text = "Open working folder";
            this.btnOpenWF.UseVisualStyleBackColor = true;
            this.btnOpenWF.Click += new System.EventHandler(this.btnOpenWF_Click);
            // 
            // pbModule
            // 
            this.pbModule.Location = new System.Drawing.Point(100, 59);
            this.pbModule.Name = "pbModule";
            this.pbModule.Size = new System.Drawing.Size(283, 11);
            this.pbModule.Step = 1;
            this.pbModule.TabIndex = 29;
            // 
            // pbBlock
            // 
            this.pbBlock.Location = new System.Drawing.Point(100, 78);
            this.pbBlock.Name = "pbBlock";
            this.pbBlock.Size = new System.Drawing.Size(283, 12);
            this.pbBlock.Step = 1;
            this.pbBlock.TabIndex = 27;
            // 
            // pbRecord
            // 
            this.pbRecord.Location = new System.Drawing.Point(100, 97);
            this.pbRecord.Name = "pbRecord";
            this.pbRecord.Size = new System.Drawing.Size(283, 11);
            this.pbRecord.Step = 1;
            this.pbRecord.TabIndex = 28;
            // 
            // lblDeployModules
            // 
            this.lblDeployModules.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDeployModules.Location = new System.Drawing.Point(626, 24);
            this.lblDeployModules.Name = "lblDeployModules";
            this.lblDeployModules.Size = new System.Drawing.Size(50, 13);
            this.lblDeployModules.TabIndex = 67;
            this.lblDeployModules.Text = "0";
            this.lblDeployModules.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(573, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 66;
            this.label3.Text = "Modules:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(513, 15);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(34, 35);
            this.pictureBox1.TabIndex = 65;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // lblTimer
            // 
            this.lblTimer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTimer.Location = new System.Drawing.Point(411, 22);
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Size = new System.Drawing.Size(100, 18);
            this.lblTimer.TabIndex = 64;
            this.lblTimer.Text = "0:00";
            this.lblTimer.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblDeployFailed
            // 
            this.lblDeployFailed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDeployFailed.Location = new System.Drawing.Point(626, 99);
            this.lblDeployFailed.Name = "lblDeployFailed";
            this.lblDeployFailed.Size = new System.Drawing.Size(50, 13);
            this.lblDeployFailed.TabIndex = 63;
            this.lblDeployFailed.Text = "0";
            this.lblDeployFailed.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblDeploySkipped
            // 
            this.lblDeploySkipped.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDeploySkipped.Location = new System.Drawing.Point(626, 84);
            this.lblDeploySkipped.Name = "lblDeploySkipped";
            this.lblDeploySkipped.Size = new System.Drawing.Size(50, 13);
            this.lblDeploySkipped.TabIndex = 62;
            this.lblDeploySkipped.Text = "0";
            this.lblDeploySkipped.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblDeployDeleted
            // 
            this.lblDeployDeleted.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDeployDeleted.Location = new System.Drawing.Point(626, 69);
            this.lblDeployDeleted.Name = "lblDeployDeleted";
            this.lblDeployDeleted.Size = new System.Drawing.Size(50, 13);
            this.lblDeployDeleted.TabIndex = 61;
            this.lblDeployDeleted.Text = "0";
            this.lblDeployDeleted.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblDeployUpdated
            // 
            this.lblDeployUpdated.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDeployUpdated.Location = new System.Drawing.Point(626, 54);
            this.lblDeployUpdated.Name = "lblDeployUpdated";
            this.lblDeployUpdated.Size = new System.Drawing.Size(50, 13);
            this.lblDeployUpdated.TabIndex = 60;
            this.lblDeployUpdated.Text = "0";
            this.lblDeployUpdated.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblDeployCreated
            // 
            this.lblDeployCreated.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDeployCreated.Location = new System.Drawing.Point(626, 39);
            this.lblDeployCreated.Name = "lblDeployCreated";
            this.lblDeployCreated.Size = new System.Drawing.Size(50, 13);
            this.lblDeployCreated.TabIndex = 59;
            this.lblDeployCreated.Text = "0";
            this.lblDeployCreated.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(573, 99);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(38, 13);
            this.label11.TabIndex = 58;
            this.label11.Text = "Failed:";
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(573, 84);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(49, 13);
            this.label10.TabIndex = 57;
            this.label10.Text = "Skipped:";
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(573, 69);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(47, 13);
            this.label9.TabIndex = 56;
            this.label9.Text = "Deleted:";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(573, 54);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(51, 13);
            this.label8.TabIndex = 55;
            this.label8.Text = "Updated:";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(573, 39);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 13);
            this.label7.TabIndex = 54;
            this.label7.Text = "Created:";
            // 
            // lbLog
            // 
            this.lbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbLog.FormattingEnabled = true;
            this.lbLog.Location = new System.Drawing.Point(21, 125);
            this.lbLog.Name = "lbLog";
            this.lbLog.Size = new System.Drawing.Size(656, 368);
            this.lbLog.TabIndex = 33;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(26, 95);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(27, 13);
            this.label6.TabIndex = 32;
            this.label6.Text = "Item";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(26, 76);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 31;
            this.label5.Text = "Block";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(26, 57);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 30;
            this.label4.Text = "Module";
            // 
            // btnStartDeploy
            // 
            this.btnStartDeploy.Location = new System.Drawing.Point(18, 17);
            this.btnStartDeploy.Name = "btnStartDeploy";
            this.btnStartDeploy.Size = new System.Drawing.Size(75, 23);
            this.btnStartDeploy.TabIndex = 0;
            this.btnStartDeploy.Text = "Deploy";
            this.btnStartDeploy.UseVisualStyleBackColor = true;
            this.btnStartDeploy.Click += new System.EventHandler(this.btnDeploy_Click);
            // 
            // lblCurrItem
            // 
            this.lblCurrItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrItem.Location = new System.Drawing.Point(389, 95);
            this.lblCurrItem.Name = "lblCurrItem";
            this.lblCurrItem.Size = new System.Drawing.Size(178, 13);
            this.lblCurrItem.TabIndex = 70;
            this.lblCurrItem.Text = "<none>";
            // 
            // lblCurrBlock
            // 
            this.lblCurrBlock.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrBlock.Location = new System.Drawing.Point(389, 76);
            this.lblCurrBlock.Name = "lblCurrBlock";
            this.lblCurrBlock.Size = new System.Drawing.Size(178, 13);
            this.lblCurrBlock.TabIndex = 69;
            this.lblCurrBlock.Text = "<none>";
            // 
            // lblCurrModule
            // 
            this.lblCurrModule.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrModule.Location = new System.Drawing.Point(389, 57);
            this.lblCurrModule.Name = "lblCurrModule";
            this.lblCurrModule.Size = new System.Drawing.Size(178, 13);
            this.lblCurrModule.TabIndex = 68;
            this.lblCurrModule.Text = "<none>";
            // 
            // tabPageBuild
            // 
            this.tabPageBuild.Controls.Add(this.cmbModuleType);
            this.tabPageBuild.Controls.Add(this.label22);
            this.tabPageBuild.Controls.Add(this.label1);
            this.tabPageBuild.Controls.Add(this.btnShowDataFile);
            this.tabPageBuild.Controls.Add(this.btnOpenDataFile);
            this.tabPageBuild.Controls.Add(this.label13);
            this.tabPageBuild.Controls.Add(this.txtModuleDataFile);
            this.tabPageBuild.Controls.Add(this.label12);
            this.tabPageBuild.Controls.Add(this.txtModuleDescr);
            this.tabPageBuild.Controls.Add(this.btnZipIt);
            this.tabPageBuild.Controls.Add(this.btnShowDefinition);
            this.tabPageBuild.Controls.Add(this.btnOpenDefinition);
            this.tabPageBuild.Controls.Add(this.chkModuleDefault);
            this.tabPageBuild.Controls.Add(this.btnDownModule);
            this.tabPageBuild.Controls.Add(this.btnUpModule);
            this.tabPageBuild.Controls.Add(this.btnSavePackage);
            this.tabPageBuild.Controls.Add(this.lblBuildModules);
            this.tabPageBuild.Controls.Add(this.chkModuleOptional);
            this.tabPageBuild.Controls.Add(this.btnDelModule);
            this.tabPageBuild.Controls.Add(this.btnAddModule);
            this.tabPageBuild.Controls.Add(this.label2);
            this.tabPageBuild.Controls.Add(this.txtModuleFile);
            this.tabPageBuild.Controls.Add(this.txtModuleName);
            this.tabPageBuild.Controls.Add(this.lbBuild);
            this.tabPageBuild.ImageIndex = 4;
            this.tabPageBuild.Location = new System.Drawing.Point(4, 44);
            this.tabPageBuild.Name = "tabPageBuild";
            this.tabPageBuild.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageBuild.Size = new System.Drawing.Size(709, 535);
            this.tabPageBuild.TabIndex = 4;
            this.tabPageBuild.Text = "Build deploy packages";
            this.tabPageBuild.UseVisualStyleBackColor = true;
            // 
            // cmbModuleType
            // 
            this.cmbModuleType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbModuleType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbModuleType.FormattingEnabled = true;
            this.cmbModuleType.Items.AddRange(new object[] {
            "Shuffle Definition",
            "SQL Script"});
            this.cmbModuleType.Location = new System.Drawing.Point(435, 38);
            this.cmbModuleType.Name = "cmbModuleType";
            this.cmbModuleType.Size = new System.Drawing.Size(268, 21);
            this.cmbModuleType.TabIndex = 1;
            // 
            // label22
            // 
            this.label22.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(432, 22);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(65, 13);
            this.label22.TabIndex = 22;
            this.label22.Text = "Module type";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(432, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Module name";
            // 
            // btnShowDataFile
            // 
            this.btnShowDataFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShowDataFile.Image = ((System.Drawing.Image)(resources.GetObject("btnShowDataFile.Image")));
            this.btnShowDataFile.Location = new System.Drawing.Point(681, 167);
            this.btnShowDataFile.Name = "btnShowDataFile";
            this.btnShowDataFile.Size = new System.Drawing.Size(24, 22);
            this.btnShowDataFile.TabIndex = 8;
            this.btnShowDataFile.UseVisualStyleBackColor = true;
            this.btnShowDataFile.Click += new System.EventHandler(this.btnShowDataFile_Click);
            // 
            // btnOpenDataFile
            // 
            this.btnOpenDataFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenDataFile.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenDataFile.Image")));
            this.btnOpenDataFile.Location = new System.Drawing.Point(651, 166);
            this.btnOpenDataFile.Name = "btnOpenDataFile";
            this.btnOpenDataFile.Size = new System.Drawing.Size(24, 22);
            this.btnOpenDataFile.TabIndex = 7;
            this.btnOpenDataFile.UseVisualStyleBackColor = true;
            this.btnOpenDataFile.Click += new System.EventHandler(this.btnOpenDataFile_Click);
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(432, 151);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(46, 13);
            this.label13.TabIndex = 19;
            this.label13.Text = "Data file";
            // 
            // txtModuleDataFile
            // 
            this.txtModuleDataFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtModuleDataFile.Location = new System.Drawing.Point(435, 167);
            this.txtModuleDataFile.Name = "txtModuleDataFile";
            this.txtModuleDataFile.Size = new System.Drawing.Size(210, 20);
            this.txtModuleDataFile.TabIndex = 6;
            this.txtModuleDataFile.TextChanged += new System.EventHandler(this.buildModuleInfo_Changed);
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(432, 233);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(60, 13);
            this.label12.TabIndex = 17;
            this.label12.Text = "Description";
            // 
            // txtModuleDescr
            // 
            this.txtModuleDescr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtModuleDescr.Location = new System.Drawing.Point(435, 249);
            this.txtModuleDescr.Multiline = true;
            this.txtModuleDescr.Name = "txtModuleDescr";
            this.txtModuleDescr.Size = new System.Drawing.Size(268, 235);
            this.txtModuleDescr.TabIndex = 11;
            this.txtModuleDescr.TextChanged += new System.EventHandler(this.buildModuleInfo_Changed);
            // 
            // btnZipIt
            // 
            this.btnZipIt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZipIt.Location = new System.Drawing.Point(567, 496);
            this.btnZipIt.Name = "btnZipIt";
            this.btnZipIt.Size = new System.Drawing.Size(108, 23);
            this.btnZipIt.TabIndex = 17;
            this.btnZipIt.Text = "Zip it!";
            this.btnZipIt.UseVisualStyleBackColor = true;
            this.btnZipIt.Click += new System.EventHandler(this.btnZipIt_Click);
            // 
            // btnShowDefinition
            // 
            this.btnShowDefinition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShowDefinition.Image = ((System.Drawing.Image)(resources.GetObject("btnShowDefinition.Image")));
            this.btnShowDefinition.Location = new System.Drawing.Point(681, 124);
            this.btnShowDefinition.Name = "btnShowDefinition";
            this.btnShowDefinition.Size = new System.Drawing.Size(24, 22);
            this.btnShowDefinition.TabIndex = 5;
            this.btnShowDefinition.UseVisualStyleBackColor = true;
            this.btnShowDefinition.Click += new System.EventHandler(this.btnShowDefinition_Click);
            // 
            // btnOpenDefinition
            // 
            this.btnOpenDefinition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenDefinition.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenDefinition.Image")));
            this.btnOpenDefinition.Location = new System.Drawing.Point(651, 123);
            this.btnOpenDefinition.Name = "btnOpenDefinition";
            this.btnOpenDefinition.Size = new System.Drawing.Size(24, 22);
            this.btnOpenDefinition.TabIndex = 4;
            this.btnOpenDefinition.UseVisualStyleBackColor = true;
            this.btnOpenDefinition.Click += new System.EventHandler(this.btnOpenDefinition_Click);
            // 
            // chkModuleDefault
            // 
            this.chkModuleDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkModuleDefault.AutoSize = true;
            this.chkModuleDefault.Location = new System.Drawing.Point(435, 202);
            this.chkModuleDefault.Name = "chkModuleDefault";
            this.chkModuleDefault.Size = new System.Drawing.Size(60, 17);
            this.chkModuleDefault.TabIndex = 9;
            this.chkModuleDefault.Text = "Default";
            this.chkModuleDefault.UseVisualStyleBackColor = true;
            this.chkModuleDefault.CheckedChanged += new System.EventHandler(this.buildModuleInfo_Changed);
            // 
            // btnDownModule
            // 
            this.btnDownModule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDownModule.Location = new System.Drawing.Point(236, 496);
            this.btnDownModule.Name = "btnDownModule";
            this.btnDownModule.Size = new System.Drawing.Size(48, 23);
            this.btnDownModule.TabIndex = 15;
            this.btnDownModule.Text = "Down";
            this.btnDownModule.UseVisualStyleBackColor = true;
            this.btnDownModule.Click += new System.EventHandler(this.btnDownModule_Click);
            // 
            // btnUpModule
            // 
            this.btnUpModule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUpModule.Location = new System.Drawing.Point(182, 496);
            this.btnUpModule.Name = "btnUpModule";
            this.btnUpModule.Size = new System.Drawing.Size(48, 23);
            this.btnUpModule.TabIndex = 14;
            this.btnUpModule.Text = "Up";
            this.btnUpModule.UseVisualStyleBackColor = true;
            this.btnUpModule.Click += new System.EventHandler(this.btnUpModule_Click);
            // 
            // btnSavePackage
            // 
            this.btnSavePackage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSavePackage.Location = new System.Drawing.Point(435, 496);
            this.btnSavePackage.Name = "btnSavePackage";
            this.btnSavePackage.Size = new System.Drawing.Size(125, 23);
            this.btnSavePackage.TabIndex = 16;
            this.btnSavePackage.Text = "Save Package...";
            this.btnSavePackage.UseVisualStyleBackColor = true;
            this.btnSavePackage.Click += new System.EventHandler(this.btnSavePackage_Click);
            // 
            // lblBuildModules
            // 
            this.lblBuildModules.AutoSize = true;
            this.lblBuildModules.Location = new System.Drawing.Point(15, 22);
            this.lblBuildModules.Name = "lblBuildModules";
            this.lblBuildModules.Size = new System.Drawing.Size(47, 13);
            this.lblBuildModules.TabIndex = 9;
            this.lblBuildModules.Text = "Modules";
            // 
            // chkModuleOptional
            // 
            this.chkModuleOptional.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkModuleOptional.AutoSize = true;
            this.chkModuleOptional.Location = new System.Drawing.Point(545, 202);
            this.chkModuleOptional.Name = "chkModuleOptional";
            this.chkModuleOptional.Size = new System.Drawing.Size(65, 17);
            this.chkModuleOptional.TabIndex = 10;
            this.chkModuleOptional.Text = "Optional";
            this.chkModuleOptional.UseVisualStyleBackColor = true;
            this.chkModuleOptional.CheckedChanged += new System.EventHandler(this.buildModuleInfo_Changed);
            // 
            // btnDelModule
            // 
            this.btnDelModule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDelModule.Location = new System.Drawing.Point(100, 496);
            this.btnDelModule.Name = "btnDelModule";
            this.btnDelModule.Size = new System.Drawing.Size(75, 23);
            this.btnDelModule.TabIndex = 13;
            this.btnDelModule.Text = "Delete";
            this.btnDelModule.UseVisualStyleBackColor = true;
            this.btnDelModule.Click += new System.EventHandler(this.btnDelModule_Click);
            // 
            // btnAddModule
            // 
            this.btnAddModule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddModule.Location = new System.Drawing.Point(18, 496);
            this.btnAddModule.Name = "btnAddModule";
            this.btnAddModule.Size = new System.Drawing.Size(75, 23);
            this.btnAddModule.TabIndex = 12;
            this.btnAddModule.Text = "Add";
            this.btnAddModule.UseVisualStyleBackColor = true;
            this.btnAddModule.Click += new System.EventHandler(this.btnAddModule_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(432, 108);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Deployment definition";
            // 
            // txtModuleFile
            // 
            this.txtModuleFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtModuleFile.Location = new System.Drawing.Point(435, 124);
            this.txtModuleFile.Name = "txtModuleFile";
            this.txtModuleFile.Size = new System.Drawing.Size(210, 20);
            this.txtModuleFile.TabIndex = 3;
            this.txtModuleFile.TextChanged += new System.EventHandler(this.buildModuleInfo_Changed);
            // 
            // txtModuleName
            // 
            this.txtModuleName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtModuleName.Location = new System.Drawing.Point(435, 81);
            this.txtModuleName.Name = "txtModuleName";
            this.txtModuleName.Size = new System.Drawing.Size(268, 20);
            this.txtModuleName.TabIndex = 2;
            this.txtModuleName.TextChanged += new System.EventHandler(this.buildModuleInfo_Changed);
            // 
            // lbBuild
            // 
            this.lbBuild.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbBuild.FormattingEnabled = true;
            this.lbBuild.Location = new System.Drawing.Point(18, 38);
            this.lbBuild.Name = "lbBuild";
            this.lbBuild.Size = new System.Drawing.Size(407, 446);
            this.lbBuild.TabIndex = 0;
            this.lbBuild.SelectedValueChanged += new System.EventHandler(this.lbBuild_SelectedValueChanged);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "earth_connection.png");
            this.imageList1.Images.SetKeyName(1, "box.png");
            this.imageList1.Images.SetKeyName(2, "checkbox.png");
            this.imageList1.Images.SetKeyName(3, "box_new.png");
            this.imageList1.Images.SetKeyName(4, "box_edit.png");
            // 
            // timer
            // 
            this.timer.Interval = 300;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // ShuffleDeployer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this.tabControl1);
            this.Name = "ShuffleDeployer";
            this.Size = new System.Drawing.Size(737, 603);
            this.Load += new System.EventHandler(this.ShuffleDeployer_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ShuffleDeployer_KeyDown);
            this.tabControl1.ResumeLayout(false);
            this.tabPagePackage.ResumeLayout(false);
            this.tabPagePackage.PerformLayout();
            this.tabPageDeploy.ResumeLayout(false);
            this.tabPageDeploy.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabPageBuild.ResumeLayout(false);
            this.tabPageBuild.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPagePackage;
        private System.Windows.Forms.TabPage tabPageDeploy;
        private System.Windows.Forms.Label lblPackage;
        private System.Windows.Forms.Button btnPackage;
        private System.Windows.Forms.TabPage tabPageBuild;
        private System.Windows.Forms.ListBox lbBuild;
        private System.Windows.Forms.TextBox txtModuleName;
        private System.Windows.Forms.Button btnSavePackage;
        private System.Windows.Forms.Label lblBuildModules;
        private System.Windows.Forms.CheckBox chkModuleOptional;
        private System.Windows.Forms.Button btnDelModule;
        private System.Windows.Forms.Button btnAddModule;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtModuleFile;
        private System.Windows.Forms.Button btnDownModule;
        private System.Windows.Forms.Button btnUpModule;
        private System.Windows.Forms.Button btnStartDeploy;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ProgressBar pbModule;
        private System.Windows.Forms.ProgressBar pbRecord;
        private System.Windows.Forms.ProgressBar pbBlock;
        private System.Windows.Forms.ListBox lbLog;
        private System.Windows.Forms.CheckBox chkModuleDefault;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button btnCancelDeploy;
        private System.Windows.Forms.Label lblDeployFailed;
        private System.Windows.Forms.Label lblDeploySkipped;
        private System.Windows.Forms.Label lblDeployDeleted;
        private System.Windows.Forms.Label lblDeployUpdated;
        private System.Windows.Forms.Label lblDeployCreated;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnShowDefinition;
        private System.Windows.Forms.Button btnOpenDefinition;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label lblTimer;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblDeployModules;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnZipIt;
        private System.Windows.Forms.ListView lvModules;
        private System.Windows.Forms.ColumnHeader hdrName;
        private System.Windows.Forms.ColumnHeader hdrDescr;
        private System.Windows.Forms.Label lblModules;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtModuleDescr;
        private System.Windows.Forms.Button btnShowDataFile;
        private System.Windows.Forms.Button btnOpenDataFile;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtModuleDataFile;
        private System.Windows.Forms.Label lblCurrItem;
        private System.Windows.Forms.Label lblCurrBlock;
        private System.Windows.Forms.Label lblCurrModule;
        private System.Windows.Forms.Button btnOpenWF;
        private System.Windows.Forms.Button btnOpenLog;
        private System.Windows.Forms.Label lblDeploying;
        private System.Windows.Forms.ComboBox cmbModuleType;
        private System.Windows.Forms.Label label22;
    }
}