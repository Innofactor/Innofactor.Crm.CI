using Cinteros.Crm.Utils.Common;
using Cinteros.Crm.Utils.Shuffle;
using Innofactor.Crm.Shuffle.Builder.AppCode;
using Innofactor.Crm.Shuffle.Builder.Controls;
using Innofactor.Crm.Shuffle.Builder.Forms;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;
using Clipboard = Innofactor.Crm.Shuffle.Builder.AppCode.Clipboard;

namespace Innofactor.Crm.Shuffle.Builder
{
    public partial class ShuffleBuilder : PluginControlBase, IMessageBusHost
    {
        internal Clipboard clipboard = new Clipboard();
        private XmlDocument definitionDoc;
        private string fileName;
        private static string definitionTemplate = "<ShuffleDefinition><Blocks></Blocks></ShuffleDefinition>";
        private CintDynEntityCollection solutionsUnmanaged = null;
        private List<EntityMetadata> entities = null;
        private bool working = false;

        public List<EntityMetadata> Entities
        {
            get
            {
                return entities;
            }
        }

        public CintDynEntityCollection Solutions
        {
            get
            {
                return solutionsUnmanaged;
            }
        }

        public ShuffleBuilder()
        {
            InitializeComponent();
        }

        private void ShuffleBuilder_ConnectionUpdated(object sender, ConnectionUpdatedEventArgs e)
        {
            GetSolutions();
        }

        #region Event handlers

