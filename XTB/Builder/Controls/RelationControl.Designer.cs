namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    partial class RelationControl
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
            this.lblName = new System.Windows.Forms.Label();
            this.txtPKAttribute = new System.Windows.Forms.TextBox();
            this.lblIncludeNull = new System.Windows.Forms.Label();
            this.txtAttribute = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbBlock = new System.Windows.Forms.ComboBox();
            this.chkIncludeNull = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(4, 7);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(34, 13);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Block";
            // 
            // txtPKAttribute
            // 
            this.txtPKAttribute.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPKAttribute.Location = new System.Drawing.Point(213, 56);
            this.txtPKAttribute.Name = "txtPKAttribute";
            this.txtPKAttribute.Size = new System.Drawing.Size(234, 20);
            this.txtPKAttribute.TabIndex = 3;
            this.txtPKAttribute.Tag = "PK-Attribute|false";
            // 
            // lblIncludeNull
            // 
            this.lblIncludeNull.AutoSize = true;
            this.lblIncludeNull.Location = new System.Drawing.Point(4, 33);
            this.lblIncludeNull.Name = "lblIncludeNull";
            this.lblIncludeNull.Size = new System.Drawing.Size(87, 13);
            this.lblIncludeNull.TabIndex = 3;
            this.lblIncludeNull.Text = "Relation attribute";
            // 
            // txtAttribute
            // 
            this.txtAttribute.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAttribute.Location = new System.Drawing.Point(213, 30);
            this.txtAttribute.Name = "txtAttribute";
            this.txtAttribute.Size = new System.Drawing.Size(234, 20);
            this.txtAttribute.TabIndex = 2;
            this.txtAttribute.Tag = "Attribute|true";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Foreign attribute";
            // 
            // cmbBlock
            // 
            this.cmbBlock.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbBlock.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbBlock.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbBlock.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBlock.FormattingEnabled = true;
            this.cmbBlock.Items.AddRange(new object[] {
            "CreateUpdate",
            "CreateOnly",
            "UpdateOnly",
            "Never"});
            this.cmbBlock.Location = new System.Drawing.Point(213, 3);
            this.cmbBlock.Name = "cmbBlock";
            this.cmbBlock.Size = new System.Drawing.Size(234, 21);
            this.cmbBlock.TabIndex = 1;
            this.cmbBlock.Tag = "Block|true";
            // 
            // chkIncludeNull
            // 
            this.chkIncludeNull.AutoSize = true;
            this.chkIncludeNull.Location = new System.Drawing.Point(213, 85);
            this.chkIncludeNull.Name = "chkIncludeNull";
            this.chkIncludeNull.Size = new System.Drawing.Size(15, 14);
            this.chkIncludeNull.TabIndex = 10;
            this.chkIncludeNull.Tag = "IncludeNull";
            this.chkIncludeNull.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Include Null";
            // 
            // RelationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.chkIncludeNull);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbBlock);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtAttribute);
            this.Controls.Add(this.lblIncludeNull);
            this.Controls.Add(this.txtPKAttribute);
            this.Controls.Add(this.lblName);
            this.Name = "RelationControl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtPKAttribute;
        private System.Windows.Forms.Label lblIncludeNull;
        private System.Windows.Forms.TextBox txtAttribute;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbBlock;
        private System.Windows.Forms.CheckBox chkIncludeNull;
        private System.Windows.Forms.Label label3;
    }
}
