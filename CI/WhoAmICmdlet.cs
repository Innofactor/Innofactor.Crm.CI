using Microsoft.Crm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Cinteros.Crm.Utils.CI.Cmdlets
{
    [Cmdlet(VerbsCommon.Select, "WhoAmI")]
    [OutputType(typeof(WhoAmIResponse))]
    public class WhoAmICmdlet : XrmCommandBase
    {
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

    }
}
