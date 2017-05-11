namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    partial class PreReqSolutionControl
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
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblType = new System.Windows.Forms.Label();
            this.cmbComparer = new System.Windows.Forms.ComboBox();
            this.txtVersion = new System.Windows.Forms.TextBox();
            this.lblIntersect = new System.Windows.Forms.Label();
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
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(213, 4);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(234, 20);
            this.txtName.TabIndex = 1;
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(4, 33);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(52, 13);
            this.lblType.TabIndex = 4;
            this.lblType.Text = "Comparer";
            // 
            // cmbComparer
            // 
            this.cmbComparer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbComparer.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbComparer.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbComparer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbComparer.FormattingEnabled = true;
            this.cmbComparer.Items.AddRange(new object[] {
            "any",
            "eq-this",
            "ge-this",
            "eq",
            "ge"});
            this.cmbComparer.Location = new System.Drawing.Point(213, 30);
            this.cmbComparer.Name = "cmbComparer";
            this.cmbComparer.Size = new System.Drawing.Size(234, 21);
            this.cmbComparer.TabIndex = 5;
            this.cmbComparer.SelectedIndexChanged += new System.EventHandler(this.cmbType_SelectedIndexChanged);
            // 
            // txtVersion
            // 
            this.txtVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtVersion.Enabled = false;
            this.txtVersion.Location = new System.Drawing.Point(213, 57);
            this.txtVersion.Name = "txtVersion";
            this.txtVersion.Size = new System.Drawing.Size(234, 20);
            this.txtVersion.TabIndex = 7;
            // 
            // lblIntersect
            // 
            this.lblIntersect.AutoSize = true;
            this.lblIntersect.Location = new System.Drawing.Point(4, 60);
            this.lblIntersect.Name = "lblIntersect";
            this.lblIntersect.Size = new System.Drawing.Size(42, 13);
            this.lblIntersect.TabIndex = 6;
            this.lblIntersect.Text = "Version";
            // 
            // PreReqSolutionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtVersion);
            this.Controls.Add(this.lblIntersect);
            this.Controls.Add(this.cmbComparer);
            this.Controls.Add(this.lblType);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblTimeout);
            this.Name = "PreReqSolutionControl";
            this.Size = new System.Drawing.Size(450, 150);
            this.Leave += new System.EventHandler(this.DataBlockControl_Leave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTimeout;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.ComboBox cmbComparer;
        private System.Windows.Forms.TextBox txtVersion;
        private System.Windows.Forms.Label lblIntersect;
    }
}
