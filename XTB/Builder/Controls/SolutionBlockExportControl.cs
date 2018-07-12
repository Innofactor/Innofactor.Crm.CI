using System.Collections.Generic;

namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    public partial class SolutionBlockExportControl : ControlBase
    {
        public SolutionBlockExportControl(Dictionary<string, string> collection, ShuffleBuilder shuffleBuilder)
            : base(collection, shuffleBuilder) { }

        public override ControlCollection GetControls()
        {
            if (InitializationNeeded(Controls))
            {
                InitializeComponent();
            }
            return Controls;
        }
    }
}
