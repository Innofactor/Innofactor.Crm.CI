using System.Collections.Generic;
using System.Windows.Forms;

namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    public partial class ShuffleDefinitionControl : ControlBase
    {
        public ShuffleDefinitionControl(Dictionary<string, string> collection, ShuffleBuilder shuffleBuilder)
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
            if (!string.IsNullOrWhiteSpace(txtTimeout.Text))
            {
                int timeout = 0;
                if (!int.TryParse(txtTimeout.Text, out timeout))
                {
                    MessageBox.Show("Timeout must be numeric");
                    return;
                }
            }
            base.Save();
        }
    }
}
