// PROJECT : MsCrmTools.SiteMapEditor
// This project was developed by Tanguy Touzard
// CODEPLEX: http://xrmtoolbox.codeplex.com
// BLOG: http://mscrmtools.blogspot.com

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Innofactor.Crm.Shuffle.Builder.Forms
{
    public partial class XmlContentDisplayDialog : Form
    {
        public XmlContentDisplayDialog(string xmlString)
        {
            InitializeComponent();

            var indentedXml = IndentXMLString(xmlString);
            txtContent.Text = indentedXml;
        }

        private string IndentXMLString(string xml)
        {
            var ms = new MemoryStream();
            var xtw = new XmlTextWriter(ms, Encoding.Unicode);
            var doc = new XmlDocument();

            try
            {
                doc.LoadXml(xml);

                xtw.Formatting = Formatting.Indented;
                doc.WriteContentTo(xtw);
                xtw.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                var sr = new StreamReader(ms);
                return sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }
    }
}
