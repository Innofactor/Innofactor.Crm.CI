using Cinteros.Crm.Utils.Common;
using Cinteros.Crm.Utils.Shuffle;
using Cinteros.Crm.Utils.Shuffle.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace Innofactor.Crm.Shuffle.Runner
{
    public partial class ShuffleRunner : PluginControlBase, IMessageBusHost
    {
        private bool shuffeling = false;
        private bool datafilerequired = true;

        public ShuffleRunner()
        {
            InitializeComponent();
            cmbType.Items.Add(SerializationType.Simple);
            cmbType.Items.Add(SerializationType.SimpleWithValue);
            cmbType.Items.Add(SerializationType.SimpleNoId);
            cmbType.Items.Add(SerializationType.Explicit);
            cmbType.Items.Add(SerializationType.Text);
            cmbType.Items.Add(SerializationType.Full);
            ConnectionUpdated += ShuffleRunner_ConnectionUpdated;
        }

        void ShuffleRunner_ConnectionUpdated(object sender, ConnectionUpdatedEventArgs e)
        {
            EnableShuffle();
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Title = "Select ShuffleDefinition",
                Filter = "XML files (*.xml)|*.xml|All files|*.*"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtFile.Text = ofd.FileName;
            }
        }

        private void btnData_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Title = "Select Data file",
                Filter = "XML files (*.xml)|*.xml|Text files (*.txt)|*.txt|All files|*.*"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtData.Text = ofd.FileName;
            }
        }

        private void btnShuffle_Click(object sender, EventArgs e)
        {
            lbLog.Items.Clear();
            var type = cmbType.SelectedItem != null ? (SerializationType)cmbType.SelectedItem : SerializationType.Simple;
            WorkAsync(new WorkAsyncInfo("Doing the Shuffle...",
                (eventargs) =>
                {
                    shuffeling = true;
                    EnableShuffle();
                    var container = new CintContainer(new CrmServiceProxy(Service), "ShuffleRunner", true);
                    var log = container.Logger;
                    var location = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    var verinfo = FileVersionInfo.GetVersionInfo(location);
                    log.Log("  ***  {0} ***", verinfo.Comments.PadRight(50));
                    log.Log("  ***  {0} ***", verinfo.LegalCopyright.PadRight(50));
                    log.Log("  ***  {0} ***", (verinfo.InternalName + ", " + verinfo.FileVersion).PadRight(50));
                    var definition = new XmlDocument();
                    definition.Load(txtFile.Text);
                    ReplaceShufflePlaceholders(definition);
                    var definitionpath = Path.GetDirectoryName(txtFile.Text);
                    try
                    {
                        if (rbExport.Checked)
                        {
                            var export = Shuffler.QuickExport(definition, type, ';', ShuffleEventHandler, container, definitionpath);
                            if (export != null)
                            {
                                export.Save(txtData.Text);
                                AddLogText("Export saved to: " + txtData.Text);
                            }
                        }
                        else if (rbImport.Checked)
                        {
                            XmlDocument data = null;
                            if (datafilerequired)
                            {
                                AddLogText("Loading data from: " + txtData.Text);
                                data = ShuffleHelper.LoadDataFile(txtData.Text);
                            }
                            var importresult = Shuffler.QuickImport(definition, data, ShuffleEventHandler, container, definitionpath);
                            AddLogText("---");
                            AddLogText(string.Format("Created: {0}", importresult.Item1));
                            AddLogText(string.Format("Updated: {0}", importresult.Item2));
                            AddLogText(string.Format("Skipped: {0}", importresult.Item3));
                            AddLogText(string.Format("Deleted: {0}", importresult.Item4));
                            AddLogText(string.Format("Failed : {0}", importresult.Item5));
                            AddLogText("---");
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Log(ex);
                        throw;
                    }
                    finally
                    {
                        log.CloseLog();
                        shuffeling = false;
                        EnableShuffle();
                    }
                })
            {
                PostWorkCallBack =
                (completedeventargs) =>
                {
                    if (completedeventargs.Error != null)
                    {
                        AddLogText(completedeventargs.Error.Message);
                    }
                }
            });
        }

        private void txtFile_TextChanged(object sender, EventArgs e)
        {
            ExtractShufflePlaceholders();
            datafilerequired = ShuffleHelper.DataFileRequired(txtFile.Text);
            if (datafilerequired)
            {
                txtData.Text = Path.ChangeExtension(txtFile.Text, ".data.xml");
            }
            else
            {
                txtData.Text = "";
            }
            EnableShuffle();
        }

        private void rbExport_CheckedChanged(object sender, EventArgs e)
        {
            cmbType.Enabled = rbExport.Checked;
            EnableShuffle();
        }

        private void ExtractShufflePlaceholders()
        {
            var definition = new XmlDocument();
            definition.Load(txtFile.Text);
            var xml = definition.OuterXml;
            txtParams.Clear();
            var lParams = new List<string>();
            while (xml.Contains("{ShuffleVar:"))
            {
                var ph = xml.Substring(xml.IndexOf("{ShuffleVar:") + 12);
                ph = ph.Substring(0, ph.IndexOf('}'));
                lParams.Add(ph + "=");
                xml = xml.Replace("{ShuffleVar:" + ph, "");
            }
            if (lParams.Count > 0)
            {
                txtParams.Lines = lParams.ToArray();
            }
        }

        private void ReplaceShufflePlaceholders(XmlDocument definition)
        {
            string xml = definition.InnerXml;
            foreach (string arg in txtParams.Lines)
            {
                if (arg.IndexOf("=", StringComparison.OrdinalIgnoreCase) > 1)
                {
                    string name = "{ShuffleVar:" + arg.Substring(0, arg.IndexOf("=", StringComparison.OrdinalIgnoreCase)) + "}";
                    string value = arg.Substring(arg.IndexOf("=", StringComparison.OrdinalIgnoreCase) + 1);
                    if (xml.Contains(name))
                    {
                        xml = xml.Replace(name, value);
                        AddLogText(string.Format("Replacing \"{0}\" with \"{1}\"", name, value));
                    }
                }
                else
                {
                    throw new Exception("Parameter incorrectly formatted: " + arg);
                }
            }
            definition.InnerXml = xml;
        }

        private void ShuffleEventHandler(object sender, ShuffleEventArgs e)
        {
            AddLogText(e.Message, e.ReplaceLastMessage);
            UpdateProgressBars(e);
        }

        private void AddLogText(string text, bool replace = false)
        {
            if (text == null)
            {
                return;
            }
            if (text == "" || !string.IsNullOrWhiteSpace(text.Trim()))
            {
                MethodInvoker mi = delegate
                {
                    if (replace && lbLog.Items.Count > 0)
                    {
                        lbLog.Items[lbLog.Items.Count - 1] = text;
                    }
                    else
                    {
                        lbLog.Items.Add(text);
                    }
                    lbLog.SelectedIndex = this.lbLog.Items.Count - 1;
                };
                if (InvokeRequired) Invoke(mi); else mi();
            }
        }

        private void UpdateProgressBars(ShuffleEventArgs e)
        {
            if (e.Counters.Blocks >= 0 && e.Counters.BlockNo >= 0)
            {
                MethodInvoker mi = delegate
                {
                    pbBlocks.Maximum = e.Counters.Blocks;
                    pbBlocks.Value = e.Counters.BlockNo;
                };
                if (InvokeRequired) Invoke(mi); else mi();
            }
            if (e.Counters.Items >= 0 && e.Counters.ItemNo >= 0)
            {
                MethodInvoker mi = delegate
                {
                    pbRecords.Maximum = e.Counters.Items;
                    pbRecords.Value = e.Counters.ItemNo;
                };
                if (InvokeRequired) Invoke(mi); else mi();
            }
        }

        private void EnableShuffle()
        {
            MethodInvoker mi = delegate
            {
                var enabled = !shuffeling &&
                    Service != null &&
                    File.Exists(txtFile.Text) &&
                    ((rbExport.Checked && cmbType.SelectedIndex >= 0) ||
                     (rbImport.Checked && (!datafilerequired || File.Exists(txtData.Text))));
                btnShuffle.Enabled = enabled;
            };
            if (InvokeRequired) Invoke(mi); else mi();
        }

        private void txtData_TextChanged(object sender, EventArgs e)
        {
            EnableShuffle();
        }

        private void tsbCloseThisTab_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableShuffle();
        }

        public void OnIncomingMessage(MessageBusEventArgs message)
        {
            if (message.TargetArgument is string)
            {
                var definitionfile = (string)message.TargetArgument;
                txtFile.Text = definitionfile;
            }
        }

        public event EventHandler<MessageBusEventArgs> OnOutgoingMessage;
    }
}
