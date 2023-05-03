using D365.Extension.Core;
using D365.Extension.Model;
using D365.Extension.Registration;
using dgt.solutions.Plugins.Helper;
using Microsoft.Xrm.Sdk.Query;

namespace dgt.solutions.Plugins
{
    [CustomApiRegistration(SdkMessageNames.DgtUpdateLinkedCarrierSolution)]
    public class UpdateLinkedCarrierSolutionService : Executor
    {
        protected override ExecutionResult Execute()
        {
            var carrier = ElevatedOrganizationService.Retrieve(
                DgtCarrier.EntityLogicalName,
                Delegate.PluginExecutionContext.PrimaryEntityId, new ColumnSet(
                    DgtCarrier.LogicalNames.DgtSolutionuniquename
                )).ToEntity<DgtCarrier>();
            var solution = new SolutionLookup(this).GetByName(carrier.DgtSolutionuniquename);
            ElevatedOrganizationService.Update(new DgtCarrier(Delegate.PluginExecutionContext.PrimaryEntityId)
            {
                DgtSolutionfriendlyname = solution.FriendlyName,
                DgtSolutionversion = solution.Version
            });
            return ExecutionResult.Ok;
        }

        public override string GetConfig(string key, int lcid = 1033, string defaultValue = null)
        {
            return null;
        }
    }
}
