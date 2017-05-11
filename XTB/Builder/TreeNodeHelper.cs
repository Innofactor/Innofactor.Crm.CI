// PROJECT : MsCrmTools.SiteMapEditor
// This project was developed by Tanguy Touzard
// CODEPLEX: http://xrmtoolbox.codeplex.com
// BLOG: http://mscrmtools.blogspot.com

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace Innofactor.Crm.Shuffle.Builder.AppCode
{
    /// <summary>
    /// Class that helps to manage TreeNode from SiteMap Treeview
    /// </summary>
    internal class TreeNodeHelper
    {
        #region Methods

        /// <summary>
        /// Adds a new TreeNode to the parent object from the XmlNode information
        /// </summary>
        /// <param name="parentObject">Object (TreeNode or TreeView) where to add a new TreeNode</param>
        /// <param name="xmlNode">Xml node from the sitemap</param>
        /// <param name="form">Current application form</param>
        /// <param name="isDisabled"> </param>
        public static void AddTreeViewNode(object parentObject, XmlNode xmlNode, ShuffleBuilder form, bool isDisabled = false)
        {
            TreeNode node = new TreeNode(xmlNode.Name);

            Dictionary<string, string> attributes = new Dictionary<string, string>();

            foreach (XmlAttribute attr in xmlNode.Attributes)
            {
                attributes.Add(attr.Name, attr.Value);
            }

            node.Tag = attributes;
            SetNodeText(node);
            node.Name = node.Text.Replace(" ", "");

            AddContextMenu(node, form);

            if (parentObject is TreeView)
            {
                ((TreeView)parentObject).Nodes.Add(node);
            }
            else if (parentObject is TreeNode)
            {
                ((TreeNode)parentObject).Nodes.Add(node);
            }
            else
            {
                throw new Exception("AddTreeViewNode: Unsupported control type");
            }

            foreach (XmlNode childNode in xmlNode.ChildNodes)
            {
                if (childNode.NodeType != XmlNodeType.Comment)
                {
                    AddTreeViewNode(node, childNode, form);
                }
                else
                {
                    //var commentDoc = new XmlDocument();
                    //commentDoc.LoadXml(childNode.InnerText);

                    //AddTreeViewNode(node, commentDoc.DocumentElement, form, true);
                }
            }
        }

        public static void SetNodeText(TreeNode node)
        {
            var text = node.Text.Split(' ')[0];
            Dictionary<string, string> attributes =
                node.Tag is Dictionary<string, string> ?
                    (Dictionary<string, string>)node.Tag :
                    new Dictionary<string, string>();
            if (text == "Filter")
            {
                text += " " +
                    (attributes.ContainsKey("Attribute") ? attributes["Attribute"] : "?") + " " +
                    (attributes.ContainsKey("Operator") ? attributes["Operator"] : "?") + " " +
                    (attributes.ContainsKey("Value") ? attributes["Value"] : "");
            }
            else if ((text == "Export" || text == "Import") &&
                node.Parent != null && node.Parent.Text.StartsWith("SolutionBlock"))
            {
                text += attributes.ContainsKey("Type") ? " " + attributes["Type"] : "";
            }
            else if (attributes.ContainsKey("Name"))
            {
                text += " " + attributes["Name"];
            }
            else if (attributes.ContainsKey("Block"))
            {
                text += " " + attributes["Block"];
            }
            else if (attributes.ContainsKey("Attribute"))
            {
                text += " " + attributes["Attribute"];
            }
            node.Text = text;
        }

        /// <summary>
        /// Adds a context menu to a TreeNode control
        /// </summary>
        /// <param name="node">TreeNode where to add the context menu</param>
        /// <param name="form">Current application form</param>
        public static void AddContextMenu(TreeNode node, ShuffleBuilder form)
        {
            var collec = (Dictionary<string, string>)node.Tag;

            HideAllContextMenuItems(form.nodeMenu);
            form.deleteToolStripMenuItem.Visible = true;

            var nodetype = node.Text.Split(' ')[0];
            var delete = false;
            var cutcopy = false;
            switch (nodetype)
            {
                case "Blocks":
                    form.addDataBlockToolStripMenuItem.Visible = true;
                    form.addSolutionBlockToolStripMenuItem.Visible = true;
                    break;

                case "DataBlock":
                case "SolutionBlock":
                    if (!node.Nodes.ContainsKey("Export"))
                    {
                        form.addExportToolStripMenuItem.Visible = true;
                    }
                    if (!node.Nodes.ContainsKey("Import"))
                    {
                        form.addImportToolStripMenuItem.Visible = true;
                    }
                    if (nodetype == "DataBlock")
                    {
                        form.addRelationToolStripMenuItem.Visible = true;
                    }
                    delete = true;
                    cutcopy = true;
                    break;

                case "Export":
                    if (node.Parent != null && node.Parent.Text.Split(' ')[0] == "DataBlock")
                    {
                        form.addFilterToolStripMenuItem.Visible = true;
                        form.addSortToolStripMenuItem.Visible = true;
                    }
                    else if (node.Parent != null && node.Parent.Text.Split(' ')[0] == "SolutionBlock" && !node.Nodes.ContainsKey("Settings"))
                    {
                        form.addSettingsToolStripMenuItem.Visible = true;
                    }
                    delete = true;
                    cutcopy = true;
                    break;

                case "Import":
                    if (node.Parent != null && node.Parent.Text.Split(' ')[0] == "DataBlock" && !node.Nodes.ContainsKey("Match"))
                    {
                        form.addMatchToolStripMenuItem.Visible = true;
                    }
                    else if (node.Parent != null && node.Parent.Text.Split(' ')[0] == "SolutionBlock" && !node.Nodes.ContainsKey("PreRequisites"))
                    {
                        form.addPreRequisitesToolStripMenuItem.Visible = true;
                    }
                    delete = true;
                    cutcopy = true;
                    break;

                case "Attributes":
                case "Match":
                    form.addAttributeToolStripMenuItem.Visible = true;
                    delete = nodetype == "Match";
                    cutcopy = true;
                    break;

                case "Attribute":
                    delete = node.Parent != null && node.Parent.Nodes.Count > 1;
                    cutcopy = true;
                    break;

                case "Filter":
                case "Sort":
                case "Relation":
                case "Settings":
                    delete = true;
                    cutcopy = true;
                    break;

                case "PreRequisites":
                    form.addSolutionToolStripMenuItem.Visible = true;
                    delete = true;
                    cutcopy = true;
                    break;

                case "Solution":
                    delete = node.Parent != null && node.Parent.Nodes.Count > 1;
                    cutcopy = true;
                    break;
            }
            form.deleteToolStripMenuItem.Visible = delete;
            form.cutToolStripMenuItem.Enabled = cutcopy;
            form.copyToolStripMenuItem.Enabled = cutcopy;
            form.pasteToolStripMenuItem.Enabled = form.clipboard.IsValidForPaste(node);

            node.ContextMenuStrip = form.nodeMenu;
        }

        /// <summary>
        /// Hides all items from a context menu
        /// </summary>
        /// <param name="cm">Context menu to clean</param>
        public static void HideAllContextMenuItems(ContextMenuStrip cm)
        {
            foreach (ToolStripItem o in cm.Items)
            {
                if (o.Text == "Cut" || o.Text == "Copy" || o.Text == "Paste")
                {
                    o.Enabled = false;
                }
                else if (o.Name == "toolStripSeparatorBeginOfEdition" || o is ToolStripSeparator)
                {
                    o.Visible = true;
                }
                else
                {
                    o.Visible = false;
                }
            }
        }

        #endregion Methods
    }
}
