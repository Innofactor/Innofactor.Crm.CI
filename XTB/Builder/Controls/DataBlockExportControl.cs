using Innofactor.Crm.Shuffle.Builder.AppCode;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    public partial class DataBlockExportControl : UserControl, IDefinitionSavable
    {
        private readonly Dictionary<string, string> collec;
        private string controlsCheckSum = "";

        #region Delegates

        public delegate void SaveEventHandler(object sender, SaveEventArgs e);

        #endregion

        #region Event Handlers

        public event SaveEventHandler Saved;

        #endregion

        public DataBlockExportControl()
        {
            InitializeComponent();
            collec = new Dictionary<string, string>();
        }

        public DataBlockExportControl(Dictionary<string, string> collection, ShuffleBuilder shuffleBuilder)
            : this()
        {
            if (collection != null)
                collec = collection;

            FillControls();
            Saved += shuffleBuilder.CtrlSaved;
        }

        private void FillControls()
        {
            chkActiveOnly.Checked = bool.Parse(collec.ContainsKey("ActiveOnly") ? collec["ActiveOnly"] : "true");
            controlsCheckSum = ControlsChecksum();
        }

        public void Save()
        {
            Dictionary<string, string> collection = new Dictionary<string, string>();
            collection.Add("ActiveOnly", chkActiveOnly.Checked ? "true" : "false");
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
            return string.Format("{0}", chkActiveOnly.Checked);
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
