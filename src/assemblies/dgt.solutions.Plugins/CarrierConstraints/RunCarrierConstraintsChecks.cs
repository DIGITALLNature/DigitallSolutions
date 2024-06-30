using System.Collections.Generic;
using System.Linq;
using D365.Extension.Core;
using D365.Extension.Model;
using D365.Extension.Registration;
using dgt.solutions.Plugins.Contract;
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
            if (carrier.DgtConstraintMset == null)
                yield return new ConstraintCheckLogEntry
                {
                    ConstraintType = "No constraints defined"
                };
            else
                foreach (var constraint in carrier.DgtConstraintMset)
                {
                    string constraintRequestName = string.Empty;
                    switch(constraint.Value)
                    {
                        case DgtCarrier.Options.DgtConstraintMset.PreventFlows:
                            constraintRequestName = SdkMessageNames.DgtPreventFlows;
                            break;
                        case DgtCarrier.Options.DgtConstraintMset.PreventItemsWithouthActiveLayer:
                            constraintRequestName = SdkMessageNames.DgtPreventItemsWithoutActiveLayer;
                            break;
                        case DgtCarrier.Options.DgtConstraintMset.PreventManagedEntitiesWithAllAssets:
                            constraintRequestName = SdkMessageNames.DgtPreventManagedTablesWithAllAssets;
                            break;
                        case DgtCarrier.Options.DgtConstraintMset.PreventPluginAssemblys:
                            constraintRequestName = SdkMessageNames.DgtPreventPluginAssemblies;
                            break;
                    };

                    if (string.IsNullOrEmpty(constraintRequestName))
                        continue;

                    var constraintCheckResponse = ElevatedOrganizationService.Execute(new OrganizationRequest(constraintRequestName)
                    {
                        Parameters = new ParameterCollection
                        {
                            { "Target", workbench.ToEntityReference() },
                        },
                    });

                    constraintCheckResponse.Results.TryGetValue("Log", out var constraintLog);
                    yield return new SerializerService().JsonDeserialize<ConstraintCheckLogEntry>(constraintLog as string);
                }
        }
    }
}