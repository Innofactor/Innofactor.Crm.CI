using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Innofactor.Crm.ShuffleDeployer
{
    public class Package
    {
        public List<Module> Modules { get; set; }

        [XmlAttribute]
        public string FileOrigin { get; set; }

        [XmlAttribute]
        public DateTime FileCreated { get; set; }

        public Package()
        {
            Modules = new List<Module>();
        }

        public Package(ListBox.ObjectCollection items) : this()
        {
            foreach (var item in items)
            {
                if (item is Module)
                {
                    Modules.Add((Module)item);
                }
            }
        }
    }
}