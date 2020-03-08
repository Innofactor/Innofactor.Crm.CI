namespace Cinteros.Crm.Utils.Shuffle
{
    using Cinteros.Crm.Utils.Common.Interfaces;
    using Cinteros.Crm.Utils.Shuffle.Types;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;

    public class ShuffleDataSplitter
    {

        #region Private Fields

        private readonly ILoggable log;
        private readonly IContainable container;

        #endregion Private Fields


        public ShuffleDataSplitter(IContainable container)
        {
            this.container = container;
            log = container.Logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blocks"></param>
        /// <param name="type"></param>
        /// <param name="delimeter"></param>
        /// <returns></returns>
        public Dictionary<string, XmlDocument> SplitFiles(ShuffleBlocks blocks, SerializationType type, char delimeter)
        {
            log.StartSection("Start SplitFiles");

            var shuffle = new Shuffler(container);
            var dictionarySplitFiles = new Dictionary<string, XmlDocument>();

            if (blocks.Count > 0)
            {
                switch (type)
                {
                    case SerializationType.Full:
                    case SerializationType.Simple:
                    case SerializationType.SimpleWithValue:
                    case SerializationType.SimpleNoId:
                    case SerializationType.Explicit:
                        foreach (var blockName in blocks.Keys)
                        {
                            var block = blocks[blockName];

                            foreach (var item in block)
                            {
                                //Build path to use later when writing to disk
                                string path = block.Entity;
                                path += "\\" + item.Id;

                                // Somehow create a single entity record shuffleBlock
                                var singleShuffleBlock = new ShuffleBlocks();
                                var entityCollection = new Common.CintDynEntityCollection();
                                singleShuffleBlock.Add(blockName, entityCollection);

                                entityCollection.Add(item);

                                dictionarySplitFiles.Add(path, shuffle.Serialize(singleShuffleBlock, type, delimeter));
                            }


                            //SendLine("Serializing {0} records in block {1}", blocks[block].Count, block);
                            //XmlNode xBlock = xml.CreateElement("Block");
                            //root.AppendChild(xBlock);
                            //CintXML.AppendAttribute(xBlock, "Name", block);
                            //CintXML.AppendAttribute(xBlock, "Count", blocks[block].Count.ToString());
                            //var xSerialized = blocks[block].Serialize((SerializationStyle)type);
                            //xBlock.AppendChild(xml.ImportNode(xSerialized.ChildNodes[0], true));
                        }
                        break;

                        //case SerializationType.Text:
                        //    CintXML.AppendAttribute(root, "Delimeter", delimeter.ToString());
                        //    var text = new StringBuilder();
                        //    foreach (var block in blocks.Keys)
                        //    {
                        //        SendLine("Serializing {0} records in block {1}", blocks[block].Count, block);
                        //        text.AppendLine("<<<" + block + ">>>");
                        //        var serializedblock = blocks[block].ToTextFile(delimeter);
                        //        text.Append(serializedblock);
                        //    }
                        //    CintXML.AddCDATANode(root, "Text", text.ToString());
                        //    break;
                }



            }
            log.EndSection();
            return dictionarySplitFiles;
        }
    }
}