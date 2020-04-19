namespace Innofactor.Crm.CI.Cmdlets
{
    using Cinteros.Crm.Utils.Shuffle;
    using Innofactor.Crm.CI.Cmdlets.Structure;
    using System;
    using System.Management.Automation;
    using System.Xml;

    [Cmdlet(VerbsData.Import, "CrmShuffle")]
    [OutputType(typeof(ShuffleImportResult))]
    public class ImportCrmShuffle : XrmCmdletBase
    {
        #region Private Fields

        private readonly ProgressRecord progress = new ProgressRecord(0, "Shuffle", "Idle");

        #endregion Private Fields

        #region Public Properties

        [Parameter(
            Mandatory = false,
            HelpMessage = "Provide Shuffle CSV-data, as a string"
        )]
        public string DataCsv { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "Provide Shuffle XML-data, as an XMLDocument"
        )]
        public XmlDocument DataXml { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "Provide a valid Shuffle definition, as an XMLDocument"
        ), Alias("Def", "D")]
        public XmlDocument Definition { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "Working folder for relative paths, solution zips etc."
        ), Alias("F")]
        public string Folder { get; set; }

        #endregion Public Properties

        #region Protected Methods

        protected override void ProcessRecord()
        {
            var Data = DataXml;
            if (Data == null && DataCsv != null)
            {
                try
                {
                    WriteVerbose("Xmlifying CSV data");
                    Data = ShuffleHelper.StringToXmlSerialized(DataCsv);
                }
                catch (Exception ex)
                {
                    WriteError(new ErrorRecord(ex, "StringToXmlSerialized", ErrorCategory.ReadError, Definition));
                }
            }
            try
            {
                WriteDebug("Importing");
                var result = Shuffler.QuickImport(new ShuffleContainer(this), Definition, Data, ShuffleListener, Folder, true);
                var output = new ShuffleImportResult
                {
                    Created = result.created,
                    Updated = result.updated,
                    Skipped = result.skipped,
                    Deleted = result.deleted,
                    Failed = result.failed
                };
                WriteObject(output);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "QuickImport", ErrorCategory.ReadError, Definition));
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private void ShuffleListener(object sender, ShuffleEventArgs e)
        {
            if (e.Counters != null && !string.IsNullOrWhiteSpace(e.Counters.Block))
            {
                progress.StatusDescription = $"Importing {e.Counters.Block}";
            }
            if (e.Counters.Blocks > 0)
            {
                progress.PercentComplete = (e.Counters.BlockNo * 100) / e.Counters.Blocks;
            }
            WriteProgress(progress);
            if (!string.IsNullOrWhiteSpace(e.Message))
            {
                WriteVerbose(e.Message);
            }
        }

        #endregion Private Methods
    }
}