        private void toolStripButtonNew_Click(object sender, EventArgs e)
        {
            definitionDoc = new XmlDocument();
            definitionDoc.LoadXml(definitionTemplate);
            DisplayDefinition();
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

                definitionDoc = new XmlDocument();
                definitionDoc.Load(ofd.FileName);

                if (definitionDoc.DocumentElement.Name != "ShuffleDefinition" ||
                    definitionDoc.DocumentElement.ChildNodes.Count > 0 &&
                    definitionDoc.DocumentElement.ChildNodes[0].Name == "ShuffleDefinition")
                {
                    MessageBox.Show(this, "Invalid Xml: Definition XML root must be ShuffleDefinition!", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                    toolStripButtonOpen.Enabled = true;
                }
                else
                {
                    fileName = ofd.FileName;
                    DisplayDefinition();
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

                if (BuildAndValidateXml())
                {
                    definitionDoc.Save(fileName);
                    MessageBox.Show(this, "ShuffleDefinition saved!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (MessageBox.Show("Save anyway?", "Shuffle Builder", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    definitionDoc.Save(fileName);
                }
                EnableControls(true);
            }
        }

        private bool BuildAndValidateXml()
        {
            // Build the Xml from TreeView
            var doc = new XmlDocument();
            XmlNode rootNode = doc.CreateElement("root");
            doc.AppendChild(rootNode);

            AddXmlNode(tvDefinition.Nodes[0], rootNode);
            definitionDoc = new XmlDocument();
            definitionDoc.LoadXml(doc.SelectSingleNode("root/ShuffleDefinition").OuterXml);

            var result = false;
            try
            {
                ShuffleHelper.ValidateDefinitionXml(definitionDoc, null);
                result = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return result;
        }

        private void tsbItemSave_Click(object sender, EventArgs e)
        {
            ((IDefinitionSavable)panelContainer.Controls[0]).Save();

            var nodeAttributesCollection = (Dictionary<string, string>)tvDefinition.SelectedNode.Tag;

            if (nodeAttributesCollection.ContainsKey("Id"))
            {
                if (tvDefinition.SelectedNode.Text.Split(' ').Length == 1)
                    tvDefinition.SelectedNode.Text += " (" +
                                                   ((Dictionary<string, string>)tvDefinition.SelectedNode.Tag)["Id"] + ")";
                else
                    tvDefinition.SelectedNode.Text = tvDefinition.SelectedNode.Text.Split(' ')[0] + " (" +
                                                  ((Dictionary<string, string>)tvDefinition.SelectedNode.Tag)["Id"] + ")";

                tvDefinition.SelectedNode.Name = tvDefinition.SelectedNode.Text.Replace(" ", "");
            }

            if (nodeAttributesCollection.ContainsKey("LCID"))
            {
                tvDefinition.SelectedNode.Text = tvDefinition.SelectedNode.Text.Split(' ')[0] + " (" +
                                              ((Dictionary<string, string>)tvDefinition.SelectedNode.Tag)["LCID"] + ")";

                tvDefinition.SelectedNode.Name = tvDefinition.SelectedNode.Text.Replace(" ", "");
            }
        }

        private void tvDefinitionNodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode selectedNode = e.Node;
            selectedNode.TreeView.SelectedNode = selectedNode;
            var collec = (Dictionary<string, string>)selectedNode.Tag;

            TreeNodeHelper.AddContextMenu(e.Node, this);
            Control existingControl = panelContainer.Controls.Count > 0 ? panelContainer.Controls[0] : null;
            this.deleteToolStripMenuItem.Text = "Delete " + selectedNode.Text;

            UserControl ctrl = null;
            switch (selectedNode.Text.Split(' ')[0])
            {
                case "ShuffleDefinition":
                    ctrl = new ShuffleDefinitionControl(collec, this);
                    break;
                case "DataBlock":
                    ctrl = new DataBlockControl(collec, this);
                    break;
                case "SolutionBlock":
                    ctrl = new SolutionBlockControl(collec, this);
                    break;
                case "Export":
                    if (selectedNode.Parent != null && selectedNode.Parent.Text.StartsWith("DataBlock"))
                    {
                        ctrl = new DataBlockExportControl(collec, this);
                    }
                    else if (selectedNode.Parent != null && selectedNode.Parent.Text.StartsWith("SolutionBlock"))
                    {
                        ctrl = new SolutionBlockExportControl(collec, this);
                    }
                    break;
                case "Import":
                    if (selectedNode.Parent != null && selectedNode.Parent.Text.StartsWith("DataBlock"))
                    {
                        ctrl = new DataBlockImportControl(collec, this);
                    }
                    else if (selectedNode.Parent != null && selectedNode.Parent.Text.StartsWith("SolutionBlock"))
                    {
                        ctrl = new SolutionBlockImportControl(collec, this);
                    }
                    break;

                case "Relation":
                    ctrl = new RelationControl(collec, selectedNode, this);
                    break;

                case "Attribute":
                    if (selectedNode.Parent != null && selectedNode.Parent.Text.StartsWith("Attributes"))
                    {
                        ctrl = new ExportAttributeControl(collec, this);
                    }
                    else if (selectedNode.Parent != null && selectedNode.Parent.Text.StartsWith("Match"))
                    {
                        ctrl = new ImportAttributeControl(collec, this);
                    }
                    break;
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
                    if (selectedNode.Parent != null && selectedNode.Parent.Text.StartsWith("PreRequisites"))
                    {
                        ctrl = new PreReqSolutionControl(collec, this);
                    }
                    break;

                default:
                    {
                        panelContainer.Controls.Clear();
                        tsbItemSave.Visible = false;
                    }
                    break;
            }
            if (ctrl != null)
            {
                panelContainer.Controls.Add(ctrl);
                ctrl.BringToFront();
                if (existingControl != null) panelContainer.Controls.Remove(existingControl);
                tsbItemSave.Visible = true;
            }

            ManageMenuDisplay();
        }

        private void tvDefinition_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var e2 = new TreeNodeMouseClickEventArgs(e.Node, MouseButtons.Left, 1, 0, 0);

            tvDefinitionNodeMouseClick(tvDefinition, e2);
        }

        private void NodeMenuItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text.StartsWith("Delete"))
            {
                tvDefinition.SelectedNode.Remove();
            }
            else if (e.ClickedItem.Text == "Cut" || e.ClickedItem.Text == "Copy" || e.ClickedItem.Text == "Paste")
            {
                if (e.ClickedItem.Text == "Cut")
                    clipboard.Cut(tvDefinition.SelectedNode);
                else if (e.ClickedItem.Text == "Copy")
                    clipboard.Copy(tvDefinition.SelectedNode);
                else
                    clipboard.Paste(tvDefinition.SelectedNode);
            }
            else
            {
                string nodeText = e.ClickedItem.Name.Remove(0, 3);
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

        private void toolStripButtonMoveDown_Click(object sender, EventArgs e)
        {
            toolStripButtonMoveDown.Enabled = false;

            TreeNode tnmNode = tvDefinition.SelectedNode;
            TreeNode tnmNextNode = tnmNode.NextNode;

            if (tnmNextNode != null)
            {
                int idxBegin = tnmNode.Index;
                int idxEnd = tnmNextNode.Index;
                TreeNode tnmNodeParent = tnmNode.Parent;
                if (tnmNodeParent != null)
                {
                    tnmNode.Remove();
                    tnmNextNode.Remove();

                    tnmNodeParent.Nodes.Insert(idxBegin, tnmNextNode);
                    tnmNodeParent.Nodes.Insert(idxEnd, tnmNode);

                    tvDefinition.SelectedNode = tnmNode;
                }
            }

            toolStripButtonMoveDown.Enabled = true;
        }

        private void toolStripButtonMoveUp_Click(object sender, EventArgs e)
        {
            toolStripButtonMoveUp.Enabled = false;

            TreeNode tnmNode = tvDefinition.SelectedNode;
            TreeNode tnmPreviousNode = tnmNode.PrevNode;

            if (tnmPreviousNode != null)
            {
                int idxBegin = tnmNode.Index;
                int idxEnd = tnmPreviousNode.Index;
                TreeNode tnmNodeParent = tnmNode.Parent;
                if (tnmNodeParent != null)
                {
                    tnmNode.Remove();
                    tnmPreviousNode.Remove();

                    tnmNodeParent.Nodes.Insert(idxEnd, tnmNode);
                    tnmNodeParent.Nodes.Insert(idxBegin, tnmPreviousNode);

                    tvDefinition.SelectedNode = tnmNode;
                }
            }

            toolStripButtonMoveUp.Enabled = true;
        }

        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            tvDefinition.SelectedNode.Remove();
        }

