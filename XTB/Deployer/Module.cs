using System;
using System.Xml.Serialization;

namespace Innofactor.Crm.ShuffleDeployer
{
    [Serializable]
    public class Module
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public ModuleType Type { get; set; }

        [XmlAttribute]
        public string Description { get; set; }

        [XmlAttribute]
        public string File { get; set; }

        [XmlAttribute]
        public string DataFile { get; set; }

        [XmlAttribute]
        public bool Default { get; set; }

        [XmlAttribute]
        public bool Optional { get; set; }

        //public List<Module> SubModules { get; set; }

        public Module() : this("undefined")
        {
        }

        public Module(string name) : this(name, ModuleType.ShuffleDefinition, "", true, true)
        {
        }

        public Module(string name, ModuleType type, string file, bool def, bool optional)
        {
            Name = name;
            Type = type;
            File = file;
            Default = def || !optional;
            Optional = optional;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public enum ModuleType
    {
        ShuffleDefinition,
        //SQLscript
        //JSON perhaps one day!
    }
}