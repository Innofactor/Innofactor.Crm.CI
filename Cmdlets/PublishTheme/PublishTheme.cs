using System;
using Microsoft.Xrm.Sdk;

namespace Innofactor.Crm.CI.Cmdlets.Cmdlets
{
    using Microsoft.Crm.Sdk.Messages;
    using System.Management.Automation;

    [Cmdlet(VerbsData.Publish, "Theme")]
    public class PublishTheme : XrmCmdletBase
    {

        #region Protected properties
        [Parameter(
            Mandatory = true,
            HelpMessage = "ID of the theme to publish."
        )]
        public string ThemeId { get; set; }
        #endregion Protected properties

        #region Protected Methods

        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();

                Guid theme = Guid.Empty;
                if (string.IsNullOrWhiteSpace(ThemeId))
                {
                    // Hämta aktivt tema från CRM?
                    throw new Exception("Theme ID not provided.");
                }
                if (!Guid.TryParse(ThemeId, out theme))
                {
                    WriteInformation(new InformationRecord($"Could not parse ThemeId {ThemeId} as a GUID.", "PublishThemeCmdlet"));
                    throw new Exception($"Could not parse ThemeId {ThemeId} as a GUID.");
                }

                WriteVerbose($"Publishing theme '{theme}'.");

                var request = new PublishThemeRequest();
                request.Target = new EntityReference("theme", theme);
                Service.Execute(request);
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(e, "Error in PublishThemeCmdlet", ErrorCategory.NotSpecified, ""));
            }
        }

        #endregion Protected Methods
    }
}