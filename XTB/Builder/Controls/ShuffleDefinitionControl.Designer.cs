namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    partial class ShuffleDefinitionControl
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
            this.txtTimeout = new System.Windows.Forms.TextBox();
            this.chkStopOnError = new System.Windows.Forms.CheckBox();
            this.lblStopOnError = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblTimeout
            // 
            this.lblTimeout.AutoSize = true;
            this.lblTimeout.Location = new System.Drawing.Point(4, 7);
            this.lblTimeout.Name = "lblTimeout";
            this.lblTimeout.Size = new System.Drawing.Size(45, 13);
            this.lblTimeout.TabIndex = 0;
            this.lblTimeout.Text = "Timeout";
            // 
            // txtTimeout
            // 
            this.txtTimeout.Location = new System.Drawing.Point(213, 4);
            this.txtTimeout.Name = "txtTimeout";
            this.txtTimeout.Size = new System.Drawing.Size(100, 20);
            this.txtTimeout.TabIndex = 1;
            this.txtTimeout.Tag = "Timeout";
            // 
            // chkStopOnError
            // 
            this.chkStopOnError.AutoSize = true;
            this.chkStopOnError.Location = new System.Drawing.Point(213, 30);
            this.chkStopOnError.Name = "chkStopOnError";
            this.chkStopOnError.Size = new System.Drawing.Size(15, 14);
            this.chkStopOnError.TabIndex = 2;
            this.chkStopOnError.Tag = "StopOnError";
            this.chkStopOnError.UseVisualStyleBackColor = true;
            // 
            // lblStopOnError
            // 
            this.lblStopOnError.AutoSize = true;
            this.lblStopOnError.Location = new System.Drawing.Point(4, 30);
            this.lblStopOnError.Name = "lblStopOnError";
            this.lblStopOnError.Size = new System.Drawing.Size(68, 13);
            this.lblStopOnError.TabIndex = 3;
            this.lblStopOnError.Text = "Stop on error";
            // 
            // ShuffleDefinitionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblStopOnError);
            this.Controls.Add(this.chkStopOnError);
            this.Controls.Add(this.txtTimeout);
            this.Controls.Add(this.lblTimeout);
            this.Name = "ShuffleDefinitionControl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTimeout;
        private System.Windows.Forms.TextBox txtTimeout;
        private System.Windows.Forms.CheckBox chkStopOnError;
        private System.Windows.Forms.Label lblStopOnError;
    }
}
