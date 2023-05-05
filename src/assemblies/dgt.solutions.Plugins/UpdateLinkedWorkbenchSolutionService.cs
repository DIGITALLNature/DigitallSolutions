using D365.Extension.Core;
using D365.Extension.Model;
using D365.Extension.Registration;
using dgt.solutions.Plugins.Helper;
using Microsoft.Xrm.Sdk.Query;

namespace dgt.solutions.Plugins
{
    [CustomApiRegistration(SdkMessageNames.DgtUpdateLinkedWorkbenchSolution)]
    public class UpdateLinkedWorkbenchSolutionService : Executor
    {
        protected override ExecutionResult Execute()
        {
            var workbench = ElevatedOrganizationService.Retrieve(
                DgtWorkbench.EntityLogicalName,
                 Delegate.PluginExecutionContext.PrimaryEntityId, new ColumnSet(
                    DgtWorkbench.LogicalNames.DgtSolutionuniquename
                 )).ToEntity<DgtWorkbench>();
            var solution = new SolutionLookup(this).GetByName(workbench.DgtSolutionuniquename);
            ElevatedOrganizationService.Update(new DgtWorkbench(Delegate.PluginExecutionContext.PrimaryEntityId)
            {
                DgtSolutionfriendlyname = solution.FriendlyName,
                DgtSolutionversion = solution.Version
            });
            return ExecutionResult.Ok;
        }
    }
}
