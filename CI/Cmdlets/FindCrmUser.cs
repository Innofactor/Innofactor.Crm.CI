namespace Cinteros.Crm.Utils.CI.Cmdlets
{
    using Cinteros.Crm.Utils.CI.Cmdlets.Structure;
    using Microsoft.Crm.Sdk.Messages;
    using System.Management.Automation;

    [Cmdlet(VerbsCommon.Find, "CrmUser")]
    [OutputType(typeof(WhoAmIResponse))]
    public class FindCrmUser : XrmCommandBase
    {
        #region Protected Methods

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            WriteVerbose(string.Format("Invoking Organization Service"));

            var response = (Container.Service.Execute(new WhoAmIRequest()) as WhoAmIResponse);

            WriteObject(response);

            WriteVerbose(string.Format("OrganizationId: {0}", response.OrganizationId));
            WriteVerbose(string.Format("BusinessUnitId: {0}", response.BusinessUnitId));
            WriteVerbose(string.Format("UserId: {0}", response.UserId));
        }

        #endregion Protected Methods
    }
}