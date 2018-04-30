namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    partial class ExportAttributeControl
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
            this.txtName = new System.Windows.Forms.TextBox();
            this.chkIncludeNull = new System.Windows.Forms.CheckBox();
            this.lblIncludeNull = new System.Windows.Forms.Label();
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
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(213, 4);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(234, 20);
            this.txtName.TabIndex = 1;
            this.txtName.Tag = "Name|true";
            // 
            // chkIncludeNull
            // 
            this.chkIncludeNull.AutoSize = true;
            this.chkIncludeNull.Location = new System.Drawing.Point(213, 30);
            this.chkIncludeNull.Name = "chkIncludeNull";
            this.chkIncludeNull.Size = new System.Drawing.Size(15, 14);
            this.chkIncludeNull.TabIndex = 2;
            this.chkIncludeNull.Tag = "IncludeNull";
            this.chkIncludeNull.UseVisualStyleBackColor = true;
            // 
            // lblIncludeNull
            // 
            this.lblIncludeNull.AutoSize = true;
            this.lblIncludeNull.Location = new System.Drawing.Point(4, 30);
            this.lblIncludeNull.Name = "lblIncludeNull";
            this.lblIncludeNull.Size = new System.Drawing.Size(61, 13);
            this.lblIncludeNull.TabIndex = 3;
            this.lblIncludeNull.Text = "Include null";
            // 
            // ExportAttributeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblIncludeNull);
            this.Controls.Add(this.chkIncludeNull);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblName);
            this.Name = "ExportAttributeControl";
            this.Size = new System.Drawing.Size(450, 150);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.CheckBox chkIncludeNull;
        private System.Windows.Forms.Label lblIncludeNull;
    }
}
