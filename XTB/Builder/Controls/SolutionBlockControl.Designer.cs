namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    partial class SolutionBlockControl
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
            this.lblTimeout = new System.Windows.Forms.Label();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.lblEntity = new System.Windows.Forms.Label();
            this.lblType = new System.Windows.Forms.Label();
            this.txtFile = new System.Windows.Forms.TextBox();
            this.cmbName = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // lblTimeout
            // 
            this.lblTimeout.AutoSize = true;
            this.lblTimeout.Location = new System.Drawing.Point(4, 7);
            this.lblTimeout.Name = "lblTimeout";
            this.lblTimeout.Size = new System.Drawing.Size(35, 13);
            this.lblTimeout.TabIndex = 0;
            this.lblTimeout.Text = "Name";
            // 
            // txtPath
            // 
            this.txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPath.Location = new System.Drawing.Point(213, 30);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(234, 20);
            this.txtPath.TabIndex = 2;
            // 
            // lblEntity
            // 
            this.lblEntity.AutoSize = true;
            this.lblEntity.Location = new System.Drawing.Point(4, 33);
            this.lblEntity.Name = "lblEntity";
            this.lblEntity.Size = new System.Drawing.Size(29, 13);
            this.lblEntity.TabIndex = 2;
            this.lblEntity.Text = "Path";
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(4, 59);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(23, 13);
            this.lblType.TabIndex = 4;
            this.lblType.Text = "File";
            // 
            // txtFile
            // 
            this.txtFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFile.Location = new System.Drawing.Point(213, 56);
            this.txtFile.Name = "txtFile";
            this.txtFile.Size = new System.Drawing.Size(234, 20);
            this.txtFile.TabIndex = 3;
            // 
            // cmbName
            // 
            this.cmbName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbName.FormattingEnabled = true;
            this.cmbName.Location = new System.Drawing.Point(213, 3);
            this.cmbName.Name = "cmbName";
            this.cmbName.Size = new System.Drawing.Size(234, 21);
            this.cmbName.TabIndex = 1;
            // 
            // SolutionBlockControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmbName);
            this.Controls.Add(this.txtFile);
            this.Controls.Add(this.lblType);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.lblEntity);
            this.Controls.Add(this.lblTimeout);
            this.Name = "SolutionBlockControl";
            this.Size = new System.Drawing.Size(450, 150);
            this.Leave += new System.EventHandler(this.DataBlockControl_Leave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTimeout;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Label lblEntity;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.TextBox txtFile;
        private System.Windows.Forms.ComboBox cmbName;
    }
}
