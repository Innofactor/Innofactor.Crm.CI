using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    public partial class PreReqSolutionControl : ControlBase
    {
        public PreReqSolutionControl(Dictionary<string, string> collection, ShuffleBuilder shuffleBuilder)
            : base(collection, shuffleBuilder) { }

        public override ControlCollection GetControls()
        {
            if (Controls?.Count == 0)
            {
                InitializeComponent();
            }
            return Controls;
        }

        public override void Save()
        {
            if (VersionRequired())
            {
                if (string.IsNullOrWhiteSpace(txtVersion.Text))
                {
                    MessageBox.Show("Enter version");
                    return;
                }
            }
            else
            {
                txtVersion.Text = string.Empty;
            }
            base.Save();
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtVersion.Enabled = VersionRequired();
        }

        private bool VersionRequired()
        {
            return (cmbComparer.Text == "eq" || cmbComparer.Text == "ge");
        }
    }
}
