using Cinteros.Crm.Utils.Shuffle;
using Innofactor.Crm.Shuffle.Builder.AppCode;
using Innofactor.Crm.Shuffle.Builder.Controls;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Args;
using XrmToolBox.Extensibility.Interfaces;
using Clipboard = Innofactor.Crm.Shuffle.Builder.AppCode.Clipboard;

namespace Innofactor.Crm.Shuffle.Builder
{
    public partial class ShuffleBuilder : PluginControlBase, IMessageBusHost, IGitHubPlugin, IStatusBarMessenger
    {
        internal Clipboard clipboard = new Clipboard();
        private string fileName;
        private static string definitionTemplate = "<ShuffleDefinition><Blocks></Blocks></ShuffleDefinition>";

        private bool working = false;
        private bool buttonsEnabled = true;

        private List<EntityMetadata> entities = null;
        private Dictionary<string, List<AttributeMetadata>> attributes = null;
        private EntityCollection solutions = null;

        public event EventHandler<MessageBusEventArgs> OnOutgoingMessage;
        public event EventHandler<StatusBarMessageEventArgs> SendMessageToStatusBar;

        public List<EntityMetadata> Entities => entities;
        public Dictionary<string, List<AttributeMetadata>> Attributes => attributes;
        public EntityCollection Solutions => solutions;

        public string RepositoryName => "Innofactor.Crm.CI";

        public string UserName => "Innofactor";

        public ShuffleBuilder()
        {
            InitializeComponent();
        }

        private void ShuffleBuilder_ConnectionUpdated(object sender, ConnectionUpdatedEventArgs e)
        {
            solutions = null;
            entities = null;
        }

        #region Event handlers

        private void toolStripButtonNew_Click(object sender, EventArgs e)
        {
            var doc = new XmlDocument();
            doc.LoadXml(definitionTemplate);
            DisplayDefinition(doc);
        }

        private void toolStripButtonOpen_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Title = "Select an XML file representing a ShuffleDefinition",
                Filter = "XML file (*.xml)|*.xml"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                EnableControls(false);

                var doc = new XmlDocument();
                doc.Load(ofd.FileName);

                if (doc.DocumentElement.Name != "ShuffleDefinition" ||
                    doc.DocumentElement.ChildNodes.Count > 0 &&
                    doc.DocumentElement.ChildNodes[0].Name == "ShuffleDefinition")
                {
                    MessageBox.Show(this, "Invalid Xml: Definition XML root must be ShuffleDefinition!", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                    toolStripButtonOpen.Enabled = true;
                }
                else
                {
                    fileName = ofd.FileName;
                    DisplayDefinition(doc);
                    EnableControls(true);
                }
            }
        }

        private void toolStripButtonValidate_Click(object sender, EventArgs e)
        {
            if (BuildAndValidateXml())
            {
                MessageBox.Show("ShuffleDefinition validated ok!");
            }
        }

        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            if (!BuildAndValidateXml() &&
                MessageBox.Show("Save anyway?", "Shuffle Builder", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            var sfd = new SaveFileDialog
            {
                Title = "Select a location to save the SiteMap as a Xml file",
                Filter = "Xml file (*.xml)|*.xml",
                FileName = System.IO.Path.GetFileName(fileName)
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                fileName = sfd.FileName;
                EnableControls(false);
                var doc = GetDefinitionDocument();
                doc.Save(fileName);
                MessageBox.Show(this, "ShuffleDefinition saved!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                EnableControls(true);
            }
        }

        private void tvDefinitionNodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                HandleNodeSelection(e.Node);
            }
        }

        private void tvDefinition_AfterSelect(object sender, TreeViewEventArgs e)
        {
            HandleNodeSelection(e.Node);
        }

