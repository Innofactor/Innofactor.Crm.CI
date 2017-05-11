using Innofactor.Crm.Shuffle.Builder.AppCode;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    public partial class SettingsControl : UserControl, IDefinitionSavable
    {
        private readonly Dictionary<string, string> collec;
        private string controlsCheckSum = "";

        #region Delegates

        public delegate void SaveEventHandler(object sender, SaveEventArgs e);

        #endregion

        #region Event Handlers

        public event SaveEventHandler Saved;

        #endregion

        public SettingsControl()
        {
            InitializeComponent();
            collec = new Dictionary<string, string>();
        }

        public SettingsControl(Dictionary<string, string> collection, ShuffleBuilder shuffleBuilder)
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
            //FillCheckbox(chkAutonum);
            //FillCheckbox(chkCalendar);
            //FillCheckbox(chkCustomization);
            //FillCheckbox(chkEmailTracking);
            //FillCheckbox(chkGeneral);
            //FillCheckbox(chkIsvConfig);
            //FillCheckbox(chkMarketing);
            //FillCheckbox(chkOutlookSync);
            //FillCheckbox(chkRelationshipRoles);
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
            SaveEventArgs sea = new SaveEventArgs { AttributeCollection = collection };

            if (Saved != null)
            {
                Saved(this, sea);
            }
        }

        public string ControlsChecksum()
        {
            var checksum = "";
            foreach (var chk in GetCheckboxes())
            {
                checksum += chk.Checked.ToString() + "/";
            }
            return checksum;
        }

        private void DataBlockExportControl_Leave(object sender, EventArgs e)
        {
            if (controlsCheckSum != ControlsChecksum())
            {
                if (MessageBox.Show("Save changes?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Save();
                }
            }
        }
    }
}
