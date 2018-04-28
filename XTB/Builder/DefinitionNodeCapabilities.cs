// PROJECT : MsCrmTools.SiteMapEditor
// This project was developed by Tanguy Touzard
// CODEPLEX: http://xrmtoolbox.codeplex.com
// BLOG: http://mscrmtools.blogspot.com

using Cinteros.Xrm.XmlEditorUtils;
using System.Windows.Forms;

namespace Innofactor.Crm.Shuffle.Builder.AppCode
{
    internal class DefinitionNodeCapabilities : TreeNodeCapabilities
    {
        public DefinitionNodeCapabilities(TreeNode node) : base(node)
        {
            switch (Name.ToLowerInvariant())
            {
                case "shuffledefinition":
                    Comment = false;
                    Delete = false;
                    CutCopy = false;
                    ChildTypes.Add(new ChildNodeCapabilities("Blocks", false));
                    break;
                case "blocks":
                    Delete = false;
                    CutCopy = false;
                    ChildTypes.Add(new ChildNodeCapabilities("DataBlock", true));
                    ChildTypes.Add(new ChildNodeCapabilities("SolutionBlock", true));
                    ChildTypes.Add(new ChildNodeCapabilities("#comment", true));
                    break;
                case "datablock":
                    ChildTypes.Add(new ChildNodeCapabilities("Export", false));
                    ChildTypes.Add(new ChildNodeCapabilities("Import", false));
                    ChildTypes.Add(new ChildNodeCapabilities("Relation", true));
                    break;
                case "solutionblock":
                    ChildTypes.Add(new ChildNodeCapabilities("Export", false));
                    ChildTypes.Add(new ChildNodeCapabilities("Import", false));
                    break;
                case "export":
                    if (node.Parent.Name.ToLowerInvariant().StartsWith("datablock"))
                    {
                        ChildTypes.Add(new ChildNodeCapabilities("Filter", true));
                        ChildTypes.Add(new ChildNodeCapabilities("Sort", true));
                    }
                    else if (node.Parent.Name.ToLowerInvariant().StartsWith("solutionblock"))
                    {
                        ChildTypes.Add(new ChildNodeCapabilities("Settings", false));
                    }
                    break;
                case "import":
                    if (node.Parent.Name.ToLowerInvariant().StartsWith("datablock"))
                    {
                        ChildTypes.Add(new ChildNodeCapabilities("Match", false));
                    }
                    else if (node.Parent.Name.ToLowerInvariant().StartsWith("solutionblock"))
                    {
                        ChildTypes.Add(new ChildNodeCapabilities("PreRequisites", false));
                    }
                    break;
                case "attributes":
                    Delete = false;
                    ChildTypes.Add(new ChildNodeCapabilities("Attribute", true));
                    break;
                case "match":
                    ChildTypes.Add(new ChildNodeCapabilities("Attribute", true));
                    break;
                case "attribute":
                    Delete = node.Parent != null && node.Parent.Nodes.Count > 1;
                    break;
                case "filter":
                case "sort":
                case "relation":
                case "settings":
                    break;
                case "prerequisites":
                    ChildTypes.Add(new ChildNodeCapabilities("Solution", true));
                    break;
                case "solution":
                    Delete = node.Parent != null && node.Parent.Nodes.Count > 1;
                    break;
                case "#comment":
                    Comment = false;
                    Uncomment = true;
                    break;
            }
        }
    }
}