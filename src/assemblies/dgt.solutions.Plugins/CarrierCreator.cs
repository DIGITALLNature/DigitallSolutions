using D365.Extension.Core;
using D365.Extension.Model;
using D365.Extension.Registration;
using dgt.solutions.Plugins.Helper;

namespace dgt.solutions.Plugins
{
    [PluginRegistration(
        PluginExecutionMode.Synchronous,
        SdkMessageNames.Create,
        PluginExecutionStage.PreOperation,
        PrimaryEntityName = DgtCarrier.EntityLogicalName
    )]
    public class CarrierCreator : Executor
    {
        protected override ExecutionResult Execute()
        {
            var carrier = Entity.ToEntity<DgtCarrier>();
            var solution = new SolutionLookup(this).GetByName(carrier.DgtSolutionuniquename);
            carrier.DgtReference = solution.FriendlyName;
            carrier.DgtSolutionfriendlyname = solution.FriendlyName;
            carrier.DgtSolutionuniquename = solution.UniqueName;
            carrier.DgtSolutionversion = solution.Version;
            carrier.DgtSolutionid = solution.Id.ToString("D");
            return ExecutionResult.Ok;
        }
    }
}
