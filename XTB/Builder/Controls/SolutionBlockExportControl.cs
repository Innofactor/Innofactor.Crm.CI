using Innofactor.Crm.Shuffle.Builder.AppCode;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    public partial class SolutionBlockExportControl : UserControl, IDefinitionSavable
    {
        private readonly Dictionary<string, string> collec;
        private string controlsCheckSum = "";

        #region Delegates

        public delegate void SaveEventHandler(object sender, SaveEventArgs e);

        #endregion

        #region Event Handlers

        public event SaveEventHandler Saved;

        #endregion

        public SolutionBlockExportControl()
        {
            InitializeComponent();
            collec = new Dictionary<string, string>();
        }

        public SolutionBlockExportControl(Dictionary<string, string> collection, ShuffleBuilder shuffleBuilder)
            : this()
        {
            if (collection != null)
                collec = collection;

            FillControls();
            Saved += shuffleBuilder.CtrlSaved;
        }

        private void FillControls()
        {
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
            txtVersion.Text = collec.ContainsKey("SetVersion") ? collec["SetVersion"] : "";
            chkPublish.Checked = bool.Parse(collec.ContainsKey("PublishBeforeExport") ? collec["PublishBeforeExport"] : "false");
            txtTarget.Text = collec.ContainsKey("TargetVersion") ? collec["TargetVersion"] : "";
            controlsCheckSum = ControlsChecksum();
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
            var version = txtVersion.Text;
            if (!string.IsNullOrWhiteSpace(version))
            {
                collection.Add("SetVersion", version);
            }
            collection.Add("PublishBeforeExport", chkPublish.Checked ? "true" : "false");
            var target = txtTarget.Text;
            if (!string.IsNullOrWhiteSpace(target))
            {
                collection.Add("TargetVersion", target);
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
            Saved?.Invoke(this, new SaveEventArgs { AttributeCollection = collection });
        }

        public string ControlsChecksum()
        {
            return string.Format("{0}/{1}/{2}/{3}", cmbType.SelectedIndex, txtVersion.Text, chkPublish.Checked, txtTarget.Text);
        }

        private void DataBlockImportControl_Leave(object sender, EventArgs e)
        {
            if (controlsCheckSum != ControlsChecksum())
            {
                Save();
            }
        }
    }
}
