using System.Collections.Generic;
using System.Windows.Forms;

namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    public partial class RelationControl : ControlBase
    {
        private TreeNode node;

        public RelationControl(Dictionary<string, string> collection, TreeNode selectedNode, ShuffleBuilder shuffleBuilder)
            : base(collection, shuffleBuilder)
        {
            node = selectedNode;
        }

        public override ControlCollection GetControls()
        {
            if (Controls?.Count == 0)
            {
                InitializeComponent();
            }
            return Controls;
        }

        public override void PopulateControls()
        {
            var datablock = node.Parent;
            var blocks = datablock.Parent;
            cmbBlock.Items.Clear();
            var i = 0;
            while (i < blocks.Nodes.Count && blocks.Nodes[i] != datablock)
            {
                var blockname = blocks.Nodes[i].Text;
                if (blockname.StartsWith("DataBlock"))
                {
                    cmbBlock.Items.Add(blockname.Replace("DataBlock", "").Trim());
                }
                i++;
            }
        }
    }
}
