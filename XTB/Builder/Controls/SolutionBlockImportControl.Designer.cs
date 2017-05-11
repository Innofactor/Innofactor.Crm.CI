namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    partial class SolutionBlockImportControl
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
            this.chkOverSame = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chkOverNewer = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chkActCode = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.chkOverCust = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.chkPublish = new System.Windows.Forms.CheckBox();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // chkOverSame
            // 
            this.chkOverSame.AutoSize = true;
            this.chkOverSame.Location = new System.Drawing.Point(213, 7);
            this.chkOverSame.Name = "chkOverSame";
            this.chkOverSame.Size = new System.Drawing.Size(15, 14);
            this.chkOverSame.TabIndex = 2;
            this.chkOverSame.Tag = "OverwriteSameVersion";
            this.chkOverSame.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Overwrite Same Version";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(124, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Overwrite Newer Version";
            // 
            // chkOverNewer
            // 
            this.chkOverNewer.AutoSize = true;
            this.chkOverNewer.Location = new System.Drawing.Point(213, 27);
            this.chkOverNewer.Name = "chkOverNewer";
            this.chkOverNewer.Size = new System.Drawing.Size(15, 14);
            this.chkOverNewer.TabIndex = 4;
            this.chkOverNewer.Tag = "OverwriteNewerVersion";
            this.chkOverNewer.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(132, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Activate Server Side Code";
            // 
            // chkActCode
            // 
            this.chkActCode.AutoSize = true;
            this.chkActCode.Location = new System.Drawing.Point(213, 75);
            this.chkActCode.Name = "chkActCode";
            this.chkActCode.Size = new System.Drawing.Size(15, 14);
            this.chkActCode.TabIndex = 6;
            this.chkActCode.Tag = "ActivateServersideCode";
            this.chkActCode.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 95);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(125, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Overwrite Customizations";
            // 
            // chkOverCust
            // 
            this.chkOverCust.AutoSize = true;
            this.chkOverCust.Location = new System.Drawing.Point(213, 95);
            this.chkOverCust.Name = "chkOverCust";
            this.chkOverCust.Size = new System.Drawing.Size(15, 14);
            this.chkOverCust.TabIndex = 8;
            this.chkOverCust.Tag = "OverwriteCustomizations";
            this.chkOverCust.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 115);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(112, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Publish All After Import";
            // 
            // chkPublish
            // 
            this.chkPublish.AutoSize = true;
            this.chkPublish.Location = new System.Drawing.Point(213, 115);
            this.chkPublish.Name = "chkPublish";
            this.chkPublish.Size = new System.Drawing.Size(15, 14);
            this.chkPublish.TabIndex = 10;
            this.chkPublish.Tag = "PublishAll";
            this.chkPublish.UseVisualStyleBackColor = true;
            // 
            // cmbType
            // 
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Items.AddRange(new object[] {
            "Managed",
            "Unmanaged",
            "Both",
            "None"});
            this.cmbType.Location = new System.Drawing.Point(213, 48);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(234, 21);
            this.cmbType.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 51);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Solution Import Type";
            // 
            // SolutionBlockImportControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cmbType);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.chkPublish);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.chkOverCust);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.chkActCode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chkOverNewer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkOverSame);
            this.Name = "SolutionBlockImportControl";
            this.Size = new System.Drawing.Size(450, 306);
            this.Leave += new System.EventHandler(this.DataBlockExportControl_Leave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkOverSame;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkOverNewer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkActCode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkOverCust;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkPublish;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.Label label6;
    }
}
