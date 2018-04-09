using Cinteros.Crm.Utils.Common;
using Cinteros.Crm.Utils.Common.Interfaces;
using Cinteros.Crm.Utils.Misc;
using Cinteros.Crm.Utils.Shuffle.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Cinteros.Crm.Utils.Shuffle
{
    /// <summary>Generic helper methods for Cinteros Shuffle</summary>
    public static class ShuffleHelper
    {
        /// <summary>
        /// Extracts delimeted text block from XML document format
        /// </summary>
        /// <param name="xml">XML document containing the text block</param>
        /// <returns>Block with text-serialized CRM data</returns>
        public static string XmlSerializedToTextFile(XmlDocument xml)
        {
            XmlNode root = CintXML.FindChild(xml, "ShuffleData");
            string sertype = CintXML.GetAttribute(root, "Type");
            if (sertype == SerializationType.Text.ToString())
            {
                XmlNode xText = CintXML.FindChild(root, "Text");
                string text = xText.InnerText;
                return text;
            }
            else
            {
                throw new ArgumentOutOfRangeException("xml", sertype, "XmlSerializedToTextFile called with xml document of incompatible serialization type");
            }
        }

        /// <summary>Helper method to embed any type of serialized data into XmlDocument for import into CRM</summary>
        /// <param name="SerializedData"></param>
        /// <returns></returns>
        public static XmlDocument StringToXmlSerialized(string SerializedData)
        {
            return StringToXmlSerialized(SerializedData, '\0');
        }

        /// <summary>Helper method to embed any type of serialized data into XmlDocument for import into CRM</summary>
        /// <param name="SerializedData"></param>
        /// <param name="delimeter"></param>
        /// <returns></returns>
        public static XmlDocument StringToXmlSerialized(string SerializedData, char delimeter)
        {
            XmlDocument data = new XmlDocument();
            try
            {
                data.LoadXml(SerializedData);
            }
            catch (XmlException)
            {
                data = TextFileToXmlSerialized(SerializedData, delimeter);
            }
            return data;
        }

        /// <summary>Helper method to embed delimeted text file into XmlDocument for import into CRM</summary>
        /// <param name="text"></param>
        /// <param name="delimeter"></param>
        /// <returns></returns>
        private static XmlDocument TextFileToXmlSerialized(string text, char delimeter)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentNullException("text", "Data string is empty, cannot compose serialized block");
            }
            if (delimeter == '\0')
            {
                StringReader reader = new StringReader(text);
                if (reader.ReadLine() != null)  // Första raden innehöll nåt
                {
                    string cols = reader.ReadLine();    // Läs andra raden
                    if (cols != null && cols.StartsWith("Entity") && cols.Length > 6)
                    {
                        delimeter = cols[6];
                    }
                }
            }
            if (delimeter == '\0')
            {
                throw new ArgumentNullException("delimeter", "Cannot parse delimeter from data string");
            }
            XmlDocument xml = new XmlDocument();
            XmlNode root = xml.CreateElement("ShuffleData");
            xml.AppendChild(root);
            CintXML.AppendAttribute(root, "Type", SerializationType.Text.ToString());
            CintXML.AppendAttribute(root, "Delimeter", delimeter.ToString());
            CintXML.AddCDATANode(root, "Text", text);
            return xml;
        }

        /// <summary>Deserializes ShuffleDefinition XML to ShuffleDefinition class.</summary>
        /// <param name="definitionfile"></param>
        /// <param name="clearVariables">Set this to remove any {ShuffleVar:xxx} variables in the definition file</param>
        /// <returns></returns>
        public static ShuffleDefinition GetShuffleDefinition(string definitionfile, bool clearVariables)
        {
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(ShuffleDefinition));
                TextReader textReader = new StreamReader(definitionfile);
                var content = textReader.ReadToEnd();
                content = VerifyShuffleVars(content, clearVariables);
                textReader = new StringReader(content);
                var result = (ShuffleDefinition)deserializer.Deserialize(textReader);
                textReader.Close();
                return result;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        /// <summary>Returns a list of file names that are required for the given ShuffleDefinition</summary>
        /// <param name="shuffleDefinition">ShuffleDefinition file</param>
        /// <param name="definitionpath"></param>
        /// <returns>List of files</returns>
        public static List<string> GetReferencedFiles(string shuffleDefinition, string definitionpath, CRMLogger log)
        {
            log.StartSection(MethodBase.GetCurrentMethod().Name);
            var result = new List<string>();
            if (File.Exists(shuffleDefinition))
            {
                var definition = GetShuffleDefinition(shuffleDefinition, true);
                if (DataFileRequired(definition))
                {
                    var datafile = Path.ChangeExtension(shuffleDefinition, ".data.xml");
                    log.Log("Adding data file: {0}", datafile);
                    result.Add(datafile);
                }
                foreach (var solBlock in definition.Blocks.Items.Where(b => b is SolutionBlock))
                {
                    var solFile = GetSolutionFilename((SolutionBlock)solBlock, definitionpath);
                    if (!result.Contains(solFile))
                    {
                        log.Log("Adding solution file: {0}", solFile);
                        result.Add(solFile);
                    }
                }
            }
            else
            {
                log.Log("Definition file not found");
            }
            log.Log("Returning {0} files", result.Count);
            log.EndSection();
            return result;
        }

        private static string GetSolutionFilename(SolutionBlock solBlock, string definitionpath)
        {
            string file = solBlock.File;
            if (string.IsNullOrWhiteSpace(file))
            {
                file = solBlock.Name;
            }
            string path = solBlock.Path;
            if (string.IsNullOrWhiteSpace(path) && !string.IsNullOrWhiteSpace(definitionpath))
            {
                path = definitionpath;
            }
            path += path.EndsWith("\\") ? "" : "\\";
            string filename;
            if (solBlock.Import.Type == SolutionTypes.Managed)
            {
                filename = path + file + "_managed.zip";
            }
            else if (solBlock.Import.Type == SolutionTypes.Unmanaged)
            {
                filename = path + file + ".zip";
            }
            else
            {
                throw new ArgumentOutOfRangeException("Type", solBlock.Import.Type, "Invalid Solution type");
            }

            if (filename.Contains("%"))
            {
                IDictionary envvars = Environment.GetEnvironmentVariables();
                foreach (DictionaryEntry de in envvars)
                {
                    filename = filename.Replace("%" + de.Key.ToString() + "%", de.Value.ToString());
                }
            }
            return filename;
        }

        /// <summary>Checks if given ShuffleDefinition contains data blocks, and thus requires data file to be used.</summary>
        /// <param name="definitionFile"></param>
        /// <returns></returns>
        public static bool DataFileRequired(string definitionFile)
        {
            if (!File.Exists(definitionFile))
            {
                return false;
            }
            var definition = GetShuffleDefinition(definitionFile, true);
            return DataFileRequired(definition);
        }

        private static bool DataFileRequired(ShuffleDefinition definition)
        {
            return definition != null && definition.Blocks != null && definition.Blocks.Items != null && definition.Blocks.Items.Any(b => b is DataBlock);
        }

        /// <summary></summary>
        /// <param name="dataFile"></param>
        /// <returns></returns>
        public static XmlDocument LoadDataFile(string dataFile)
        {
            XmlDocument data = new XmlDocument();
            try
            {
                data.Load(dataFile);
            }
            catch (XmlException)
            {
                using (StreamReader sr = new StreamReader(dataFile))
                {
                    String textfile = sr.ReadToEnd();
                    data = StringToXmlSerialized(textfile);
                }
            }
            catch (FileNotFoundException)
            {
                data = null;
            }
            return data;
        }

        /// <summary>Validates given ShuffleDefinition with XSD.</summary>
        /// <param name="def"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static string ValidateDefinitionXml(XmlDocument def, ILoggable log)
        {
            var result = "";
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                var shufdefresource = assembly.GetManifestResourceNames().Where(n => n.ToLowerInvariant().EndsWith("shuffledefinition.xsd")).FirstOrDefault();
                var qryexpresource = assembly.GetManifestResourceNames().Where(n => n.ToLowerInvariant().EndsWith("queryexpression.xsd")).FirstOrDefault();

                Stream stream = assembly.GetManifestResourceStream(shufdefresource);
                if (stream == null)
                {
                    result = "Cannot find resource " + shufdefresource;
                }
                else
                {
                    def.Schemas = new XmlSchemaSet();
                    def.Schemas.Add(null, XmlReader.Create(stream));
                    stream = assembly.GetManifestResourceStream(qryexpresource);
                    if (stream == null)
                    {
                        result = "Cannot find resource " + qryexpresource;
                    }
                    else
                    {
                        def.Schemas.Add(null, XmlReader.Create(stream));
                        def.Validate(null);
                        log.Log("ShuffleDefinition validated");
                    }
                }
            }
            catch (XmlSchemaValidationException ex)
            {
                if (log != null)
                {
                    log.Log(ex);
                }

                throw;
            }
            return result;
        }

        /// <summary>Verifies that no {ShuffleVar:xxx} exist in the definition</summary>
        /// <param name="definition"></param>
        /// <param name="clearExisting">Set this to first try to remove all existing variables</param>
        /// <returns></returns>
        public static string VerifyShuffleVars(string definition, bool clearExisting)
        {
            if (clearExisting)
            {
                while (definition.Contains("{ShuffleVar:"))
                {
                    var variable = definition.Substring(definition.IndexOf("{ShuffleVar:", StringComparison.InvariantCulture));
                    variable = variable.Split('}')[0] + "}";
                    definition = definition.Replace(variable, "");
                }
            }
            if (definition.Contains("{ShuffleVar:"))
            {
                var variable = definition.Substring(definition.IndexOf("{ShuffleVar:", StringComparison.InvariantCulture) + 12);
                variable = variable.Split('}')[0];
                throw new Exception("Missing replacement value for variable: " + variable);
            }
            return definition;
        }

        /// <summary>Verifies that no {ShuffleVar:xxx} exist in the definition</summary>
        /// <param name="xDefinitionDoc"></param>
        /// <param name="clearExisting">Set this to first try to remove all existing variables</param>
        public static void VerifyShuffleVars(XmlDocument xDefinitionDoc, bool clearExisting)
        {
            string xml = xDefinitionDoc.InnerXml;
            xml = VerifyShuffleVars(xml, clearExisting);
            xDefinitionDoc.InnerXml = xml;
        }
    }
}
