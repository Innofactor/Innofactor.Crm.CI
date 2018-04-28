using Innofactor.Crm.Shuffle.Builder.AppCode;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    public partial class SolutionBlockImportControl : UserControl, IDefinitionSavable
    {
        private readonly Dictionary<string, string> collec;
        private string controlsCheckSum = "";

        #region Delegates

        public delegate void SaveEventHandler(object sender, SaveEventArgs e);

        #endregion

        #region Event Handlers

        public event SaveEventHandler Saved;

        #endregion

        public SolutionBlockImportControl()
        {
            InitializeComponent();
            collec = new Dictionary<string, string>();
        }

        public SolutionBlockImportControl(Dictionary<string, string> collection, ShuffleBuilder shuffleBuilder)
            : this()
        {
            if (collection != null)
                collec = collection;

            FillControls();
            Saved += shuffleBuilder.CtrlSaved;
        }

        private void FillControls()
        {
            foreach (var chk in GetCheckboxes())
            {
                FillCheckbox(chk);
            }
            cmbType.SelectedIndex = -1;
            var type = collec.ContainsKey("Type") ? collec["Type"] : "";
            if (!string.IsNullOrWhiteSpace(type))
            {
                var index = cmbType.FindStringExact(type);
                if (index >= 0)
                {
                    cmbType.SelectedIndex = index;
                }
                else
                {
                    MessageBox.Show("Invalid type currently selected: " + type);
                }
            }
            controlsCheckSum = ControlsChecksum();
        }

        private void FillCheckbox(CheckBox chk, bool def = false)
        {
            var key = chk.Tag != null ? chk.Tag.ToString() : "dethärvarintebraallsdet!";
            chk.Checked = collec.ContainsKey(key) ? bool.Parse(collec[key]) : def;
        }

        public void Save()
        {
            Dictionary<string, string> collection = new Dictionary<string, string>();
            if (cmbType.SelectedIndex >= 0)
            {
                collection.Add("Type", cmbType.Text);
            }
            else
            {
                MessageBox.Show("Select type");
                return;
            }
            foreach (var chk in GetCheckboxes())
            {
                var key = chk.Tag.ToString();
                collection.Add(key, chk.Checked ? "true" : "false");
            }
            controlsCheckSum = ControlsChecksum();
            SendSaveMessage(collection);
        }

        private List<CheckBox> GetCheckboxes()
        {
            var result = new List<CheckBox>();
            foreach (var control in this.Controls)
            {
                if (control is CheckBox)
                {
                    result.Add((CheckBox)control);
                }
            }
            return result;
        }

        /// <summary>
        /// Sends a connection success message 
        /// </summary>
        /// <param name="service">IOrganizationService generated</param>
        /// <param name="parameters">Lsit of parameter</param>
        private void SendSaveMessage(Dictionary<string, string> collection)
        {
            Saved?.Invoke(this, new SaveEventArgs { AttributeCollection = collection });
        }

        public string ControlsChecksum()
        {
            var checksum = "";
            foreach (var chk in GetCheckboxes())
            {
                checksum += chk.Checked.ToString() + "/";
            }
            checksum += cmbType.Text;
            return checksum;
        }

        private void DataBlockExportControl_Leave(object sender, EventArgs e)
        {
            if (controlsCheckSum != ControlsChecksum())
            {
                Save();
            }
        }
    }
}
