using Innofactor.Crm.Shuffle.Builder.AppCode;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    public partial class FilterControl : UserControl, IDefinitionSavable
    {
        private readonly Dictionary<string, string> collec;
        private string controlsCheckSum = "";

        #region Delegates

        public delegate void SaveEventHandler(object sender, SaveEventArgs e);

        #endregion

        #region Event Handlers

        public event SaveEventHandler Saved;

        #endregion

        public FilterControl()
        {
            InitializeComponent();
            collec = new Dictionary<string, string>();
        }

        public FilterControl(Dictionary<string, string> collection, ShuffleBuilder shuffleBuilder)
            : this()
        {
            if (collection != null)
                collec = collection;

            FillControls();
            Saved += shuffleBuilder.CtrlSaved;
        }

        private void FillControls()
        {
            txtAttribute.Text = collec.ContainsKey("Attribute") ? collec["Attribute"] : "";
            cmbOperator.SelectedIndex = cmbOperator.FindStringExact(collec.ContainsKey("Operator") ? collec["Operator"] : "");
            cmbType.SelectedIndex = cmbType.FindStringExact(collec.ContainsKey("Type") ? collec["Type"] : "");
            txtValue.Text = collec.ContainsKey("Value") ? collec["Value"] : "";
            controlsCheckSum = ControlsChecksum();
        }

        public void Save()
        {
            Dictionary<string, string> collection = new Dictionary<string, string>();
            collection.Add("Attribute", txtAttribute.Text);
            collection.Add("Operator", cmbOperator.Text);
            collection.Add("Type", cmbType.Text);
            collection.Add("Value", txtValue.Text);
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
            return string.Format("{0}/{1}/{2}/{3}", txtAttribute.Text, cmbOperator.SelectedIndex, cmbType.SelectedIndex, txtValue.Text);
        }

        private void FilterControl_Leave(object sender, EventArgs e)
        {
            if (controlsCheckSum != ControlsChecksum())
            {
                Save();
            }
        }
    }
}
