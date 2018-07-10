using Innofactor.Crm.Shuffle.Builder.AppCode;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    public partial class SolutionBlockControl : UserControl, IDefinitionSavable
    {
        private readonly Dictionary<string, string> collec;
        private string controlsCheckSum = "";

        #region Delegates

        public delegate void SaveEventHandler(object sender, SaveEventArgs e);

        #endregion

        #region Event Handlers

        public event SaveEventHandler Saved;

        #endregion

        public SolutionBlockControl()
        {
            InitializeComponent();
            collec = new Dictionary<string, string>();
        }

        public SolutionBlockControl(Dictionary<string, string> collection, ShuffleBuilder shuffleBuilder)
            : this()
        {
            if (collection != null)
            {
                collec = collection;
            }

            cmbName.Items.Clear();
            if (shuffleBuilder.Solutions != null && shuffleBuilder.Solutions.Count > 0)
            {
                //cmbName.Items.AddRange(shuffleBuilder.Solutions.ToArray());
                foreach (var solution in shuffleBuilder.Solutions)
                {
                    cmbName.Items.Add(solution.Property("uniquename", "-"));
                }
                cmbName.DropDownStyle = ComboBoxStyle.DropDown;
            }
            else
            {
                cmbName.DropDownStyle = ComboBoxStyle.Simple;
            }
            FillControls();
            Saved += shuffleBuilder.CtrlSaved;
        }

        private void FillControls()
        {
            //var name = collec.ContainsKey("Name") ? collec["Name"] : "";
            //cmbName.SelectedIndex = -1;
            //foreach (CintDynEntity solution in cmbName.Items)
            //{
            //    if (solution.Property("uniquename", "-") == name)
            //    {
            //        cmbName.SelectedItem = solution;
            //        break;
            //    }
            //}
            cmbName.Text = collec.ContainsKey("Name") ? collec["Name"] : "";
            txtPath.Text = collec.ContainsKey("Path") ? collec["Path"] : "";
            txtFile.Text = collec.ContainsKey("File") ? collec["File"] : "";
            controlsCheckSum = ControlsChecksum();
        }

        public void Save()
        {
            Dictionary<string, string> collection = new Dictionary<string, string>();
            //var name = cmbName.SelectedItem != null ? ((CintDynEntity)cmbName.SelectedItem).Property("uniquename", "") : "";
            var name = cmbName.Text;
            if (!string.IsNullOrWhiteSpace(name))
            {
                collection.Add("Name", name);
            }
            else
            {
                MessageBox.Show("Solution name must be entered");
                return;
            }
            if (!string.IsNullOrWhiteSpace(txtPath.Text))
            {
                collection.Add("Path", txtPath.Text);
            }
            if (!string.IsNullOrWhiteSpace(txtFile.Text))
            {
                collection.Add("File", txtFile.Text);
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

        public string ControlsChecksum()
        {
            return string.Format("{0}/{1}/{2}", cmbName.Text, txtPath.Text, txtFile.Text);
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
