using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    public partial class DataBlockControl : ControlBase
    {
        List<EntityMetadata> entities;

        public DataBlockControl(Dictionary<string, string> collection, ShuffleBuilder shuffleBuilder, List<EntityMetadata> entities)
            : base(collection, shuffleBuilder)
        {
            this.entities = entities;
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
            if (shuffleBuilder.Entities == null)
            {
                shuffleBuilder.LoadEntities(PopulateControls);
                return;
            }
            cmbEntity.Items.Clear();
            cmbEntity.Items.AddRange(shuffleBuilder.Entities.OrderBy(e => e.LogicalName).Select(e => e.LogicalName).ToArray());
            if (cmbEntity.Items.Count > 0)
            {
                cmbEntity.DropDownStyle = ComboBoxStyle.DropDown;
            }
            else
            {
                cmbEntity.DropDownStyle = ComboBoxStyle.Simple;
            }
            base.PopulateControls();
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtIntersect.Enabled = cmbType.Text == "Intersect";
            if (!txtIntersect.Enabled)
            {
                txtIntersect.Text = string.Empty;
            }
        }
    }
}
