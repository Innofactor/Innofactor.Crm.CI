using Innofactor.Crm.Shuffle.Builder.AppCode;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    public partial class PreReqSolutionControl : UserControl, IDefinitionSavable
    {
        private readonly Dictionary<string, string> collec;
        private string controlsCheckSum = "";

        #region Delegates

        public delegate void SaveEventHandler(object sender, SaveEventArgs e);

        #endregion

        #region Event Handlers

        public event SaveEventHandler Saved;

        #endregion

        public PreReqSolutionControl()
        {
            InitializeComponent();
            collec = new Dictionary<string, string>();
        }

        public PreReqSolutionControl(Dictionary<string, string> collection, ShuffleBuilder shuffleBuilder)
            : this()
        {
            if (collection != null)
                collec = collection;

            FillControls();
            Saved += shuffleBuilder.CtrlSaved;
        }

        private void FillControls()
        {
            txtName.Text = collec.ContainsKey("Name") ? collec["Name"] : "";
            cmbComparer.SelectedIndex = cmbComparer.FindStringExact(collec.ContainsKey("Comparer") ? collec["Comparer"] : "");
            txtVersion.Text = collec.ContainsKey("Version") ? collec["Version"] : "";
            controlsCheckSum = ControlsChecksum();
        }

        public void Save()
        {
            Dictionary<string, string> collection = new Dictionary<string, string>();
            var name = txtName.Text;
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Enter solution name");
                return;
            }
            collection.Add("Name", txtName.Text);
            if (cmbComparer.SelectedItem == null)
            {
                MessageBox.Show("Enter comparer");
                return;
            }
            collection.Add("Comparer", cmbComparer.Text);
            if (VersionRequired())
            {
                if (string.IsNullOrWhiteSpace(txtVersion.Text))
                {
                    MessageBox.Show("Enter version");
                    return;
                }
                collection.Add("Version", txtVersion.Text);
            }
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

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtVersion.Enabled = VersionRequired();
        }

        private bool VersionRequired()
        {
            return (cmbComparer.Text == "eq" || cmbComparer.Text == "ge");
        }

        public string ControlsChecksum()
        {
            return string.Format("{0}/{1}/{2}", txtName.Text, cmbComparer.SelectedIndex, txtVersion.Text);
        }

        private void DataBlockControl_Leave(object sender, EventArgs e)
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
