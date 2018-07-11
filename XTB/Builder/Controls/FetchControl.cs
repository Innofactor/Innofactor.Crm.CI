using System.Collections.Generic;

namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    public partial class FetchControl : ControlBase
    {
        string entity;

        public FetchControl(Dictionary<string, string> collection, ShuffleBuilder shuffleBuilder, string entity)
            : base(collection, shuffleBuilder)
        {
            this.entity = entity;
        }

        public override ControlCollection GetControls()
        {
            if (InitializationNeeded(Controls))
            {
                InitializeComponent();
            }
            return Controls;
        }

        public override void PopulateControls()
        {
            base.PopulateControls();
            if (string.IsNullOrEmpty(txtFetchXML.Text))
            {
                txtFetchXML.Text = $@"<fetch>
  <entity name='{entity}' />
</fetch>";
            }
            txtFetchXML.Process();
        }

        private void btnFXB_Click(object sender, System.EventArgs e)
        {
            shuffleBuilder.CallFXB(txtFetchXML.Text);
        }

        private void btnFormat_Click(object sender, System.EventArgs e)
        {
            txtFetchXML.Process();
        }
    }
}
