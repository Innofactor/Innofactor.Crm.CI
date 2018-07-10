using Cinteros.Crm.Utils.Shuffle;
using Innofactor.Crm.Shuffle.Builder.AppCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Innofactor.Crm.Shuffle.Builder.Controls
{
    public partial class ControlBase : UserControl
    {
        #region Internal Fields

        internal ShuffleBuilder shuffleBuilder;

        #endregion Internal Fields

        #region Private Fields

        private const int controlposition = 150;
        private readonly Dictionary<string, string> attributeCollection;
        private string controlsCheckSum = "";

        #endregion Private Fields

        #region Public Constructors

        public ControlBase()
        {
            InitializeComponent();
        }

        public ControlBase(Dictionary<string, string> collection, ShuffleBuilder shuffleBuilder)
            : this(string.Empty, collection, shuffleBuilder) { }

        public ControlBase(string nodename, Dictionary<string, string> collection, ShuffleBuilder shuffleBuilder)
            : this()
        {
            this.shuffleBuilder = shuffleBuilder;
            if (collection != null)
            {
                attributeCollection = collection;
            }
            else
            {
                attributeCollection = new Dictionary<string, string>();
            }
            Saved += shuffleBuilder.CtrlSaved;
            if (string.IsNullOrEmpty(nodename))
            {
                nodename = GetCallingNodeName();
            }
            txtInfo.Text = ShuffleHelper.GetNodeDocumentation(nodename);
            gbInfo.Visible = !string.IsNullOrEmpty(txtInfo.Text);
            LayoutControls();
        }

        private string GetCallingNodeName()
        {
            var nodename = Tag?.ToString();
            if (string.IsNullOrEmpty(nodename))
            {
                StackFrame[] stackFrames = new StackTrace(true).GetFrames();
                MethodBase mb = null;
                foreach (StackFrame stackFrame in stackFrames)
                {
                    var tmpmb = stackFrame.GetMethod();
                    if (!tmpmb.ReflectedType.FullName.ToLowerInvariant().EndsWith("controlbase"))
                    {
                        mb = tmpmb;
                        break;
                    }
                }
                if (mb != null)
                {
                    nodename = mb.ReflectedType.FullName;
                    nodename = nodename.Split('.').LastOrDefault();
                    nodename = nodename.Replace("Control", string.Empty);
                }
            }
            return nodename;
        }

        #endregion Public Constructors

        #region Public Delegates

        public delegate void SaveEventHandler(object sender, SaveEventArgs e);

        #endregion Public Delegates

        #region Public Events

        public event SaveEventHandler Saved;

        #endregion Public Events

        #region Public Methods

        public bool InitializationNeeded(ControlCollection ntrols)
        {
            return Controls == null || Controls.Count == 0 || !Controls.Cast<Control>().Any(c => c != gbInfo);
        }

        public virtual ControlCollection GetControls()
        {
            return Controls;
        }

        public virtual void PopulateControls()
        {
            // Nothing in base class, but possible to override
            FillControls();
        }

        public virtual void Save()
        {
            try
            {
                var collection = GetAttributesCollection(true);
                SendSaveMessage(collection);
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            controlsCheckSum = ControlsChecksum();
        }

        #endregion Public Methods

        #region Private Methods

        private Dictionary<string, string> GetAttributesCollection(bool validate = false)
        {
            Dictionary<string, string> collection = new Dictionary<string, string>();

            foreach (Control control in GetControls().Cast<Control>().Where(y => y.Tag != null).OrderBy(y => y.TabIndex))
            {
                if (GetControlDefinition(control, out string attribute, out bool required, out string defaultvalue))
                {
                    var value = GetValueFromControl(control);
                    if (validate && required && string.IsNullOrEmpty(value))
                    {
                        throw new ArgumentNullException(attribute, "Field cannot be empty");
                    }
                    if (required || value != defaultvalue)
                    {
                        collection.Add(attribute, value);
                    }
                }
            }
            return collection;
        }

        private static bool GetControlDefinition(Control control, out string attribute, out bool required, out string defaultvalue)
        {
            var tags = control.Tag != null ? control.Tag.ToString().Split('|') : new string[] { };
            attribute = tags.Length > 0 ? tags[0] : "";
            required = tags.Length > 1 ? bool.Parse(tags[1]) : false;
            defaultvalue = tags.Length > 2 ? tags[2] : control is CheckBox ? "false" : "";
            return !string.IsNullOrWhiteSpace(attribute);
        }

        private static string GetValueFromControl(Control control)
        {
            var result = "";
            if (control is CheckBox)
            {
                result = ((CheckBox)control).Checked ? "true" : "false";
            }
            else if (control is TextBox)
            {
                result = ((TextBox)control).Text;
            }
            else if (control is RichTextBox)
            {
                result = ((RichTextBox)control).Text;
            }
            else if (control is ComboBox)
            {
                var item = ((ComboBox)control).SelectedItem;
                if (item is IComboBoxItem)
                {
                    result = ((IComboBoxItem)item).GetValue();
                }
                else
                {
                    result = ((ComboBox)control).Text;
                }
            }
            return result.Trim();
        }

        private void ControlBase_Leave(object sender, EventArgs e)
        {
            if (controlsCheckSum != ControlsChecksum())
            {
                Save();
            }
        }

        private void ControlBase_Load(object sender, EventArgs e)
        {
            PopulateControls();
        }

        private string ControlsChecksum()
        {
            var checksum = "";
            foreach (Control control in GetControls())
            {
                if (control.Tag == null) { continue; }
                checksum += GetValueFromControl(control) + "|";
            }
            return checksum;
        }

        private void FillControl(Control control)
        {
            string attribute;
            bool required;
            string defaultvalue;
            if (GetControlDefinition(control, out attribute, out required, out defaultvalue))
            {
                var value = attributeCollection.ContainsKey(attribute) ? attributeCollection[attribute] : defaultvalue;
                if (control is CheckBox)
                {
                    bool.TryParse(value, out bool chk);
                    ((CheckBox)control).Checked = chk;
                }
                else if (control is TextBox)
                {
                    ((TextBox)control).Text = value;
                }
                else if (control is RichTextBox)
                {
                    ((RichTextBox)control).Text = value;
                }
                else if (control is ComboBox cmb)
                {
                    object selitem = null;
                    foreach (var item in cmb.Items)
                    {
                        if (item is IComboBoxItem)
                        {
                            if (((IComboBoxItem)item).GetValue() == value)
                            {
                                selitem = item;
                                break;
                            }
                        }
                    }
                    if (selitem != null)
                    {
                        cmb.SelectedItem = selitem;
                    }
                    else if (cmb.Items.IndexOf(value) >= 0)
                    {
                        cmb.SelectedItem = value;
                    }
                    else
                    {
                        cmb.Text = value;
                    }
                }
            }
        }

        private void FillControls()
        {
            foreach (Control control in GetControls().Cast<Control>().Where(y => y.Tag != null).OrderBy(y => y.TabIndex))
            {
                FillControl(control);
            }
        }

        private void LayoutControls()
        {
            foreach (Control control in GetControls()
                .Cast<Control>()
                .Where(y => y.Tag != null && y.Left > 50)
                .OrderBy(y => y.TabIndex))
            {
                var rightmargin = control.Parent.Width - control.Left - control.Width;
                control.Left = controlposition;
                if (control.Anchor.HasFlag(AnchorStyles.Right))
                {
                    control.Width = control.Parent.Width - control.Left - rightmargin;
                }
            }
        }

        private void SendSaveMessage(Dictionary<string, string> collection)
        {
            Saved?.Invoke(this, new SaveEventArgs { AttributeCollection = collection });
        }

        #endregion Private Methods
    }
}