        private void toolStripButtonDisplayXml_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = tvDefinition.SelectedNode;
            var collec = (Dictionary<string, string>)selectedNode.Tag;

            var doc = new XmlDocument();
            doc.AppendChild(doc.CreateElement(selectedNode.Text.Split(' ')[0]));

            foreach (string key in collec.Keys)
            {
                XmlAttribute attr = doc.CreateAttribute(key);
                attr.Value = collec[key];

                doc.DocumentElement.Attributes.Append(attr);
            }

            foreach (TreeNode node in selectedNode.Nodes)
            {
                AddXmlNode(node, doc.DocumentElement);
            }

            var xcdDialog = new XmlContentDisplayDialog(doc.OuterXml);
            xcdDialog.StartPosition = FormStartPosition.CenterParent;
            xcdDialog.ShowDialog();
        }

        #endregion

        /// <summary>
        ///     Loads the Definition from the extracted Xml solution files
        /// </summary>
        private void DisplayDefinition()
        {
            XmlNode definitionXmlNode = null;

            MethodInvoker miReadDefinition = delegate { definitionXmlNode = definitionDoc.DocumentElement; };

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

        internal void EnableControls(bool enabled)
        {
            MethodInvoker mi = delegate
            {
                toolStripButtonSave.Enabled = enabled;
                toolStripButtonOpen.Enabled = enabled;
                toolStripButtonRunit.Enabled = enabled && !string.IsNullOrEmpty(fileName);
                gbSiteMap.Enabled = enabled;
                gbProperties.Enabled = enabled;
                lblFilename.Text = string.IsNullOrEmpty(fileName) ? "<no file selected>" : fileName;
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
            tvDefinition.SelectedNode.Tag = e.AttributeCollection;
            TreeNodeHelper.SetNodeText(tvDefinition.SelectedNode);
        }

        /// <summary>
        ///     Manages which controls should be visible/enabled
        /// </summary>
        private void ManageMenuDisplay()
        {
            TreeNode selectedNode = tvDefinition.SelectedNode;

            tsbItemSave.Enabled = selectedNode != null;
            toolStripButtonDelete.Enabled = selectedNode != null && selectedNode.Text != "ShuffleDefinition";
            toolStripButtonMoveUp.Enabled = selectedNode != null && selectedNode.Parent != null &&
                                            selectedNode.Index != 0;
            toolStripButtonMoveDown.Enabled = selectedNode != null && selectedNode.Parent != null &&
                                              selectedNode.Index != selectedNode.Parent.Nodes.Count - 1;
            toolStripButtonDisplayXml.Enabled = selectedNode != null;

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

        private void GetSolutions()
        {
            if (Service == null)
            {
                return;
            }
            if (working)
            {
                return;
            }
            working = true;
            WorkAsync(new WorkAsyncInfo("Loading solutions",
                (eventargs) =>
                {
                    var svc = new CrmServiceProxy(Service);
                    var log = new PluginLogger("ShuffleBuilder", true, "");
                    try
                    {
                        solutionsUnmanaged = CintDynEntity.RetrieveMultiple(svc, "solution",
                            new string[] { "isvisible", "ismanaged" },
                            new object[] { true, false },
                            new ColumnSet("solutionid", "uniquename", "friendlyname", "version"), log);
                    }
                    finally
                    {
                        log.CloseLog();
                    }
                })
            {
                PostWorkCallBack =
                (completedargs) =>
                {
                    if (completedargs.Error != null)
                    {
                        MessageBox.Show(completedargs.Error.Message);
                    }
                    working = false;
                }
            });
        }

        public void OnIncomingMessage(MessageBusEventArgs message)
        {
            throw new NotImplementedException("Shuffle Builder cannot accept calls from other plugins in this version");
        }

        public event EventHandler<MessageBusEventArgs> OnOutgoingMessage;

        private void toolStripButtonRunit_Click(object sender, EventArgs e)
        {
            var args = new MessageBusEventArgs("Shuffle Runner")
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
    }
}
