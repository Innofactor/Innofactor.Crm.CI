namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    partial class FetchControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FetchControl));
            CSRichTextBoxSyntaxHighlighting.XMLViewerSettings xmlViewerSettings1 = new CSRichTextBoxSyntaxHighlighting.XMLViewerSettings();
            this.label1 = new System.Windows.Forms.Label();
            this.btnFXB = new System.Windows.Forms.Button();
            this.txtFetchXML = new CSRichTextBoxSyntaxHighlighting.XMLViewer();
            this.btnFormat = new System.Windows.Forms.Button();
            this.gbFetchXML = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "FetchXML";
            // 
            // btnFXB
            // 
            this.btnFXB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFXB.Image = ((System.Drawing.Image)(resources.GetObject("btnFXB.Image")));
            this.btnFXB.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFXB.Location = new System.Drawing.Point(307, 7);
            this.btnFXB.Name = "btnFXB";
            this.btnFXB.Padding = new System.Windows.Forms.Padding(5);
            this.btnFXB.Size = new System.Drawing.Size(140, 32);
            this.btnFXB.TabIndex = 2;
            this.btnFXB.Text = "FetchXML Builder";
            this.btnFXB.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnFXB.UseVisualStyleBackColor = true;
            this.btnFXB.Click += new System.EventHandler(this.btnFXB_Click);
            // 
            // txtFetchXML
            // 
            this.txtFetchXML.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFetchXML.BackColor = System.Drawing.SystemColors.Window;
            this.txtFetchXML.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtFetchXML.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFetchXML.Location = new System.Drawing.Point(3, 58);
            this.txtFetchXML.Name = "txtFetchXML";
            xmlViewerSettings1.AttributeKey = System.Drawing.Color.Red;
            xmlViewerSettings1.AttributeValue = System.Drawing.Color.Blue;
            xmlViewerSettings1.Comment = System.Drawing.Color.Green;
            xmlViewerSettings1.Element = System.Drawing.Color.DarkRed;
            xmlViewerSettings1.QuoteCharacter = '\"';
            xmlViewerSettings1.Tag = System.Drawing.Color.Blue;
            xmlViewerSettings1.Value = System.Drawing.Color.Black;
            this.txtFetchXML.Settings = xmlViewerSettings1;
            this.txtFetchXML.Size = new System.Drawing.Size(444, 89);
            this.txtFetchXML.TabIndex = 3;
            this.txtFetchXML.Tag = "#text";
            this.txtFetchXML.Text = "";
            // 
            // btnFormat
            // 
            this.btnFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFormat.Location = new System.Drawing.Point(197, 7);
            this.btnFormat.Name = "btnFormat";
            this.btnFormat.Size = new System.Drawing.Size(104, 32);
            this.btnFormat.TabIndex = 4;
            this.btnFormat.Text = "Format XML";
            this.btnFormat.UseVisualStyleBackColor = true;
            this.btnFormat.Click += new System.EventHandler(this.btnFormat_Click);
            // 
            // gbFetchXML
            // 
            this.gbFetchXML.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbFetchXML.Location = new System.Drawing.Point(-1, 44);
            this.gbFetchXML.Name = "gbFetchXML";
            this.gbFetchXML.Size = new System.Drawing.Size(452, 8);
            this.gbFetchXML.TabIndex = 5;
            this.gbFetchXML.TabStop = false;
            // 
            // FetchControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtFetchXML);
            this.Controls.Add(this.gbFetchXML);
            this.Controls.Add(this.btnFormat);
            this.Controls.Add(this.btnFXB);
            this.Controls.Add(this.label1);
            this.Name = "FetchControl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnFXB;
        internal CSRichTextBoxSyntaxHighlighting.XMLViewer txtFetchXML;
        private System.Windows.Forms.Button btnFormat;
        private System.Windows.Forms.GroupBox gbFetchXML;
    }
}
