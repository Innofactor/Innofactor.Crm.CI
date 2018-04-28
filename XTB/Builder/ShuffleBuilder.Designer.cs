namespace Innofactor.Crm.Shuffle.Builder
{
    partial class ShuffleBuilder
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
            this.components = new System.ComponentModel.Container();
            CSRichTextBoxSyntaxHighlighting.XMLViewerSettings xmlViewerSettings1 = new CSRichTextBoxSyntaxHighlighting.XMLViewerSettings();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShuffleBuilder));
            this.txtXML = new CSRichTextBoxSyntaxHighlighting.XMLViewer();
            this.treeviewMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addRuleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.tsbCloseThisTab = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonNew = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonOpen = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonValidate = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRunit = new System.Windows.Forms.ToolStripButton();
            this.splitContainerBuilder = new System.Windows.Forms.SplitContainer();
            this.tvDefinition = new System.Windows.Forms.TreeView();
            this.panProperties = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panelContainer = new System.Windows.Forms.Panel();
            this.panQuickActions = new System.Windows.Forms.Panel();
            this.gbQuickActions = new System.Windows.Forms.GroupBox();
            this.lblQAExpander = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.panTreeSplitter = new System.Windows.Forms.Panel();
            this.addMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.nothingToAddToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainerForm = new System.Windows.Forms.SplitContainer();
            this.nodeMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.commentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uncommentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeviewMenuStrip.SuspendLayout();
            this.toolStripMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerBuilder)).BeginInit();
            this.splitContainerBuilder.Panel1.SuspendLayout();
            this.splitContainerBuilder.Panel2.SuspendLayout();
            this.splitContainerBuilder.SuspendLayout();
            this.panProperties.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panQuickActions.SuspendLayout();
            this.gbQuickActions.SuspendLayout();
            this.addMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerForm)).BeginInit();
            this.splitContainerForm.Panel1.SuspendLayout();
            this.splitContainerForm.Panel2.SuspendLayout();
            this.splitContainerForm.SuspendLayout();
            this.nodeMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtXML
            // 
            this.txtXML.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtXML.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtXML.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtXML.Location = new System.Drawing.Point(0, 0);
            this.txtXML.Name = "txtXML";
            this.txtXML.ReadOnly = true;
            xmlViewerSettings1.AttributeKey = System.Drawing.Color.Red;
            xmlViewerSettings1.AttributeValue = System.Drawing.Color.Blue;
            xmlViewerSettings1.Comment = System.Drawing.Color.Green;
            xmlViewerSettings1.Element = System.Drawing.Color.DarkRed;
            xmlViewerSettings1.QuoteCharacter = '\"';
            xmlViewerSettings1.Tag = System.Drawing.Color.Blue;
            xmlViewerSettings1.Value = System.Drawing.Color.Black;
            this.txtXML.Settings = xmlViewerSettings1;
            this.txtXML.Size = new System.Drawing.Size(536, 637);
            this.txtXML.TabIndex = 0;
            this.txtXML.Text = "";
            // 
            // treeviewMenuStrip
            // 
            this.treeviewMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addRuleToolStripMenuItem});
            this.treeviewMenuStrip.Name = "treeviewMenuStrip";
            this.treeviewMenuStrip.Size = new System.Drawing.Size(123, 26);
            // 
            // addRuleToolStripMenuItem
            // 
            this.addRuleToolStripMenuItem.Name = "addRuleToolStripMenuItem";
            this.addRuleToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.addRuleToolStripMenuItem.Tag = "AddRule";
            this.addRuleToolStripMenuItem.Text = "Add Rule";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Icon.png");
            this.imageList1.Images.SetKeyName(1, "Cinteros 100 transp.png");
            // 
            // toolStripMain
            // 
            this.toolStripMain.AutoSize = false;
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbCloseThisTab,
            this.toolStripSeparator2,
            this.toolStripSeparator3,
            this.toolStripButtonNew,
            this.toolStripButtonOpen,
            this.toolStripSeparator4,
            this.toolStripButtonValidate,
            this.toolStripButtonSave,
            this.toolStripButtonRunit});
            this.toolStripMain.Location = new System.Drawing.Point(0, 0);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.Size = new System.Drawing.Size(914, 25);
            this.toolStripMain.TabIndex = 21;
            this.toolStripMain.Text = "toolStrip1";
            // 
            // tsbCloseThisTab
            // 
            this.tsbCloseThisTab.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCloseThisTab.Image = ((System.Drawing.Image)(resources.GetObject("tsbCloseThisTab.Image")));
            this.tsbCloseThisTab.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCloseThisTab.Name = "tsbCloseThisTab";
            this.tsbCloseThisTab.Size = new System.Drawing.Size(23, 22);
            this.tsbCloseThisTab.Text = "Close this tab";
            this.tsbCloseThisTab.Click += new System.EventHandler(this.tsbCloseThisTab_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonNew
            // 
            this.toolStripButtonNew.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonNew.Image")));
            this.toolStripButtonNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonNew.Name = "toolStripButtonNew";
            this.toolStripButtonNew.Size = new System.Drawing.Size(51, 22);
            this.toolStripButtonNew.Text = "New";
            this.toolStripButtonNew.ToolTipText = "New ShuffleDefinition";
            this.toolStripButtonNew.Click += new System.EventHandler(this.toolStripButtonNew_Click);
            // 
            // toolStripButtonOpen
            // 
            this.toolStripButtonOpen.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonOpen.Image")));
            this.toolStripButtonOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonOpen.Name = "toolStripButtonOpen";
            this.toolStripButtonOpen.Size = new System.Drawing.Size(56, 22);
            this.toolStripButtonOpen.Text = "Open";
            this.toolStripButtonOpen.ToolTipText = "Open ShuffleDefinition";
            this.toolStripButtonOpen.Click += new System.EventHandler(this.toolStripButtonOpen_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonValidate
            // 
            this.toolStripButtonValidate.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonValidate.Image")));
            this.toolStripButtonValidate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonValidate.Name = "toolStripButtonValidate";
            this.toolStripButtonValidate.Size = new System.Drawing.Size(68, 22);
            this.toolStripButtonValidate.Text = "Validate";
            this.toolStripButtonValidate.Click += new System.EventHandler(this.toolStripButtonValidate_Click);
            // 
            // toolStripButtonSave
            // 
            this.toolStripButtonSave.Enabled = false;
            this.toolStripButtonSave.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSave.Image")));
            this.toolStripButtonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSave.Name = "toolStripButtonSave";
            this.toolStripButtonSave.Size = new System.Drawing.Size(51, 22);
            this.toolStripButtonSave.Text = "Save";
            this.toolStripButtonSave.ToolTipText = "Save ShuffleDefinition";
            this.toolStripButtonSave.Click += new System.EventHandler(this.toolStripButtonSave_Click);
            // 
            // toolStripButtonRunit
            // 
            this.toolStripButtonRunit.Enabled = false;
            this.toolStripButtonRunit.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRunit.Image")));
            this.toolStripButtonRunit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRunit.Name = "toolStripButtonRunit";
            this.toolStripButtonRunit.Size = new System.Drawing.Size(105, 22);
            this.toolStripButtonRunit.Text = "Do the Shuffle!";
            this.toolStripButtonRunit.Click += new System.EventHandler(this.toolStripButtonRunit_Click);
            // 
            // splitContainerBuilder
            // 
            this.splitContainerBuilder.BackColor = System.Drawing.SystemColors.Window;
            this.splitContainerBuilder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerBuilder.Location = new System.Drawing.Point(0, 0);
            this.splitContainerBuilder.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.splitContainerBuilder.Name = "splitContainerBuilder";
            this.splitContainerBuilder.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerBuilder.Panel1
            // 
            this.splitContainerBuilder.Panel1.Controls.Add(this.tvDefinition);
            // 
            // splitContainerBuilder.Panel2
            // 
            this.splitContainerBuilder.Panel2.Controls.Add(this.panProperties);
            this.splitContainerBuilder.Panel2.Controls.Add(this.panQuickActions);
            this.splitContainerBuilder.Panel2.Controls.Add(this.panTreeSplitter);
            this.splitContainerBuilder.Size = new System.Drawing.Size(374, 637);
            this.splitContainerBuilder.SplitterDistance = 359;
            this.splitContainerBuilder.SplitterWidth = 6;
            this.splitContainerBuilder.TabIndex = 25;
            // 
            // tvDefinition
            // 
            this.tvDefinition.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvDefinition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvDefinition.HideSelection = false;
            this.tvDefinition.Location = new System.Drawing.Point(0, 0);
            this.tvDefinition.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tvDefinition.Name = "tvDefinition";
            this.tvDefinition.ShowNodeToolTips = true;
            this.tvDefinition.Size = new System.Drawing.Size(374, 359);
            this.tvDefinition.TabIndex = 0;
            this.tvDefinition.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvDefinition_AfterSelect);
            this.tvDefinition.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvDefinitionNodeMouseClick);
            this.tvDefinition.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tvDefinition_KeyDown);
            // 
            // panProperties
            // 
            this.panProperties.Controls.Add(this.groupBox1);
            this.panProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panProperties.Location = new System.Drawing.Point(0, 56);
            this.panProperties.Name = "panProperties";
            this.panProperties.Size = new System.Drawing.Size(374, 216);
            this.panProperties.TabIndex = 36;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.panelContainer);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(-1, 8);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox1.Size = new System.Drawing.Size(376, 210);
            this.groupBox1.TabIndex = 34;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Node Properties";
            // 
            // panelContainer
            // 
            this.panelContainer.BackColor = System.Drawing.SystemColors.Window;
            this.panelContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContainer.Location = new System.Drawing.Point(2, 16);
            this.panelContainer.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panelContainer.Name = "panelContainer";
            this.panelContainer.Size = new System.Drawing.Size(372, 191);
            this.panelContainer.TabIndex = 14;
            // 
            // panQuickActions
            // 
            this.panQuickActions.Controls.Add(this.gbQuickActions);
            this.panQuickActions.Dock = System.Windows.Forms.DockStyle.Top;
            this.panQuickActions.Location = new System.Drawing.Point(0, 1);
            this.panQuickActions.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panQuickActions.Name = "panQuickActions";
            this.panQuickActions.Padding = new System.Windows.Forms.Padding(0, 8, 0, 4);
            this.panQuickActions.Size = new System.Drawing.Size(374, 55);
            this.panQuickActions.TabIndex = 17;
            // 
            // gbQuickActions
            // 
            this.gbQuickActions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbQuickActions.Controls.Add(this.lblQAExpander);
            this.gbQuickActions.Controls.Add(this.linkLabel1);
            this.gbQuickActions.Location = new System.Drawing.Point(-1, 8);
            this.gbQuickActions.Name = "gbQuickActions";
            this.gbQuickActions.Padding = new System.Windows.Forms.Padding(8, 6, 3, 3);
            this.gbQuickActions.Size = new System.Drawing.Size(376, 49);
            this.gbQuickActions.TabIndex = 19;
            this.gbQuickActions.TabStop = false;
            this.gbQuickActions.Text = "Quick Actions";
            // 
            // lblQAExpander
            // 
            this.lblQAExpander.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblQAExpander.AutoSize = true;
            this.lblQAExpander.Location = new System.Drawing.Point(356, 0);
            this.lblQAExpander.Name = "lblQAExpander";
            this.lblQAExpander.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblQAExpander.Size = new System.Drawing.Size(14, 13);
            this.lblQAExpander.TabIndex = 9;
            this.lblQAExpander.Text = "–";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.linkLabel1.Location = new System.Drawing.Point(8, 19);
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(55, 13);
            this.linkLabel1.TabIndex = 0;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "linkLabel1";
            // 
            // panTreeSplitter
            // 
            this.panTreeSplitter.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.panTreeSplitter.Dock = System.Windows.Forms.DockStyle.Top;
            this.panTreeSplitter.Location = new System.Drawing.Point(0, 0);
            this.panTreeSplitter.Name = "panTreeSplitter";
            this.panTreeSplitter.Size = new System.Drawing.Size(374, 1);
            this.panTreeSplitter.TabIndex = 35;
            // 
            // addMenu
            // 
            this.addMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nothingToAddToolStripMenuItem});
            this.addMenu.Name = "addMenu";
            this.addMenu.Size = new System.Drawing.Size(154, 26);
            this.addMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.NodeMenuItemClicked);
            // 
            // nothingToAddToolStripMenuItem
            // 
            this.nothingToAddToolStripMenuItem.Name = "nothingToAddToolStripMenuItem";
            this.nothingToAddToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.nothingToAddToolStripMenuItem.Text = "nothing to add";
            // 
            // splitContainerForm
            // 
            this.splitContainerForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerForm.Location = new System.Drawing.Point(0, 25);
            this.splitContainerForm.Name = "splitContainerForm";
            // 
            // splitContainerForm.Panel1
            // 
            this.splitContainerForm.Panel1.Controls.Add(this.splitContainerBuilder);
            // 
            // splitContainerForm.Panel2
            // 
            this.splitContainerForm.Panel2.Controls.Add(this.txtXML);
            this.splitContainerForm.Size = new System.Drawing.Size(914, 637);
            this.splitContainerForm.SplitterDistance = 374;
            this.splitContainerForm.TabIndex = 26;
            // 
            // nodeMenu
            // 
            this.nodeMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator1,
            this.commentToolStripMenuItem,
            this.uncommentToolStripMenuItem,
            this.toolStripMenuItem2,
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem,
            this.toolStripMenuItem1,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem});
            this.nodeMenu.Name = "nodeMenu";
            this.nodeMenu.Size = new System.Drawing.Size(203, 242);
            this.nodeMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.NodeMenuItemClicked);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.DropDown = this.addMenu;
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.ShortcutKeyDisplayString = "Ins";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.addToolStripMenuItem.Tag = "Add";
            this.addToolStripMenuItem.Text = "Add";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(199, 6);
            // 
            // commentToolStripMenuItem
            // 
            this.commentToolStripMenuItem.Name = "commentToolStripMenuItem";
            this.commentToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+K";
            this.commentToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.commentToolStripMenuItem.Tag = "Comment";
            this.commentToolStripMenuItem.Text = "Comment";
            // 
            // uncommentToolStripMenuItem
            // 
            this.uncommentToolStripMenuItem.Name = "uncommentToolStripMenuItem";
            this.uncommentToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+U";
            this.uncommentToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.uncommentToolStripMenuItem.Tag = "Uncomment";
            this.uncommentToolStripMenuItem.Text = "Uncomment";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(199, 6);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            this.moveUpToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Up";
            this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.moveUpToolStripMenuItem.Text = "Move up";
            // 
            // moveDownToolStripMenuItem
            // 
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            this.moveDownToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Down";
            this.moveDownToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.moveDownToolStripMenuItem.Text = "Move down";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(199, 6);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("cutToolStripMenuItem.Image")));
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("copyToolStripMenuItem.Image")));
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("pasteToolStripMenuItem.Image")));
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("deleteToolStripMenuItem.Image")));
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.ShortcutKeyDisplayString = "Del";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.deleteToolStripMenuItem.Tag = "Delete";
            this.deleteToolStripMenuItem.Text = "Delete";
            // 
            // ShuffleBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerForm);
            this.Controls.Add(this.toolStripMain);
            this.Name = "ShuffleBuilder";
            this.Size = new System.Drawing.Size(914, 662);
            this.TabIcon = ((System.Drawing.Image)(resources.GetObject("$this.TabIcon")));
            this.ConnectionUpdated += new XrmToolBox.Extensibility.PluginControlBase.ConnectionUpdatedHandler(this.ShuffleBuilder_ConnectionUpdated);
            this.treeviewMenuStrip.ResumeLayout(false);
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.splitContainerBuilder.Panel1.ResumeLayout(false);
            this.splitContainerBuilder.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerBuilder)).EndInit();
            this.splitContainerBuilder.ResumeLayout(false);
            this.panProperties.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.panQuickActions.ResumeLayout(false);
            this.gbQuickActions.ResumeLayout(false);
            this.gbQuickActions.PerformLayout();
            this.addMenu.ResumeLayout(false);
            this.splitContainerForm.Panel1.ResumeLayout(false);
            this.splitContainerForm.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerForm)).EndInit();
            this.splitContainerForm.ResumeLayout(false);
            this.nodeMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip treeviewMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem addRuleToolStripMenuItem;
        private System.Windows.Forms.ImageList imageList1;
        internal System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripButton tsbCloseThisTab;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        internal System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        internal System.Windows.Forms.ToolStripButton toolStripButtonOpen;
        internal System.Windows.Forms.ToolStripButton toolStripButtonSave;
        internal System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton toolStripButtonNew;
        private System.Windows.Forms.ToolStripButton toolStripButtonValidate;
        private System.Windows.Forms.ToolStripButton toolStripButtonRunit;
        private System.Windows.Forms.SplitContainer splitContainerBuilder;
        internal System.Windows.Forms.TreeView tvDefinition;
        private System.Windows.Forms.Panel panProperties;
        internal System.Windows.Forms.GroupBox groupBox1;
        internal System.Windows.Forms.Panel panelContainer;
        private System.Windows.Forms.Panel panQuickActions;
        internal System.Windows.Forms.GroupBox gbQuickActions;
        internal System.Windows.Forms.Label lblQAExpander;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Panel panTreeSplitter;
        internal System.Windows.Forms.ContextMenuStrip addMenu;
        private System.Windows.Forms.ToolStripMenuItem nothingToAddToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainerForm;
        internal CSRichTextBoxSyntaxHighlighting.XMLViewer txtXML;
        internal System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        internal System.Windows.Forms.ContextMenuStrip nodeMenu;
        internal System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        internal System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem commentToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem uncommentToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem moveUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveDownToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        internal System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
    }
}