        private void NodeMenuItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            HandleNodeMenuClick(e.ClickedItem.Tag?.ToString() ?? e.ClickedItem.Text);
        }

        private void toolStripButtonRunit_Click(object sender, EventArgs e)
        {
            var args = new MessageBusEventArgs("Shuffle Runner", false)
            {
                TargetArgument = fileName
            };
            try
            {
                OnOutgoingMessage(this, args);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Calliung Shuffle Runner failed:\n" + ex.Message);
            }
        }

        internal void CallFXB(string text)
        {
            var args = new MessageBusEventArgs("FetchXML Builder", false)
            {
                TargetArgument = text
            };
            OnOutgoingMessage(this, args);
        }

        private void tvDefinition_KeyDown(object sender, KeyEventArgs e)
        {
            HandleTVKeyDown(e);
        }

        internal void QuickActionLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HandleNodeMenuClick((sender as LinkLabel)?.Tag?.ToString());
        }

        #endregion

        /// <summary>
        ///     Loads the Definition from the extracted Xml solution files
        /// </summary>
        private void DisplayDefinition(XmlDocument doc)
        {
            XmlNode definitionXmlNode = null;

            MethodInvoker miReadDefinition = delegate { definitionXmlNode = doc.DocumentElement; };

            if (InvokeRequired)
                Invoke(miReadDefinition);
            else
                miReadDefinition();

            MethodInvoker miFillTreeView = delegate
            {
                tvDefinition.Nodes.Clear();

                TreeNodeHelper.AddTreeViewNode(tvDefinition, definitionXmlNode, this);

                ManageMenuDisplay();
                tvDefinition.Nodes[0].Expand();
                if (tvDefinition.Nodes[0].Nodes.Count > 0)
                {
                    tvDefinition.Nodes[0].Nodes[0].Expand();
                }
                UpdateLiveXML();
            };

            if (tvDefinition.InvokeRequired)
            {
                tvDefinition.Invoke(miFillTreeView);
            }
            else
            {
                miFillTreeView();
            }
        }

        internal void EnableControls()
        {
            EnableControls(buttonsEnabled);
        }

        internal void EnableControls(bool enabled)
        {
            MethodInvoker mi = delegate
            {
                toolStripButtonSave.Enabled = enabled;
                toolStripButtonOpen.Enabled = enabled;
                toolStripButtonRunit.Enabled = enabled && !string.IsNullOrEmpty(fileName);
                tvDefinition.Enabled = enabled;
                SendMessageToStatusBar(this, new StatusBarMessageEventArgs(string.IsNullOrEmpty(fileName) ? "<no file selected>" : fileName));
                buttonsEnabled = enabled;
            };

            if (InvokeRequired)
            {
                Invoke(mi);
            }
            else
            {
                mi();
            }
        }

        private bool BuildAndValidateXml(bool validate = true)
        {
            var result = string.Empty;
            if (tvDefinition.Nodes.Count > 0 && validate)
            {
                // Build the Xml from TreeView
                var def = GetDefinitionDocument();
                try
                {
                    result = ShuffleHelper.ValidateDefinitionXml(def, null);
                }
                catch (Exception ex)
                {
                    if (string.IsNullOrEmpty(result))
                    {
                        result = ex.Message;
                    }
                    MessageBox.Show(ex.Message);
                }
            }
            return string.IsNullOrEmpty(result);
        }

        private XmlDocument GetDefinitionDocument()
        {
            var doc = new XmlDocument();
            if (tvDefinition.Nodes.Count > 0)
            {
                XmlNode rootNode = doc.CreateElement("root");
                doc.AppendChild(rootNode);
                TreeNodeHelper.AddXmlNode(tvDefinition.Nodes[0], rootNode);
                var xmlbody = doc.SelectSingleNode("root/ShuffleDefinition").OuterXml;
                doc.LoadXml(xmlbody);
            }
            return doc;
        }

        /// <summary>
        ///     Add the specified TreeNode properties in a XmlNode
        /// </summary>
        /// <param name="currentNode">TreeNode to add to the Xml</param>
        /// <param name="parentXmlNode">XmlNode where to add data</param>
        /// <param name="hasDisabledParent">Indicates if a parent node is already disabled</param>
        private void AddXmlNode(TreeNode currentNode, XmlNode parentXmlNode, bool hasDisabledParent = false)
        {
            XmlNode newNode = parentXmlNode.OwnerDocument.CreateElement(currentNode.Text.Split(' ')[0]);

            var collec = (Dictionary<string, string>)currentNode.Tag;

            foreach (string key in collec.Keys)
            {
                XmlAttribute attr = parentXmlNode.OwnerDocument.CreateAttribute(key);
                attr.Value = collec[key];
                newNode.Attributes.Append(attr);
            }

            var others = new List<TreeNode>();

            foreach (TreeNode childNode in currentNode.Nodes)
            {
                others.Add(childNode);
            }

            foreach (TreeNode otherNode in others)
            {
                AddXmlNode(otherNode, newNode);
            }

            parentXmlNode.AppendChild(newNode);
        }

        /// <summary>
        ///     When SiteMap component properties are saved, they are
        ///     copied in the current selected TreeNode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void CtrlSaved(object sender, SaveEventArgs e)
        {
            if (tvDefinition.SelectedNode == null)
            {
                return;
            }
            tvDefinition.SelectedNode.Tag = e.AttributeCollection;
            TreeNodeHelper.SetNodeText(tvDefinition.SelectedNode);
            UpdateLiveXML();
        }

        /// <summary>
        ///     Manages which controls should be visible/enabled
        /// </summary>
        private void ManageMenuDisplay()
        {
            TreeNode selectedNode = tvDefinition.SelectedNode;

            //toolStripButtonDelete.Enabled = selectedNode != null && selectedNode.Text != "ShuffleDefinition";
            //toolStripButtonMoveUp.Enabled = selectedNode != null && selectedNode.Parent != null &&
            //                                selectedNode.Index != 0;
            //toolStripButtonMoveDown.Enabled = selectedNode != null && selectedNode.Parent != null &&
            //                                  selectedNode.Index != selectedNode.Parent.Nodes.Count - 1;
            //toolStripButtonDisplayXml.Enabled = selectedNode != null;

            toolStripButtonSave.Enabled = tvDefinition.Nodes.Count > 0;
        }

        private static TreeNode AddChildNode(TreeNode parentNode, string name)
        {
            var childNode = new TreeNode(name);
            childNode.Tag = new Dictionary<string, string>();
            childNode.Name = childNode.Text.Replace(" ", "");
            var e3 = new TreeNodeMouseClickEventArgs(childNode, MouseButtons.Left, 1, 0, 0);
            parentNode.Nodes.Add(childNode);
            return childNode;
        }

        //private bool ValidateDefinitionXml(XmlDocument def)
        //{
        //    try
        //    {
        //        Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
        //        string assemblyname = assembly.ManifestModule.ToString();
        //        if (assemblyname.ToLower().EndsWith(".dll"))
        //        {
        //            assemblyname = assemblyname.Substring(0, assemblyname.Length - 4);
        //        }
        //        assemblyname = assemblyname.Replace("..", ".");
        //        Stream stream = assembly.GetManifestResourceStream(assemblyname + ".Resources.ShuffleDefinition.xsd");
        //        if (stream == null)
        //        {
        //            MessageBox.Show("Cannot find resource " + assemblyname + ".Resources.ShuffleDefinition.xsd");
        //            return false;
        //        }
        //        def.Schemas.Add(null, XmlReader.Create(stream));
        //        stream = assembly.GetManifestResourceStream(assemblyname + ".Resources.QueryExpression.xsd");
        //        if (stream == null)
        //        {
        //            MessageBox.Show("Cannot find resource " + assemblyname + ".Resources.QueryExpression.xsd");
        //            return false;
        //        }
        //        def.Schemas.Add(null, XmlReader.Create(stream));
        //        def.Validate(null);
        //        return true;
        //        //SendLine("Shuffle definition xml is valid");
        //    }
        //    catch (XmlSchemaValidationException ex)
        //    {
        //        var msg = "Shuffle definition xml validation error: \n" + ex.Message;
        //        if (ex.SourceObject != null && ex.SourceObject is XmlNode)
        //        {
        //            var node = (XmlNode)ex.SourceObject;
        //            var path = "";
        //            while (node != null)
        //            {
        //                path = node.Name + "/" + path;
        //                node = node.ParentNode;
        //            }
        //            msg += "\n\n" + path;
        //        }
        //        MessageBox.Show(msg);
        //        return false;
        //    }
        //}

        private void tsbCloseThisTab_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        internal void LoadSolutions(Action callback)
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading solutions",
                Work = (worker, args) =>
                {
                    var qry = new QueryByAttribute("solution");
                    qry.AddAttributeValue("isvisible", true);
                    qry.AddAttributeValue("ismanaged", false);
                    qry.ColumnSet = new ColumnSet("solutionid", "uniquename", "friendlyname", "version");
                    args.Result = Service.RetrieveMultiple(qry);
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.Message);
                    }
                    else if (args.Result is EntityCollection solutions)
                    {
                        this.solutions = solutions;
                        callback?.Invoke();
                    }
                }
            });
        }

        internal void LoadEntities(Action callback)
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading entities",
                Work = (worker, args) =>
                {
                    args.Result = MetadataHelper.LoadEntities(Service, ConnectionDetail.OrganizationMajorVersion);
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.Message);
                    }
                    else if (args.Result is RetrieveMetadataChangesResponse resp)
                    {
                        entities = resp.EntityMetadata.ToList();
                        callback?.Invoke();
                    }
                }
            });
        }

        internal void LoadAttributes(string entity, Action callback)
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = $"Loading attributes for {entity}",
                Work = (worker, args) =>
                {
                    args.Result = MetadataHelper.LoadEntityDetails(Service, entity);
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.Message);
                    }
                    else if (args.Result is RetrieveMetadataChangesResponse resp &&
                        resp.EntityMetadata.Count > 0 &&
                        resp.EntityMetadata[0] is EntityMetadata entitymeta)
                    {
                        if (attributes == null)
                        {
                            attributes = new Dictionary<string, List<AttributeMetadata>>();
                        }
                        if (attributes.ContainsKey(entity))
                        {
                            attributes.Remove(entity);
                        }
                        attributes.Add(entity, entitymeta.Attributes.ToList());
                        callback?.Invoke();
                    }
                }
            });
        }

        public void OnIncomingMessage(MessageBusEventArgs message)
        {
            if (message.SourcePlugin == "FetchXML Builder" &&
                message.TargetArgument is string fetchxml)
            {
                if (tvDefinition.SelectedNode?.Name == "FetchXML" &&
                    tvDefinition.SelectedNode?.Tag is Dictionary<string, string> collec)
                {
                    if (collec.ContainsKey("#text"))
                    {
                        collec["#text"] = fetchxml;
                    }
                    else
                    {
                        collec.Add("#text", fetchxml);
                    }
                    HandleNodeSelection(tvDefinition.SelectedNode);
                }
            }
        }

        private void HandleNodeSelection(TreeNode node)
        {
            if (!working)
            {
                if (tvDefinition.SelectedNode != node)
                {
                    tvDefinition.SelectedNode = node;
                    return;
                }

                UserControl ctrl = null;
                if (node != null)
                {
                    TreeNodeHelper.AddContextMenu(node, this);
                    this.deleteToolStripMenuItem.Text = "Delete " + node.Name;
                    var collec = (Dictionary<string, string>)node.Tag;

                    switch (node.Name)
                    {
                        case "ShuffleDefinition":
                            ctrl = new ShuffleDefinitionControl(collec, this);
                            break;
                        case "DataBlock":
                            ctrl = new DataBlockControl(collec, this, Entities);
                            break;
                        case "SolutionBlock":
                            ctrl = new SolutionBlockControl(collec, this);
                            break;
                        case "Export":
                            if (node.Parent != null && node.Parent.Text.StartsWith("DataBlock"))
                            {
                                ctrl = new DataBlockExportControl(collec, this);
                            }
                            else if (node.Parent != null && node.Parent.Text.StartsWith("SolutionBlock"))
                            {
                                ctrl = new SolutionBlockExportControl(collec, this);
                            }
                            break;
                        case "Import":
                            if (node.Parent != null && node.Parent.Text.StartsWith("DataBlock"))
                            {
                                ctrl = new DataBlockImportControl(collec, this);
                            }
                            else if (node.Parent != null && node.Parent.Text.StartsWith("SolutionBlock"))
                            {
                                ctrl = new SolutionBlockImportControl(collec, this);
                            }
                            break;

                        case "Relation":
                            ctrl = new RelationControl(collec, node, this);
                            break;

                        case "Attribute":
                            {
                                if (node.Parent?.Name == "Attributes" &&
                                    node.Parent?.Parent?.Parent?.Tag is Dictionary<string, string> entityprops &&
                                    entityprops.ContainsKey("Entity") &&
                                    entityprops["Entity"] is string entity &&
                                    !string.IsNullOrWhiteSpace(entity))
                                {
                                    ctrl = new ExportAttributeControl(collec, this, entity);
                                }
                                else if (node.Parent != null && node.Parent.Text.StartsWith("Match"))
                                {
                                    ctrl = new ImportAttributeControl(collec, this);
                                }
                                break;
                            }
                        case "FetchXML":
                            {
                                if (node.Parent.Parent.Tag is Dictionary<string, string> entityprops &&
                                    entityprops.ContainsKey("Entity") &&
                                    entityprops["Entity"] is string entity &&
                                    !string.IsNullOrWhiteSpace(entity))
                                {
                                    ctrl = new FetchControl(collec, this, entity);
                                }
                                break;
                            }
                        case "Filter":
                            ctrl = new FilterControl(collec, this);
                            break;

                        case "Sort":
                            ctrl = new SortControl(collec, this);
                            break;

                        case "Match":
                            ctrl = new ImportMatchControl(collec, this);
                            break;

                        case "Settings":
                            ctrl = new SettingsControl(collec, this);
                            break;

                        case "Solution":
                            if (node.Parent != null && node.Parent.Text.StartsWith("PreRequisites"))
                            {
                                ctrl = new PreReqSolutionControl(collec, this);
                            }
                            break;

                        default:
                            {
                                panelContainer.Controls.Clear();
                            }
                            break;
                    }
                }
                var existingControl = panelContainer.Controls.Count > 0 ? panelContainer.Controls[0] : null;
                if (ctrl != null)
                {
                    panelContainer.Controls.Add(ctrl);
                    ctrl.BringToFront();
                    ctrl.Dock = DockStyle.Fill;
                }
                if (existingControl != null) panelContainer.Controls.Remove(existingControl);
            }
            ManageMenuDisplay();
            ShowNodeXml(node);
        }

        private void HandleTVKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (deleteToolStripMenuItem.Enabled)
                {
                    if (MessageBox.Show(deleteToolStripMenuItem.Text + " ?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                    {
                        HandleNodeMenuClick(deleteToolStripMenuItem.Tag?.ToString());
                    }
                }
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Insert)
            {
                addMenu.Show(tvDefinition.PointToScreen(tvDefinition.Location));
            }
            else if (e.Control && e.KeyCode == Keys.K && commentToolStripMenuItem.Enabled)
            {
                HandleNodeMenuClick(commentToolStripMenuItem.Tag?.ToString());
            }
            else if (e.Control && e.KeyCode == Keys.U && uncommentToolStripMenuItem.Enabled)
            {
                HandleNodeMenuClick(uncommentToolStripMenuItem.Tag?.ToString());
            }
            else if (e.Control && e.KeyCode == Keys.Up && moveUpToolStripMenuItem.Enabled)
            {
                HandleNodeMenuClick(moveUpToolStripMenuItem.Tag?.ToString());
            }
            else if (e.Control && e.KeyCode == Keys.Down && moveDownToolStripMenuItem.Enabled)
            {
                HandleNodeMenuClick(moveDownToolStripMenuItem.Tag?.ToString());
            }
        }

        private void HandleNodeMenuClick(string ClickedTag)
        {
            if (ClickedTag == null || ClickedTag == "Add")
                return;
            TreeNode updateNode = null;
            if (ClickedTag == "Delete")
            {
                DeleteNode();
            }
            else if (ClickedTag == "Comment")
            {
                CommentNode();
            }
            else if (ClickedTag == "Uncomment")
            {
                UncommentNode();
            }
            else if (ClickedTag == "Cut" || ClickedTag == "Copy" || ClickedTag == "Paste")
            {
                if (ClickedTag == "Cut")
                    clipboard.Cut(tvDefinition.SelectedNode);
                else if (ClickedTag == "Copy")
                    clipboard.Copy(tvDefinition.SelectedNode);
                else
                    clipboard.Paste(tvDefinition.SelectedNode);
            }
            else
            {
                string nodeText = ClickedTag;
                updateNode = TreeNodeHelper.AddChildNode(tvDefinition.SelectedNode, nodeText);
                HandleNodeSelection(updateNode);
            }
            if (updateNode != null)
            {
                TreeNodeHelper.SetNodeTooltip(updateNode);
            }
            UpdateLiveXML();

            return;

            {
                string nodeText = ClickedTag;
                nodeText = nodeText.Substring(0, nodeText.IndexOf("ToolStripMenuItem"));

                var newNode = new TreeNode(nodeText);
                newNode.Tag = new Dictionary<string, string>();
                newNode.Name = newNode.Text.Replace(" ", "");
                var e2 = new TreeNodeMouseClickEventArgs(newNode, MouseButtons.Left, 1, 0, 0);

                if (newNode.Text == "Export" && tvDefinition.SelectedNode.Text.StartsWith("DataBlock"))
                {
                    var attributesNode = AddChildNode(newNode, "Attributes");
                    var firstAttributeNode = AddChildNode(attributesNode, "Attribute");
                }
                else if (newNode.Text == "Import" && tvDefinition.SelectedNode.Text.StartsWith("DataBlock"))
                {
                    var matchNode = AddChildNode(newNode, "Match");
                    var firstAttributeNode = AddChildNode(matchNode, "Attribute");
                }

                if (nodeText == "Filter")
                {
                    int i = 0;
                    while (i < tvDefinition.SelectedNode.Nodes.Count &&
                        tvDefinition.SelectedNode.Nodes[i].Text.StartsWith("Filter"))
                    {
                        i++;
                    }
                    tvDefinition.SelectedNode.Nodes.Insert(i, newNode);
                }
                else if (nodeText == "Sort")
                {
                    int i = 0;
                    while (i < tvDefinition.SelectedNode.Nodes.Count &&
                        (tvDefinition.SelectedNode.Nodes[i].Text.StartsWith("Filter") ||
                         tvDefinition.SelectedNode.Nodes[i].Text.StartsWith("Sort")))
                    {
                        i++;
                    }
                    tvDefinition.SelectedNode.Nodes.Insert(i, newNode);
                }
                else
                {
                    tvDefinition.SelectedNode.Nodes.Add(newNode);
                }
                tvDefinitionNodeMouseClick(tvDefinition, e2);
            }

        }

        internal void UpdateLiveXML()
        {
            var xml = string.Empty;
            string GetXml()
            {
                if (string.IsNullOrWhiteSpace(xml))
                {
                    xml = GetXmlString(true, false);
                }
                return xml;
            }
            txtXML.Text = GetXml();
            try
            {
                txtXML.Process();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "XML Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowNodeXml(TreeNode node)
        {
            txtPropertyXml.Text = TreeNodeHelper.GetNodeXml(node);
            try
            {
                txtPropertyXml.Process();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "XML Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetXmlString(bool format, bool validate)
        {
            var xml = string.Empty;
            if (BuildAndValidateXml(validate))
            {
                if (tvDefinition.Nodes.Count > 0)
                {
                    var doc = GetDefinitionDocument();
                    xml = doc.OuterXml;
                }
                if (format)
                {
                    XDocument doc = XDocument.Parse(xml);
                    xml = doc.ToString();
                }
            }
            return xml;
        }

        private void DeleteNode()
        {
            tvDefinition.SelectedNode.Remove();
        }

        private void CommentNode()
        {
            var node = tvDefinition.SelectedNode;
            if (node != null)
            {
                var doc = new XmlDocument();
                XmlNode rootNode = doc.CreateElement("root");
                doc.AppendChild(rootNode);
                TreeNodeHelper.AddXmlNode(node, rootNode);
                XDocument xdoc = XDocument.Parse(rootNode.InnerXml);
                var comment = xdoc.ToString();
                if (node.Nodes != null && node.Nodes.Count > 0)
                {
                    comment = "\r\n" + comment + "\r\n";
                }
                if (comment.Contains("--"))
                {
                    comment = comment.Replace("--", "~~");
                }
                if (comment.EndsWith("-"))
                {
                    comment = comment.Substring(0, comment.Length - 1) + "~";
                }
                var commentNode = doc.CreateComment(comment);
                var parent = node.Parent;
                var index = node.Index;
                node.Parent.Nodes.Remove(node);
                tvDefinition.SelectedNode = TreeNodeHelper.AddTreeViewNode(parent, commentNode, this, index);
            }
        }

        private void UncommentNode()
        {
            var node = tvDefinition.SelectedNode;
            if (node != null && node.Tag is Dictionary<string, string>)
            {
                var coll = node.Tag as Dictionary<string, string>;
                if (coll.ContainsKey("#comment"))
                {
                    var comment = coll["#comment"];
                    if (comment.Contains("~~"))
                    {
                        comment = comment.Replace("~~", "--");
                    }
                    if (comment.EndsWith("~"))
                    {
                        comment = comment.Substring(0, comment.Length - 1) + "-";
                    }
                    var doc = new XmlDocument();
                    try
                    {
                        doc.LoadXml(comment);
                        var parent = node.Parent;
                        var index = node.Index;
                        node.Parent.Nodes.Remove(node);
                        tvDefinition.SelectedNode = TreeNodeHelper.AddTreeViewNode(parent, doc.DocumentElement, this, index);
                        tvDefinition.SelectedNode.Expand();
                    }
                    catch (XmlException ex)
                    {
                        var msg = "Comment does contain well formatted xml.\nError description:\n\n" + ex.Message;
                        MessageBox.Show(msg, "Uncomment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }
    }
}
