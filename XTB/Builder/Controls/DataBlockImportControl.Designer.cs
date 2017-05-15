namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    partial class DataBlockImportControl
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
            this.chkCreateWithId = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbSave = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbDelete = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.chkUpdateInactive = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtOverwrite = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.chkUpdateIdentical = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // chkCreateWithId
            // 
            this.chkCreateWithId.AutoSize = true;
            this.chkCreateWithId.Location = new System.Drawing.Point(213, 7);
            this.chkCreateWithId.Name = "chkCreateWithId";
            this.chkCreateWithId.Size = new System.Drawing.Size(15, 14);
            this.chkCreateWithId.TabIndex = 2;
            this.chkCreateWithId.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Create with Id";
            // 
            // cmbSave
            // 
            this.cmbSave.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSave.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbSave.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbSave.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSave.FormattingEnabled = true;
            this.cmbSave.Items.AddRange(new object[] {
            "CreateUpdate",
            "CreateOnly",
            "UpdateOnly",
            "Never"});
            this.cmbSave.Location = new System.Drawing.Point(213, 31);
            this.cmbSave.Name = "cmbSave";
            this.cmbSave.Size = new System.Drawing.Size(234, 21);
            this.cmbSave.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Save";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Delete";
            // 
            // cmbDelete
            // 
            this.cmbDelete.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbDelete.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbDelete.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbDelete.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDelete.FormattingEnabled = true;
            this.cmbDelete.Items.AddRange(new object[] {
            "None",
            "Existing",
            "All"});
            this.cmbDelete.Location = new System.Drawing.Point(213, 58);
            this.cmbDelete.Name = "cmbDelete";
            this.cmbDelete.Size = new System.Drawing.Size(234, 21);
            this.cmbDelete.TabIndex = 8;
            this.cmbDelete.SelectedIndexChanged += new System.EventHandler(this.cmbDelete_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 88);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Update inactive";
            // 
            // chkUpdateInactive
            // 
            this.chkUpdateInactive.AutoSize = true;
            this.chkUpdateInactive.Location = new System.Drawing.Point(213, 88);
            this.chkUpdateInactive.Name = "chkUpdateInactive";
            this.chkUpdateInactive.Size = new System.Drawing.Size(15, 14);
            this.chkUpdateInactive.TabIndex = 10;
            this.chkUpdateInactive.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 154);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Deprecated:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 171);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Overwrite";
            // 
            // txtOverwrite
            // 
            this.txtOverwrite.Location = new System.Drawing.Point(213, 168);
            this.txtOverwrite.Name = "txtOverwrite";
            this.txtOverwrite.Size = new System.Drawing.Size(234, 20);
            this.txtOverwrite.TabIndex = 20;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 115);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(84, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Update identical";
            // 
            // chkUpdateIdentical
            // 
            this.chkUpdateIdentical.AutoSize = true;
            this.chkUpdateIdentical.Location = new System.Drawing.Point(213, 115);
            this.chkUpdateIdentical.Name = "chkUpdateIdentical";
            this.chkUpdateIdentical.Size = new System.Drawing.Size(15, 14);
            this.chkUpdateIdentical.TabIndex = 15;
            this.chkUpdateIdentical.UseVisualStyleBackColor = true;
            // 
            // DataBlockImportControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label7);
            this.Controls.Add(this.chkUpdateIdentical);
            this.Controls.Add(this.txtOverwrite);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.chkUpdateInactive);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbDelete);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbSave);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkCreateWithId);
            this.Name = "DataBlockImportControl";
            this.Size = new System.Drawing.Size(450, 205);
            this.Leave += new System.EventHandler(this.DataBlockImportControl_Leave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkCreateWithId;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbSave;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbDelete;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkUpdateInactive;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtOverwrite;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox chkUpdateIdentical;
    }
}
