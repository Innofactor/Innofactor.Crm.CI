using Innofactor.Crm.Shuffle.Builder.AppCode;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    public partial class RelationControl : UserControl, IDefinitionSavable
    {
        private readonly Dictionary<string, string> collec;
        private string controlsCheckSum = "";

        #region Delegates

        public delegate void SaveEventHandler(object sender, SaveEventArgs e);

        #endregion

        #region Event Handlers

        public event SaveEventHandler Saved;

        #endregion

        public RelationControl()
        {
            InitializeComponent();
            collec = new Dictionary<string, string>();
        }

        public RelationControl(Dictionary<string, string> collection, TreeNode selectedNode, ShuffleBuilder shuffleBuilder)
            : this()
        {
            if (collection != null)
            {
                collec = collection;
            }

            FillOptions(selectedNode);
            FillControls();
            Saved += shuffleBuilder.CtrlSaved;
        }

        private void FillOptions(TreeNode selectedNode)
        {
            var datablock = selectedNode.Parent;
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

        private void FillControls()
        {
            cmbBlock.SelectedIndex = -1;
            var block = collec.ContainsKey("Block") ? collec["Block"] : "";
            if (!string.IsNullOrWhiteSpace(block))
            {
                var index = cmbBlock.FindStringExact(block);
                if (index >= 0)
                {
                    cmbBlock.SelectedIndex = index;
                }
                else
                {
                    MessageBox.Show("Invalid block currently selected: " + block);
                }
            }
            txtAttribute.Text = collec.ContainsKey("Attribute") ? collec["Attribute"] : "";
            txtPKAttribute.Text = collec.ContainsKey("PK-Attribute") ? collec["PK-Attribute"] : "";
            chkIncludeNull.Checked = bool.Parse(collec.ContainsKey("IncludeNull") ? collec["IncludeNull"] : "false");
            controlsCheckSum = ControlsChecksum();
        }

        public void Save()
        {
            Dictionary<string, string> collection = new Dictionary<string, string>();
            collection.Add("Block", cmbBlock.Text);
            collection.Add("Attribute", txtAttribute.Text);
            if (!string.IsNullOrWhiteSpace(txtPKAttribute.Text))
            {
                collection.Add("PK-Attribute", txtPKAttribute.Text);
            }
            collection.Add("IncludeNull", chkIncludeNull.Checked ? "true" : "false");
            controlsCheckSum = ControlsChecksum();
            SendSaveMessage(collection);
        }

        /// <summary>
        /// Sends a connection success message 
        /// </summary>
        /// <param name="service">IOrganizationService generated</param>
        /// <param name="parameters">Lsit of parameter</param>
        private void SendSaveMessage(Dictionary<string, string> collection)
        {
            SaveEventArgs sea = new SaveEventArgs { AttributeCollection = collection };

            if (Saved != null)
            {
                Saved(this, sea);
            }
        }

        public string ControlsChecksum()
        {
            return string.Format("{0}/{1}/{2}/{3}", cmbBlock.SelectedIndex, txtAttribute.Text, txtPKAttribute.Text, chkIncludeNull.Checked);
        }

        private void RelationControl_Leave(object sender, EventArgs e)
        {
            if (controlsCheckSum != ControlsChecksum())
            {
                if (MessageBox.Show("Save changes?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Save();
                }
            }
        }
    }
}
