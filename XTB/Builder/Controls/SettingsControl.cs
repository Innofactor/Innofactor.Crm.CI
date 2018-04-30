using System.Collections.Generic;

namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    public partial class SettingsControl : ControlBase
    {
        public SettingsControl(Dictionary<string, string> collection, ShuffleBuilder shuffleBuilder)
            : base(collection, shuffleBuilder) { }

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
