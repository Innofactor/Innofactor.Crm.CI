using Innofactor.Crm.Shuffle.Builder.AppCode;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    public partial class DataBlockControl : UserControl, IDefinitionSavable
    {
        private readonly Dictionary<string, string> collec;
        private string controlsCheckSum = "";

        #region Delegates

        public delegate void SaveEventHandler(object sender, SaveEventArgs e);

        #endregion

        #region Event Handlers

        public event SaveEventHandler Saved;

        #endregion

        public DataBlockControl()
        {
            InitializeComponent();
            collec = new Dictionary<string, string>();
        }

        public DataBlockControl(Dictionary<string, string> collection, ShuffleBuilder shuffleBuilder)
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
            txtEntity.Text = collec.ContainsKey("Entity") ? collec["Entity"] : "";
            cmbType.SelectedIndex = cmbType.FindStringExact(collec.ContainsKey("Type") ? collec["Type"] : "");
            txtIntersect.Text = collec.ContainsKey("IntersectName") ? collec["IntersectName"] : "";
            controlsCheckSum = ControlsChecksum();
        }

        public void Save()
        {
            Dictionary<string, string> collection = new Dictionary<string, string>();
            collection.Add("Name", txtName.Text);
            collection.Add("Entity", txtEntity.Text);
            if (cmbType.SelectedItem != null)
            {
                collection.Add("Type", cmbType.Text);
                if (cmbType.Text == "Intersect" && !string.IsNullOrWhiteSpace(txtIntersect.Text))
                {
                    collection.Add("IntersectName", txtIntersect.Text);
                }
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

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtIntersect.Enabled = cmbType.Text == "Intersect";
        }

        public string ControlsChecksum()
        {
            return string.Format("{0}/{1}/{2}/{3}", txtName.Text, txtEntity.Text, cmbType.SelectedIndex, txtIntersect.Text);
        }

        private void DataBlockControl_Leave(object sender, EventArgs e)
        {
            if (controlsCheckSum != ControlsChecksum())
            {
                Save();
            }
        }
    }
}
