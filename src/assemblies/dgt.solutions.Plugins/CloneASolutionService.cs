using D365.Extension.Core;
using D365.Extension.Model;
using D365.Extension.Registration;
using Microsoft.Crm.Sdk.Messages;

namespace dgt.solutions.Plugins
{
    [CustomApiRegistration(SdkMessageNames.DgtCloneASolution)]
    public class CloneASolutionService : Executor
    {
        protected override ExecutionResult Execute()
        {
            GetInputParameter("UniqueName", out string uniqueName);
            GetInputParameter("FriendlyName", out string friendlyName);
            GetInputParameter("Version", out string version);
            var request = new CloneAsSolutionRequest
            {
                ParentSolutionUniqueName = uniqueName,
                DisplayName = friendlyName,
                VersionNumber = version
            };
            //clone as patch execute and response
            var response = (CloneAsSolutionResponse)ElevatedOrganizationService.Execute(request);
            SetOutputParameter("SolutionId", response.SolutionId.ToString("D"));
            return ExecutionResult.Ok;
        }
    }
}
