using System.Collections.Generic;
using System.Windows.Forms;

namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    public partial class DataBlockImportControl : ControlBase
    {
        public DataBlockImportControl(Dictionary<string, string> collection, ShuffleBuilder shuffleBuilder)
            : base(collection, shuffleBuilder)
        {
            var overwrite = collection.ContainsKey("Overwrite") ? collection["Overwrite"] : "";
            if (!string.IsNullOrWhiteSpace(overwrite))
            {
                txtOverwrite.Text = overwrite;
                lblDeprecated.Visible = true;
                lblDeprOverwrite.Visible = true;
                txtOverwrite.Visible = true;
                MessageBox.Show("This is currently using deprecated attribute Overwrite.\nPlease specify Save-option instead.");
            }
        }

        public override ControlCollection GetControls()
        {
            if (Controls?.Count == 0)
            {
                InitializeComponent();
            }
            return Controls;
        }
    }
}
