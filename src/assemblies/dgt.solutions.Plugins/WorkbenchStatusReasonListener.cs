using D365.Extension.Core;
using D365.Extension.Model;
using D365.Extension.Registration;
using dgt.solutions.Plugins.Processor;

namespace dgt.solutions.Plugins
{
    [PluginRegistration(
        PluginExecutionMode.Asynchronous,
        SdkMessageNames.Update,
        PluginExecutionStage.PostOperation,
        PrimaryEntityName = DgtWorkbench.EntityLogicalName,
        FilterAttributes = new[]
        {
            DgtWorkbench.LogicalNames.Statuscode
        },
        PreEntityImage = true,
        PreEntityImageAttributes = new[]
        {
            DgtWorkbench.LogicalNames.DgtSolutionid,
            DgtWorkbench.LogicalNames.DgtSolutionuniquename,
            DgtWorkbench.LogicalNames.DgtSolutionversion,
            DgtWorkbench.LogicalNames.DgtTargetCarrierId
        }
    )]
    public class WorkbenchStatusReasonListener : Executor
    {
        protected override ExecutionResult Execute()
        {
            var workbench = this.MergeEntity<DgtWorkbench>();
            switch (Entity.ToEntity<DgtWorkbench>().Statuscode.Value)
            {
                case DgtWorkbench.Options.Statuscode.Merge:
                    return new ComponentMoveProcessor(this).Init("Merge", DgtWorkbench.Options.Statecode.Active, DgtWorkbench.Options.Statuscode.Active).Execute(workbench);
                case DgtWorkbench.Options.Statuscode.Finalize:
                    return new ComponentMoveProcessor(this).Init("Finalize", DgtWorkbench.Options.Statecode.Inactive, DgtWorkbench.Options.Statuscode.Inactive).Execute(workbench);
                case DgtWorkbench.Options.Statuscode.Close:
                    return new CloseProcessor(this).Init("Close", DgtWorkbench.Options.Statecode.Inactive, DgtWorkbench.Options.Statuscode.Inactive).Execute(workbench);
                case DgtWorkbench.Options.Statuscode.Inactive:
                    return new FinalizeProcessor(this).Execute(workbench);
                default:
                    return ExecutionResult.Skipped;
            }
        }
    }
}
