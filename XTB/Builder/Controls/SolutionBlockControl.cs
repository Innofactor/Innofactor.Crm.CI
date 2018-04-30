using System.Collections.Generic;
using System.Windows.Forms;

namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    public partial class SolutionBlockControl : ControlBase
    {
        public SolutionBlockControl(Dictionary<string, string> collection, ShuffleBuilder shuffleBuilder)
            : base(collection, shuffleBuilder) { }

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
            cmbName.Items.Clear();
            if (shuffleBuilder.Solutions != null && shuffleBuilder.Solutions.Count > 0)
            {
                //cmbName.Items.AddRange(shuffleBuilder.Solutions.ToArray());
                foreach (var solution in shuffleBuilder.Solutions)
                {
                    cmbName.Items.Add(solution.Property("uniquename", "-"));
                }
                cmbName.DropDownStyle = ComboBoxStyle.DropDown;
            }
            else
            {
                cmbName.DropDownStyle = ComboBoxStyle.Simple;
            }
        }
    }
}
