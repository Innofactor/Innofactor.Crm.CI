using Innofactor.Crm.Shuffle.Builder.AppCode;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    public partial class DataBlockImportControl : UserControl, IDefinitionSavable
    {
        private readonly Dictionary<string, string> collec;
        private string controlsCheckSum = "";

        #region Delegates

        public delegate void SaveEventHandler(object sender, SaveEventArgs e);

        #endregion

        #region Event Handlers

        public event SaveEventHandler Saved;

        #endregion

        public DataBlockImportControl()
        {
            InitializeComponent();
            collec = new Dictionary<string, string>();
        }

        public DataBlockImportControl(Dictionary<string, string> collection, ShuffleBuilder shuffleBuilder)
            : this()
        {
            if (collection != null)
            {
                collec = collection;
            }

            FillControls();
            Saved += shuffleBuilder.CtrlSaved;
        }

        private void FillControls()
        {
            chkCreateWithId.Checked = bool.Parse(collec.ContainsKey("CreateWithId") ? collec["CreateWithId"] : "false");
            cmbSave.SelectedIndex = cmbSave.FindStringExact(collec.ContainsKey("Save") ? collec["Save"] : "");
            cmbDelete.SelectedIndex = cmbDelete.FindStringExact(collec.ContainsKey("Delete") ? collec["Delete"] : "");
            chkUpdateInactive.Checked = bool.Parse(collec.ContainsKey("UpdateInactive") ? collec["UpdateInactive"] : "false");
            chkUpdateIdentical.Checked = bool.Parse(collec.ContainsKey("UpdateIdentical") ? collec["UpdateIdentical"] : "false");
            var overwrite = collec.ContainsKey("Overwrite") ? collec["Overwrite"] : "";
            if (!string.IsNullOrWhiteSpace(overwrite))
            {
                txtOverwrite.Text = overwrite;
                MessageBox.Show("This is currently using deprecated attribute Overwrite.\nPlease specify Save-option instead.");
            }
            controlsCheckSum = ControlsChecksum();
        }

        public void Save()
        {
            Dictionary<string, string> collection = new Dictionary<string, string>();
            collection.Add("CreateWithId", chkCreateWithId.Checked ? "true" : "false");
            if (cmbSave.SelectedIndex >= 0)
            {
                collection.Add("Save", cmbSave.Text);
            }
            else
            {
                MessageBox.Show("Select Save-option");
                return;
            }
            if (cmbDelete.SelectedIndex >= 0)
            {
                collection.Add("Delete", cmbDelete.Text);
            }
            collection.Add("UpdateInactive", chkUpdateInactive.Checked ? "true" : "false");
            collection.Add("UpdateIdentical", chkUpdateIdentical.Checked ? "true" : "false");
            controlsCheckSum = ControlsChecksum();
            SendSaveMessage(collection);
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
            return string.Format("{0}/{1}/{2}/{3}", chkCreateWithId.Checked, cmbSave.SelectedIndex, cmbDelete.SelectedIndex, chkUpdateInactive.Checked);
        }

        private void DataBlockImportControl_Leave(object sender, EventArgs e)
        {
            if (controlsCheckSum != ControlsChecksum())
            {
                if (MessageBox.Show("Save changes?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Save();
                }
            }
        }

        private void cmbDelete_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
