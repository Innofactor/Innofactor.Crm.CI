namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    partial class FilterControl
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
            this.txtAttribute = new System.Windows.Forms.TextBox();
            this.lblEntity = new System.Windows.Forms.Label();
            this.lblType = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.lblIntersect = new System.Windows.Forms.Label();
            this.cmbOperator = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // lblTimeout
            // 
            this.lblTimeout.AutoSize = true;
            this.lblTimeout.Location = new System.Drawing.Point(4, 7);
            this.lblTimeout.Name = "lblTimeout";
            this.lblTimeout.Size = new System.Drawing.Size(46, 13);
            this.lblTimeout.TabIndex = 0;
            this.lblTimeout.Text = "Attribute";
            // 
            // txtAttribute
            // 
            this.txtAttribute.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAttribute.Location = new System.Drawing.Point(213, 4);
            this.txtAttribute.Name = "txtAttribute";
            this.txtAttribute.Size = new System.Drawing.Size(234, 20);
            this.txtAttribute.TabIndex = 1;
            // 
            // lblEntity
            // 
            this.lblEntity.AutoSize = true;
            this.lblEntity.Location = new System.Drawing.Point(4, 33);
            this.lblEntity.Name = "lblEntity";
            this.lblEntity.Size = new System.Drawing.Size(48, 13);
            this.lblEntity.TabIndex = 2;
            this.lblEntity.Text = "Operator";
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(4, 60);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(52, 13);
            this.lblType.TabIndex = 4;
            this.lblType.Text = "Field type";
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
            "string",
            "guid",
            "int",
            "bool",
            "datetime",
            "null",
            "not-null"});
            this.cmbType.Location = new System.Drawing.Point(213, 57);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(234, 21);
            this.cmbType.TabIndex = 3;
            // 
            // txtValue
            // 
            this.txtValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtValue.Location = new System.Drawing.Point(213, 84);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(234, 20);
            this.txtValue.TabIndex = 4;
            // 
            // lblIntersect
            // 
            this.lblIntersect.AutoSize = true;
            this.lblIntersect.Location = new System.Drawing.Point(4, 87);
            this.lblIntersect.Name = "lblIntersect";
            this.lblIntersect.Size = new System.Drawing.Size(34, 13);
            this.lblIntersect.TabIndex = 6;
            this.lblIntersect.Text = "Value";
            // 
            // cmbOperator
            // 
            this.cmbOperator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbOperator.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbOperator.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbOperator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOperator.FormattingEnabled = true;
            this.cmbOperator.Items.AddRange(new object[] {
            "Equal",
            "NotEqual",
            "GreaterThan",
            "LessThan",
            "GreaterEqual",
            "LessEqual",
            "Like",
            "NotLike",
            "In",
            "NotIn",
            "Between",
            "NotBetween",
            "Null",
            "NotNull",
            "Yesterday",
            "Today",
            "Tomorrow",
            "Last7Days",
            "Next7Days",
            "LastWeek",
            "ThisWeek",
            "NextWeek",
            "LastMonth",
            "ThisMonth",
            "NextMonth",
            "On",
            "OnOrBefore",
            "OnOrAfter",
            "LastYear",
            "ThisYear",
            "NextYear",
            "LastXHours",
            "NextXHours",
            "LastXDays",
            "NextXDays",
            "LastXWeeks",
            "NextXWeeks",
            "LastXMonths",
            "NextXMonths",
            "LastXYears",
            "NextXYears",
            "EqualUserId",
            "NotEqualUserId",
            "EqualBusinessId",
            "NotEqualBusinessId"});
            this.cmbOperator.Location = new System.Drawing.Point(213, 30);
            this.cmbOperator.Name = "cmbOperator";
            this.cmbOperator.Size = new System.Drawing.Size(234, 21);
            this.cmbOperator.TabIndex = 2;
            // 
            // FilterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmbOperator);
            this.Controls.Add(this.txtValue);
            this.Controls.Add(this.lblIntersect);
            this.Controls.Add(this.cmbType);
            this.Controls.Add(this.lblType);
            this.Controls.Add(this.lblEntity);
            this.Controls.Add(this.txtAttribute);
            this.Controls.Add(this.lblTimeout);
            this.Name = "FilterControl";
            this.Size = new System.Drawing.Size(450, 150);
            this.Leave += new System.EventHandler(this.FilterControl_Leave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTimeout;
        private System.Windows.Forms.TextBox txtAttribute;
        private System.Windows.Forms.Label lblEntity;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Label lblIntersect;
        private System.Windows.Forms.ComboBox cmbOperator;
    }
}
