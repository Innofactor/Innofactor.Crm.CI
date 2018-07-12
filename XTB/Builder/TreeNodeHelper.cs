// PROJECT : MsCrmTools.SiteMapEditor
// This project was developed by Tanguy Touzard
// CODEPLEX: http://xrmtoolbox.codeplex.com
// BLOG: http://mscrmtools.blogspot.com

using Innofactor.Crm.Shuffle.Builder.Controls;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

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
        public static TreeNode AddTreeViewNode(object parentObject, XmlNode xmlNode, ShuffleBuilder form, int index = -1)
        {
            TreeNode node = null;
            if (xmlNode is XmlElement || xmlNode is XmlComment)
            {
                node = new TreeNode(xmlNode.Name)
                {
                    Name = xmlNode.Name
                };
                var attributes = new Dictionary<string, string>();

                if (xmlNode.NodeType == XmlNodeType.Comment)
                {
                    attributes.Add("#comment", xmlNode.Value);
                    node.ForeColor = System.Drawing.Color.Gray;
                }
                else if (xmlNode.Attributes != null)
                {
                    foreach (XmlAttribute attr in xmlNode.Attributes)
                    {
                        attributes.Add(attr.Name, attr.Value);
                    }
                }
                if (parentObject is TreeView)
                {
                    ((TreeView)parentObject).Nodes.Add(node);
                }
                else if (parentObject is TreeNode)
                {
                    if (index == -1)
                    {
                        ((TreeNode)parentObject).Nodes.Add(node);
                    }
                    else
                    {
                        ((TreeNode)parentObject).Nodes.Insert(index, node);
                    }
                }
                else
                {
                    throw new Exception("AddTreeViewNode: Unsupported control type");
                }
                node.Tag = attributes;
                AddContextMenu(node, form);
                foreach (XmlNode childNode in xmlNode.ChildNodes)
                {
                    AddTreeViewNode(node, childNode, form);
                }
                SetNodeText(node);
            }
            else if (xmlNode is XmlText && parentObject is TreeNode)
            {
                var treeNode = (TreeNode)parentObject;
                if (treeNode.Tag is Dictionary<string, string>)
                {
                    var attributes = (Dictionary<string, string>)treeNode.Tag;
                    attributes.Add("#text", ((XmlText)xmlNode).Value);
                }
            }
            return node;
        }

        public static void SetNodeText(TreeNode node)
        {
            if (node == null)
            {
                return;
            }
            var text = node.Name;
            if ((node.Name == "Export" || node.Name == "Import") &&
                node.Parent?.Name == "SolutionBlock")
            {
                text += GetAttributeFromNode(node, "Type");
            }
            else if (node.Name == "Export" && node.Nodes.ContainsKey("FetchXML"))
            {
                text += " FetchXML";
            }
            else if (node.Name == "Filter")
            {
                text +=
                    GetAttributeFromNode(node, "Attribute") +
                    GetAttributeFromNode(node, "Operator") +
                    GetAttributeFromNode(node, "Value");
            }
            else if (node.Name == "#comment")
            {
                text = GetAttributeFromNode(node, "#comment").Trim().Replace("\r\n", "  ");
                if (string.IsNullOrWhiteSpace(text))
                {
                    text = " - comment - ";
                }
            }
            else
            {
                text += GetAttributeFromNode(node, "Name");
                text += GetAttributeFromNode(node, "Block");
                text += GetAttributeFromNode(node, "Attribute");
            }
            if (node.Name == "Blocks" || node.Name == "Attributes" || node.Name == "Match" || node.Name == "PreRequisites")
            {
                text += $" ({node.Nodes.Count})";
            }
            node.Text = text.Trim();
        }

        /// <summary>
        /// Adds a context menu to a TreeNode control
        /// </summary>
        /// <param name="node">TreeNode where to add the context menu</param>
        /// <param name="tree">Current application form</param>
        public static void AddContextMenu(TreeNode node, ShuffleBuilder tree)
        {

            tree.addMenu.Items.Clear();
            tree.gbNodeQuickActions.Controls.Clear();
            if (node == null && tree.tvDefinition.Nodes.Count > 0)
            {
                node = tree.tvDefinition.Nodes[0];
            }
            if (node != null)
            {
                var nodecapabilities = new DefinitionNodeCapabilities(node);

                foreach (var childcapability in nodecapabilities.ChildTypes)
                {
                    if (childcapability.Name == "-")
                    {
                        tree.addMenu.Items.Add(new ToolStripSeparator());
                        AddLinkSeparator(tree);
                    }
                    else if (childcapability.Multiple || !node.Nodes.ContainsKey(childcapability.Name))
                    {
                        AddMenuFromCapability(tree.addMenu, childcapability.Name);
                        AddLinkFromCapability(tree, childcapability.Name, null, childcapability.Name == "#comment");
                    }
                }
                if (tree.addMenu.Items.Count == 0)
                {
                    AddLinkFromCapability(tree, "nothing to add", string.Empty);
                    var dummy = tree.addMenu.Items.Add("nothing to add");
                    dummy.Enabled = false;
                }

                tree.deleteToolStripMenuItem.Enabled = nodecapabilities.Delete;
                tree.commentToolStripMenuItem.Enabled = nodecapabilities.Comment;
                tree.uncommentToolStripMenuItem.Enabled = nodecapabilities.Uncomment;
                tree.cutToolStripMenuItem.Enabled = nodecapabilities.CutCopy;
                tree.copyToolStripMenuItem.Enabled = nodecapabilities.CutCopy;
                tree.pasteToolStripMenuItem.Enabled = tree.clipboard.IsValidForPaste(node);

                node.ContextMenuStrip = tree.nodeMenu;
            }
            return;
        }

        private static void AddLinkSeparator(ShuffleBuilder tree)
        {
            var sep = new Label
            {
                AutoSize = true,
                Dock = DockStyle.Left,
                Text = "|"
            };
            tree.gbNodeQuickActions.Controls.Add(sep);
            sep.BringToFront();
        }

        private static void AddLinkFromCapability(ShuffleBuilder tree, string name, string tag = null, bool alignright = false)
        {
            var link = new LinkLabel
            {
                AutoSize = true,
                Dock = alignright ? DockStyle.Right : DockStyle.Left,
                TabIndex = tree.gbNodeQuickActions.Controls.Count,
                TabStop = true,
                Text = name
            };
            var tagstr = tag ?? name;
            if (!string.IsNullOrEmpty(tagstr))
            {
                link.Tag = tagstr;
                link.LinkBehavior = LinkBehavior.HoverUnderline;
                link.LinkClicked += tree.QuickActionLink_LinkClicked;
            }
            else
            {
                link.Enabled = false;
            }
            tree.gbNodeQuickActions.Controls.Add(link);
            if (!alignright)
            {
                link.BringToFront();
            }
        }

        private static void AddMenuFromCapability(ToolStrip owner, string name, bool alignright = false, string prefix = "")
        {
            var additem = owner.Items.Add(prefix + name);
            additem.Tag = name;
            if (alignright)
            {
                additem.Alignment = ToolStripItemAlignment.Right;
            }
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

        internal static TreeNode AddChildNode(TreeNode parentNode, string name)
        {
            var childNode = new TreeNode(name)
            {
                Tag = new Dictionary<string, string>()
            };
            childNode.Name = childNode.Text.Replace(" ", "");
            if (name == "#comment")
            {
                childNode.ForeColor = System.Drawing.Color.Gray;
            }
            if (parentNode != null)
            {
                var parentCap = new DefinitionNodeCapabilities(parentNode);
                var nodeIndex = parentCap.IndexOfChild(name);
                var pos = 0;
                while (pos < parentNode.Nodes.Count && nodeIndex >= parentCap.IndexOfChild(parentNode.Nodes[pos].Name))
                {
                    pos++;
                }
                if (pos == parentNode.Nodes.Count)
                {
                    parentNode.Nodes.Add(childNode);
                }
                else
                {
                    parentNode.Nodes.Insert(pos, childNode);
                }
                if (parentNode.Name == "DataBlock")
                {
                    if (name == "Export")
                    {
                        //var attributesNode = AddChildNode(childNode, "Attributes");
                        //AddChildNode(attributesNode, "Attribute");
                    }
                    else if (name == "Import")
                    {
                        var matchNode = AddChildNode(childNode, "Match");
                        AddChildNode(matchNode, "Attribute");
                    }
                }
            }
            return childNode;
        }

        internal static void SetNodeTooltip(TreeNode node)
        {
            if (node != null)
            {
                var tooltip = GetNodeXml(node);
                node.ToolTipText = tooltip;
                if (node.Parent != null)
                {
                    SetNodeTooltip(node.Parent);
                }
            }
        }

        internal static string GetNodeXml(TreeNode node)
        {
            if (node == null)
            {
                return string.Empty;
            }
            var doc = new XmlDocument();
            XmlNode rootNode = doc.CreateElement("root");
            doc.AppendChild(rootNode);
            TreeNodeHelper.AddXmlNode(node, rootNode);
            var tooltip = "";
            try
            {
                var xdoc = XDocument.Parse(rootNode.InnerXml);
                tooltip = xdoc.ToString();
            }
            catch
            {
                tooltip = rootNode.InnerXml;
            }
            return tooltip;
        }

        internal static void AddXmlNode(TreeNode currentNode, XmlNode parentXmlNode)
        {
            if (currentNode?.Tag is Dictionary<string, string> collec)
            {
                XmlNode newNode;
                if (currentNode.Name == "#comment")
                {
                    newNode = parentXmlNode.OwnerDocument.CreateComment(collec.ContainsKey("#comment") ? collec["#comment"] : "");
                }
                else
                {
                    newNode = parentXmlNode.OwnerDocument.CreateElement(currentNode.Name);
                    foreach (var key in collec.Keys)
                    {
                        if (key == "#text")
                        {
                            var newText = parentXmlNode.OwnerDocument.CreateTextNode(collec[key]);
                            newNode.AppendChild(newText);
                        }
                        else
                        {
                            var attr = parentXmlNode.OwnerDocument.CreateAttribute(key);
                            attr.Value = collec[key];
                            newNode.Attributes.Append(attr);
                        }
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
                }
                parentXmlNode.AppendChild(newNode);
            }
        }

        internal static string GetAttributeFromNode(TreeNode treeNode, string attribute)
        {
            var result = "";
            if (treeNode != null && treeNode.Tag != null && treeNode.Tag is Dictionary<string, string>)
            {
                var collection = (Dictionary<string, string>)treeNode.Tag;
                if (collection.ContainsKey(attribute))
                {
                    result = collection[attribute];
                }
            }
            if (!string.IsNullOrEmpty(result.Trim()))
            {
                result = " " + result.Trim();
            }
            return result;
        }

        #endregion Methods
    }
}
