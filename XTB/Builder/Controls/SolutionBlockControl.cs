﻿using Innofactor.Xrm.Utils.Common.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    public partial class SolutionBlockControl : ControlBase
    {
        public SolutionBlockControl(Dictionary<string, string> collection, ShuffleBuilder shuffleBuilder)
            : base(collection, shuffleBuilder) { }

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
            if (shuffleBuilder.Solutions == null)
            {
                shuffleBuilder.LoadSolutions(PopulateControls);
                return;
            }
            cmbName.Items.Clear();
            cmbName.Items.AddRange(shuffleBuilder.Solutions.Entities
                .Select(s => s.GetAttribute("uniquename", string.Empty))
                .Where(s => !string.IsNullOrEmpty(s))
                .OrderBy(s => s)
                .ToArray());
            if (cmbName.Items.Count > 0)
            {
                cmbName.DropDownStyle = ComboBoxStyle.DropDown;
            }
            else
            {
                cmbName.DropDownStyle = ComboBoxStyle.Simple;
            }
            base.PopulateControls();
        }
    }
}
