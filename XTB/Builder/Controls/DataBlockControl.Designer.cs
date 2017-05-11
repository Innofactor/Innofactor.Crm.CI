namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    partial class DataBlockControl
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
            this.txtEntity = new System.Windows.Forms.TextBox();
            this.lblEntity = new System.Windows.Forms.Label();
            this.lblType = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.txtIntersect = new System.Windows.Forms.TextBox();
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
            // txtEntity
            // 
            this.txtEntity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEntity.Location = new System.Drawing.Point(213, 30);
            this.txtEntity.Name = "txtEntity";
            this.txtEntity.Size = new System.Drawing.Size(234, 20);
            this.txtEntity.TabIndex = 3;
            // 
            // lblEntity
            // 
            this.lblEntity.AutoSize = true;
            this.lblEntity.Location = new System.Drawing.Point(4, 33);
            this.lblEntity.Name = "lblEntity";
            this.lblEntity.Size = new System.Drawing.Size(33, 13);
            this.lblEntity.TabIndex = 2;
            this.lblEntity.Text = "Entity";
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(4, 59);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(31, 13);
            this.lblType.TabIndex = 4;
            this.lblType.Text = "Type";
            // 
            // cmbType
            // 
            this.cmbType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Items.AddRange(new object[] {
            "Entity",
            "Intersect"});
            this.cmbType.Location = new System.Drawing.Point(213, 56);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(234, 21);
            this.cmbType.TabIndex = 5;
            this.cmbType.SelectedIndexChanged += new System.EventHandler(this.cmbType_SelectedIndexChanged);
            // 
            // txtIntersect
            // 
            this.txtIntersect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtIntersect.Enabled = false;
            this.txtIntersect.Location = new System.Drawing.Point(213, 83);
            this.txtIntersect.Name = "txtIntersect";
            this.txtIntersect.Size = new System.Drawing.Size(234, 20);
            this.txtIntersect.TabIndex = 7;
            // 
            // lblIntersect
            // 
            this.lblIntersect.AutoSize = true;
            this.lblIntersect.Location = new System.Drawing.Point(4, 86);
            this.lblIntersect.Name = "lblIntersect";
            this.lblIntersect.Size = new System.Drawing.Size(77, 13);
            this.lblIntersect.TabIndex = 6;
            this.lblIntersect.Text = "Intersect name";
            // 
            // DataBlockControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtIntersect);
            this.Controls.Add(this.lblIntersect);
            this.Controls.Add(this.cmbType);
            this.Controls.Add(this.lblType);
            this.Controls.Add(this.txtEntity);
            this.Controls.Add(this.lblEntity);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblTimeout);
            this.Name = "DataBlockControl";
            this.Size = new System.Drawing.Size(450, 150);
            this.Leave += new System.EventHandler(this.DataBlockControl_Leave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTimeout;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtEntity;
        private System.Windows.Forms.Label lblEntity;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.TextBox txtIntersect;
        private System.Windows.Forms.Label lblIntersect;
    }
}
