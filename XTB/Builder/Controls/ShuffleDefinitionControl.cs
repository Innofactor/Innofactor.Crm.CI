using Innofactor.Crm.Shuffle.Builder.AppCode;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    public partial class ShuffleDefinitionControl : UserControl, IDefinitionSavable
    {
        private readonly Dictionary<string, string> collec;
        private string controlsCheckSum = "";

        #region Delegates

        public delegate void SaveEventHandler(object sender, SaveEventArgs e);

        #endregion

        #region Event Handlers

        public event SaveEventHandler Saved;

        #endregion

        public ShuffleDefinitionControl()
        {
            InitializeComponent();
            collec = new Dictionary<string, string>();
        }

        public ShuffleDefinitionControl(Dictionary<string, string> collection, ShuffleBuilder shuffleBuilder)
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
            txtTimeout.Text = collec.ContainsKey("Timeout") ? collec["Timeout"] : "";
            chkStopOnError.Checked = bool.Parse(collec.ContainsKey("StopOnError") ? collec["StopOnError"] : "false");
            controlsCheckSum = ControlsChecksum();
        }

        public void Save()
        {
            Dictionary<string, string> collection = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(txtTimeout.Text))
            {
                int timeout = 0;
                if (!int.TryParse(txtTimeout.Text, out timeout))
                {
                    MessageBox.Show("Timeout must be numeric");
                    return;
                }
                collection.Add("Timeout", timeout.ToString());
            }
            collection.Add("StopOnError", chkStopOnError.Checked ? "true" : "false");
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
            return string.Format("{0}/{1}", txtTimeout.Text, chkStopOnError.Checked);
        }

        private void ShuffleDefinitionControl_Leave(object sender, EventArgs e)
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
