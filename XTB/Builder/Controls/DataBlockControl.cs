using System;
using System.Collections.Generic;

namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    public partial class DataBlockControl : ControlBase
    {
        public DataBlockControl(Dictionary<string, string> collection, ShuffleBuilder shuffleBuilder)
            : base(collection, shuffleBuilder) { }

        public override ControlCollection GetControls()
        {
            if (Controls?.Count == 0)
            {
                InitializeComponent();
            }
            return Controls;
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
