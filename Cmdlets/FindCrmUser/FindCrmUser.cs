namespace Innofactor.Crm.CI.Cmdlets
{
    using Microsoft.Crm.Sdk.Messages;
    using System.Management.Automation;

    [Cmdlet(VerbsCommon.Find, "CrmUser")]
    [OutputType(typeof(WhoAmIResponse))]
    public class FindCrmUser : XrmCmdletBase
    {
        #region Protected Methods

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            WriteVerbose(string.Format("Invoking Organization Service"));

            var response = (Service.Execute(new WhoAmIRequest()) as WhoAmIResponse);

            WriteObject(response);

            WriteVerbose($"OrganizationId: {response.OrganizationId}");
            WriteVerbose($"BusinessUnitId: {response.BusinessUnitId}");
            WriteVerbose($"UserId: {response.UserId}");
        }

        #endregion Protected Methods
    }
}