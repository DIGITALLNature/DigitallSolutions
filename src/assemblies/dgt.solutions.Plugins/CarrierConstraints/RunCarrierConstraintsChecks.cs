using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using D365.Extension.Core;
using D365.Extension.Model;
using D365.Extension.Registration;
using dgt.solutions.Plugins.Contract;
using dgt.solutions.Plugins.Helper;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace dgt.solutions.Plugins.CarrierConstraints
{
    [CustomApiRegistration(SdkMessageNames.DgtRunCarrierConstraintsCheck)]
    public class RunCarrierConstraintsCheck : Executor
    {
        protected override ExecutionResult Execute()
        {
            GetInputParameter("Target", out EntityReference workbenchReference);

            var elevatedOrgService = ElevatedOrganizationService;

            var workbench = elevatedOrgService
                .Retrieve(DgtWorkbench.EntityLogicalName, workbenchReference.Id, new ColumnSet(true))
                .ToEntity<DgtWorkbench>();
            var carrier = elevatedOrgService
                .Retrieve(DgtCarrier.EntityLogicalName, workbench.DgtTargetCarrierId.Id, new ColumnSet(true))
                .ToEntity<DgtCarrier>();

            var constraintCheckLog = Check(carrier, workbench);
            var constraintCheckLogJson = new SerializerService().JsonSerialize<List<ConstraintCheckLogEntry>>(constraintCheckLog);

            SetOutputParameter(DgtRunCarrierConstraintsCheckResponse.OutParameters.CarrierConstraintsSuccessStatus, constraintCheckLog.All(cl => cl.Succeded));
            SetOutputParameter(DgtRunCarrierConstraintsCheckResponse.OutParameters.CarrierConstraintsLog, constraintCheckLogJson);

            return ExecutionResult.Ok;
        }

        internal IEnumerable<ConstraintCheckLogEntry> Check(DgtCarrier carrier, DgtWorkbench workbench)
        {
            var constraintCheck = new ConstraintCheck(this);

            if (carrier.DgtConstraintMset == null)
                yield return new ConstraintCheckLogEntry
                {
                    ConstraintType = "No constraints defined"
                };
            else
                foreach (var constraint in carrier.DgtConstraintMset)
                    switch (constraint.Value)
                    {
                        case DgtCarrier.Options.DgtConstraintMset.PreventFlows:
                            var preventFlowsResponse = ElevatedOrganizationService.Execute(new DgtPreventFlowsRequest
                            {
                                Target = workbench.ToEntityReference()
                            }) as DgtPreventFlowsResponse;

                            yield return new SerializerService().JsonDeserialize<ConstraintCheckLogEntry>(preventFlowsResponse.ConstraintLog_PreventFlows);
                            break;
                        case DgtCarrier.Options.DgtConstraintMset.PreventItemsWithouthActiveLayer:
                            yield return constraintCheck.CheckForItemsWithouthActiveLayer(Guid.Parse(workbench.DgtSolutionid));
                            break;
                        case DgtCarrier.Options.DgtConstraintMset.PreventManagedEntitiesWithAllAssets:
                            yield return constraintCheck.CheckForManagedEntitiesWithAllAssets(Guid.Parse(workbench.DgtSolutionid));
                            break;
                        case DgtCarrier.Options.DgtConstraintMset.PreventPluginAssemblys:
                            yield return constraintCheck.CheckForAssemblysAndAssemblysSteps(Guid.Parse(workbench.DgtSolutionid));
                            break;
                    }
        }
    }
}