using Cinteros.Crm.Utils.Common;
using Cinteros.Crm.Utils.Shuffle;
using Cinteros.Crm.Utils.Shuffle.Types;
using System;
using System.Management.Automation;
using System.Xml;

namespace Cinteros.Crm.Utils.CI.Cmdlets
{
    [Cmdlet(VerbsData.Export, "CrmShuffle")]
    [OutputType(typeof(XmlDocument))]
    public class ExportCrmShuffleCmdlet : XrmCommandBase
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "Provide a valid Shuffle definition, as an XMLDocument"
        )]
        [Alias("Def", "D")]
        public XmlDocument Definition { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 1,
            HelpMessage = "Provide the type of Shuffle export: Full, Simple, SimpleWithValue, SimpleNoId, Explicit or Text"
        )]
        [Alias("T")]
        public SerializationType Type { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "Working folder for relative paths, solution zips etc."
        )]
        [Alias("F")]
        public string Folder { get; set; }

        private readonly ProgressRecord progress = new ProgressRecord(0, "Shuffle", "Idle");

        protected override void ProcessRecord()
        {
            var svc = Container.Service as CrmServiceProxy;
            var log = Container.Logger as CRMLogger;
            try
            {
                WriteDebug("Exporting");
                var doc = Shuffler.QuickExport(Definition, Type, ';', ShuffleListener, svc, log, Folder, true);
                WriteObject(doc);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "QuickExport", ErrorCategory.ReadError, Definition));
            }
        }

        private void ShuffleListener(object sender, ShuffleEventArgs e)
        {
            if (e.Counters != null && !string.IsNullOrWhiteSpace(e.Counters.Block))
            {
                progress.StatusDescription = $"Exporting {e.Counters.Block}";
            }
            else if (!string.IsNullOrWhiteSpace(e.Message))
            {
                progress.StatusDescription = e.Message;
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
    }
}
