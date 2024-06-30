using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
            var constraintsQuery = new QueryExpression(DgtCarrierConstraint.EntityLogicalName);

            constraintsQuery.AddLink(DgtCarrierConstraintDgtCarrier.EntityLogicalName, DgtCarrierConstraint.LogicalNames.DgtCarrierConstraintId, DgtCarrierConstraintDgtCarrier.LogicalNames.DgtCarrierConstraintid, JoinOperator.LeftOuter)
                .EntityAlias = "carrier";
            var customApiLink = constraintsQuery.AddLink(CustomAPI.EntityLogicalName, DgtCarrierConstraint.LogicalNames.DgtCustomapiId, CustomAPI.LogicalNames.CustomAPIId, JoinOperator.Inner);
            customApiLink.Columns = new ColumnSet(CustomAPI.LogicalNames.UniqueName);
            customApiLink.EntityAlias = "customapi";

            constraintsQuery.Criteria.AddCondition(DgtCarrierConstraint.LogicalNames.Statecode, ConditionOperator.Equal, DgtCarrierConstraint.Options.Statecode.Active);
            constraintsQuery.Criteria.AddFilter(new FilterExpression
            {
                FilterOperator = LogicalOperator.Or,
                Conditions =
                {
                    new ConditionExpression(DgtCarrierConstraint.LogicalNames.DgtDefaultBit, ConditionOperator.Equal, DgtCarrierConstraint.Options.DgtDefaultBit.Yes),
                    new ConditionExpression("carrier", DgtCarrier.LogicalNames.DgtCarrierId, ConditionOperator.Equal, carrier.Id),
                },
            });

            var constraintsResponse = ElevatedOrganizationService.RetrieveMultiple(constraintsQuery);

            if (!constraintsResponse.Entities.Any())
                yield return new ConstraintCheckLogEntry
                {
                    ConstraintType = "No constraints defined"
                };
            else
                foreach (var constraint in constraintsResponse.Entities)
                {
                    var constraintRequestName = constraint.GetAttributeValue<AliasedValue>($"customapi.{CustomAPI.LogicalNames.UniqueName}")?.Value as string;

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