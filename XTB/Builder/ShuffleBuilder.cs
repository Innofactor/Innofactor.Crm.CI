﻿using Cinteros.Crm.Utils.Shuffle;
using Innofactor.Crm.Shuffle.Builder.AppCode;
using Innofactor.Crm.Shuffle.Builder.Controls;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        private bool buttonsEnabled = true;

        public event EventHandler<MessageBusEventArgs> OnOutgoingMessage;
        public event EventHandler<StatusBarMessageEventArgs> SendMessageToStatusBar;

        public List<EntityMetadata> Entities { get; private set; } = null;

        public Dictionary<string, List<AttributeMetadata>> Attributes { get; private set; } = null;

        public EntityCollection Solutions { get; private set; } = null;

        public string RepositoryName => "Innofactor.Crm.CI";

        public string UserName => "Innofactor";

        public ShuffleBuilder()
        {
            InitializeComponent();
        }

        private void ShuffleBuilder_ConnectionUpdated(object sender, ConnectionUpdatedEventArgs e)
        {
            Solutions = null;
            Entities = null;
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
            CommitLastChange();
            if (BuildAndValidateXml())
            {
                MessageBox.Show("ShuffleDefinition validated ok!");
            }
        }

        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            CommitLastChange();
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
                using (var sw = new StreamWriter(fileName, false, new UTF8Encoding(true)))
                {
                    doc.Save(sw);
                }
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
            CommitLastChange();
            var xtbver = new Version(TopLevelControl.ProductVersion);
            if (xtbver > new Version("1.2018") &&
                xtbver < new Version("1.2018.5"))
            {   // Bug introduced with docking layout in XTB
                MessageBox.Show($"Unfortunately current version ({xtbver}) of XrmToolBox cannot connect properly to Shuffle Runner.\nPlease start Shuffle Runner from the Plugin List instead.", "Shuffle Runner", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
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
            CommitLastChange();
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

        private void tsbCloseThisTab_Click(object sender, EventArgs e)
        {
            CloseTool();
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
            {
                Invoke(miReadDefinition);
            }
            else
            {
                miReadDefinition();
            }

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
                if (tvDefinition.Nodes.Count > 0)
                {
                    tvDefinition.SelectedNode = tvDefinition.Nodes[0];
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

        private void CommitLastChange()
        {
            // This will trigger Leave event of any property panel being active, which till trigger it saving
            tvDefinition.Focus();
        }

        private bool BuildAndValidateXml(bool validate = true)
        {
            var result = true;
            if (tvDefinition.Nodes.Count > 0 && validate)
            {
                // Build the Xml from TreeView
                var def = GetDefinitionDocument();
                try
                {
                    ShuffleHelper.ValidateDefinitionXml(null, def);
                }
                catch (Exception ex)
                {
                    result = false;
                    MessageBox.Show(ex.Message);
                }
            }
            return result;
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

            foreach (var key in collec.Keys)
            {
                var attr = parentXmlNode.OwnerDocument.CreateAttribute(key);
                attr.Value = collec[key];
                newNode.Attributes.Append(attr);
            }

            var others = new List<TreeNode>();

            foreach (TreeNode childNode in currentNode.Nodes)
            {
                others.Add(childNode);
            }

            foreach (var otherNode in others)
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
            var selectedNode = tvDefinition.SelectedNode;

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
            var childNode = new TreeNode(name)
            {
                Tag = new Dictionary<string, string>()
            };
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

        internal void LoadSolutions(Action callback)
        {
            if (Service == null)
            {
                Solutions = new EntityCollection();
                callback?.Invoke();
                return;
            }
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
                        Solutions = solutions;
                        callback?.Invoke();
                    }
                }
            });
        }

        internal void LoadEntities(Action callback)
        {
            if (Service == null)
            {
                Entities = new List<EntityMetadata>();
                callback?.Invoke();
                return;
            }
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
                        Entities = resp.EntityMetadata.ToList();
                        callback?.Invoke();
                    }
                }
            });
        }

        internal void LoadAttributes(string entity, Action callback)
        {
            if (Attributes == null)
            {
                Attributes = new Dictionary<string, List<AttributeMetadata>>();
            }
            if (Service == null)
            {
                Attributes.Add(entity, new List<AttributeMetadata>());
                callback?.Invoke();
                return;
            }
            if (Attributes.ContainsKey(entity))
            {
                Attributes.Remove(entity);
            }
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
                        Attributes.Add(entity, entitymeta.Attributes.ToList());
                        callback?.Invoke();
                    }
                }
            });
        }

        public void OnIncomingMessage(MessageBusEventArgs message)
        {
            if (message.SourcePlugin != "FetchXML Builder" ||
                !(message.TargetArgument is string fetchxml))
            {
                MessageBox.Show($"Not sure what to do with a {message.TargetArgument.GetType()} message from {message.SourcePlugin}", "Incoming Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (tvDefinition.SelectedNode == null)
            {
                MessageBox.Show($"No node selected to handle value returned from {message.SourcePlugin}.\nMake sure a FetchXML node is selected when returning.", "Incoming Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (tvDefinition.SelectedNode.Name != "FetchXML" ||
                !(tvDefinition.SelectedNode.Tag is Dictionary<string, string> collec))
            {
                MessageBox.Show($"Current node '{tvDefinition.SelectedNode.Name}' cannot handle value returned from {message.SourcePlugin}.\nMake sure a FetchXML node is selected when returning.", "Incoming Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
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

        private void HandleNodeSelection(TreeNode node)
        {
            if (tvDefinition.SelectedNode != node)
            {
                tvDefinition.SelectedNode = node;
                return;
            }

            ControlBase ctrl = null;
            if (node != null)
            {
                TreeNodeHelper.AddContextMenu(node, this);
                deleteToolStripMenuItem.Text = "Delete " + node.Name;
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
                        if (node.Parent?.Name == "DataBlock")
                        {
                            ctrl = new DataBlockExportControl(collec, this);
                        }
                        else if (node.Parent?.Name == "SolutionBlock")
                        {
                            ctrl = new SolutionBlockExportControl(collec, this);
                        }
                        else
                        {
                            ctrl = new ErrorControl("Invalid parent", "Parent node is neither DataBlock nor SolutionBlock.");
                        }
                        break;
                    case "Import":
                        if (node.Parent?.Name == "DataBlock")
                        {
                            ctrl = new DataBlockImportControl(collec, this);
                        }
                        else if (node.Parent?.Name == "SolutionBlock")
                        {
                            ctrl = new SolutionBlockImportControl(collec, this);
                        }
                        else
                        {
                            ctrl = new ErrorControl("Invalid parent", "Parent node is neither DataBlock nor SolutionBlock.");
                        }
                        break;

                    case "Relation":
                        ctrl = new RelationControl(collec, node, this);
                        break;

                    case "Attribute":
                        {
                            if (node.Parent?.Name == "Attributes")
                            {
                                if (node.Parent?.Parent?.Parent?.Name == "DataBlock" &&
                                    node.Parent?.Parent?.Parent?.Tag is Dictionary<string, string> entityprops)
                                {
                                    if (entityprops.ContainsKey("Entity") &&
                                        entityprops["Entity"] is string entity &&
                                        !string.IsNullOrWhiteSpace(entity))
                                    {
                                        ctrl = new ExportAttributeControl(collec, this, entity);
                                    }
                                    else
                                    {
                                        ctrl = new ErrorControl("Invalid entity", "Entity not defined on parent block node.");
                                    }
                                }
                                else
                                {
                                    ctrl = new ErrorControl("Invalid block", "This is not a valid DataBlock.");
                                }
                            }
                            else if (node.Parent?.Name == "Match")
                            {
                                if (node.Parent?.Parent?.Parent?.Name == "DataBlock" &&
                                    node.Parent?.Parent?.Parent?.Tag is Dictionary<string, string> entityprops)
                                {
                                    if (entityprops.ContainsKey("Entity") &&
                                        entityprops["Entity"] is string entity &&
                                        !string.IsNullOrWhiteSpace(entity))
                                    {
                                        ctrl = new ImportAttributeControl(collec, this, entity);
                                    }
                                    else
                                    {
                                        ctrl = new ErrorControl("Invalid entity", "Entity not defined on parent block node.");
                                    }
                                }
                                else
                                {
                                    ctrl = new ErrorControl("Invalid block", "This is not a valid DataBlock.");
                                }
                            }
                            else
                            {
                                ctrl = new ErrorControl("Invalid parent", "Parent node is neither Attributes nor Match.");
                            }
                            break;
                        }
                    case "FetchXML":
                        {
                            if (node.Parent?.Parent?.Name == "DataBlock" &&
                                  node.Parent?.Parent?.Tag is Dictionary<string, string> entityprops)
                            {
                                if (entityprops.ContainsKey("Entity") &&
                                    entityprops["Entity"] is string entity &&
                                    !string.IsNullOrWhiteSpace(entity))
                                {
                                    ctrl = new FetchControl(collec, this, entity);
                                }
                                else
                                {
                                    ctrl = new ErrorControl("Invalid entity", "Entity not defined on parent block node.");
                                }
                            }
                            else
                            {
                                ctrl = new ErrorControl("Invalid block", "This is not a valid DataBlock.");
                            }
                            break;
                        }
                    case "Filter":
                        {
                            if (node.Parent?.Parent?.Name == "DataBlock" &&
                                node.Parent?.Parent?.Tag is Dictionary<string, string> entityprops)
                            {
                                if (entityprops.ContainsKey("Entity") &&
                                    entityprops["Entity"] is string entity &&
                                    !string.IsNullOrWhiteSpace(entity))
                                {
                                    ctrl = new FilterControl(collec, this, entity);
                                }
                                else
                                {
                                    ctrl = new ErrorControl("Invalid entity", "Entity not defined on parent block node.");
                                }
                            }
                            else
                            {
                                ctrl = new ErrorControl("Invalid block", "This is not a valid DataBlock.");
                            }
                            break;
                        }
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
                        if (node.Parent?.Name == "PreRequisites")
                        {
                            ctrl = new PreReqSolutionControl(collec, this);
                        }
                        else
                        {
                            ctrl = new ErrorControl("Invalid node", "This is only valid under PreRequisites.");
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
            if (existingControl != null)
            {
                panelContainer.Controls.Remove(existingControl);
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
            var node = tvDefinition.SelectedNode;
            if (ClickedTag == null || ClickedTag == "Add" || node == null)
            {
                return;
            }
            TreeNode updateNode = null;
            if (ClickedTag == "Delete")
            {
                DeleteNode(node);
            }
            else if (ClickedTag == "Comment")
            {
                CommentNode(node);
            }
            else if (ClickedTag == "Uncomment")
            {
                UncommentNode(node);
            }
            else if (ClickedTag == "MoveDown")
            {
                MoveNodeDown(node);
            }
            else if (ClickedTag == "MoveUp")
            {
                MoveNodeUp(node);
            }
            else if (ClickedTag == "Cut" || ClickedTag == "Copy" || ClickedTag == "Paste")
            {
                if (ClickedTag == "Cut")
                {
                    clipboard.Cut(tvDefinition.SelectedNode);
                }
                else if (ClickedTag == "Copy")
                {
                    clipboard.Copy(tvDefinition.SelectedNode);
                }
                else
                {
                    clipboard.Paste(tvDefinition.SelectedNode);
                }
            }
            else
            {
                var nodeText = ClickedTag;
                updateNode = TreeNodeHelper.AddChildNode(tvDefinition.SelectedNode, nodeText);
                HandleNodeSelection(updateNode);
            }
            if (updateNode != null)
            {
                TreeNodeHelper.SetNodeTooltip(updateNode);
            }
            UpdateLiveXML();
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
                    var doc = XDocument.Parse(xml);
                    xml = doc.ToString();
                }
            }
            return xml;
        }

        private void MoveNodeDown(TreeNode node)
        {
            var nextnode = node.NextNode;
            if (nextnode != null)
            {
                var idxBegin = node.Index;
                var idxEnd = nextnode.Index;
                var tnmNodeParent = node.Parent;
                if (tnmNodeParent != null)
                {
                    node.Remove();
                    nextnode.Remove();
                    tnmNodeParent.Nodes.Insert(idxBegin, nextnode);
                    tnmNodeParent.Nodes.Insert(idxEnd, node);
                    tvDefinition.SelectedNode = node;
                    UpdateLiveXML();
                }
            }
        }

        private void MoveNodeUp(TreeNode node)
        {
            var prevnode = node.PrevNode;
            if (prevnode != null)
            {
                var idxBegin = node.Index;
                var idxEnd = prevnode.Index;
                var tnmNodeParent = node.Parent;
                if (tnmNodeParent != null)
                {
                    node.Remove();
                    prevnode.Remove();
                    tnmNodeParent.Nodes.Insert(idxEnd, node);
                    tnmNodeParent.Nodes.Insert(idxBegin, prevnode);
                    tvDefinition.SelectedNode = node;
                    UpdateLiveXML();
                }
            }
        }

        private void DeleteNode(TreeNode node)
        {
            node.Remove();
        }

        private void CommentNode(TreeNode node)
        {
            var doc = new XmlDocument();
            XmlNode rootNode = doc.CreateElement("root");
            doc.AppendChild(rootNode);
            TreeNodeHelper.AddXmlNode(node, rootNode);
            var xdoc = XDocument.Parse(rootNode.InnerXml);
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

        private void UncommentNode(TreeNode node)
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
