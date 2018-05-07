namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    partial class ImportAttributeControl
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
            this.lblIncludeNull = new System.Windows.Forms.Label();
            this.txtDisplay = new System.Windows.Forms.TextBox();
            this.cmbAttribute = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(4, 7);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(35, 13);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name";
            // 
            // lblIncludeNull
            // 
            this.lblIncludeNull.AutoSize = true;
            this.lblIncludeNull.Location = new System.Drawing.Point(4, 30);
            this.lblIncludeNull.Name = "lblIncludeNull";
            this.lblIncludeNull.Size = new System.Drawing.Size(82, 13);
            this.lblIncludeNull.TabIndex = 3;
            this.lblIncludeNull.Text = "Display attribute";
            // 
            // txtDisplay
            // 
            this.txtDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDisplay.Location = new System.Drawing.Point(213, 30);
            this.txtDisplay.Name = "txtDisplay";
            this.txtDisplay.Size = new System.Drawing.Size(234, 20);
            this.txtDisplay.TabIndex = 4;
            this.txtDisplay.Tag = "Display";
            // 
            // cmbAttribute
            // 
            this.cmbAttribute.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbAttribute.FormattingEnabled = true;
            this.cmbAttribute.Location = new System.Drawing.Point(213, 4);
            this.cmbAttribute.Name = "cmbAttribute";
            this.cmbAttribute.Size = new System.Drawing.Size(234, 21);
            this.cmbAttribute.TabIndex = 1;
            this.cmbAttribute.Tag = "Name|true";
            // 
            // ImportAttributeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.cmbAttribute);
            this.Controls.Add(this.txtDisplay);
            this.Controls.Add(this.lblIncludeNull);
            this.Controls.Add(this.lblName);
            this.Name = "ImportAttributeControl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblIncludeNull;
        private System.Windows.Forms.TextBox txtDisplay;
        private System.Windows.Forms.ComboBox cmbAttribute;
    }
}
