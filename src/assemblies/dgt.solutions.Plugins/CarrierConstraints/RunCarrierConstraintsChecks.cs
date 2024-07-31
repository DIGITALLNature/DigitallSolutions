using System;
using System.Linq;
using D365.Extension.Core;
using D365.Extension.Model;
using D365.Extension.Registration;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace dgt.solutions.Plugins.CarrierConstraints
{
    [CustomApiRegistration(SdkMessageNames.DgtRunCarrierConstraintsCheck)]
    public class RunCarrierConstraintsCheck : Executor
    {
        protected override ExecutionResult Execute()
        {
            Delegate.TracingService.Trace("starting {0}", nameof(RunCarrierConstraintsCheck));

            GetInputParameter("Target", out EntityReference carrierReference);
            GetInputParameter("WorkbenchHistory", out EntityReference workbenchHistoryReference);

            var elevatedOrgService = ElevatedOrganizationService;

            Delegate.TracingService.Trace("retrieving carrier {0}", carrierReference?.Id);
            var carrier = elevatedOrgService
                .Retrieve(DgtCarrier.EntityLogicalName, carrierReference.Id, new ColumnSet(true))
                .ToEntity<DgtCarrier>();

            var solutionId = Guid.Parse(carrier.DgtSolutionid);
            if (GetInputParameter("Workbench", out EntityReference workbenchReference))
            {
                Delegate.TracingService.Trace("retrieving workbench {0}", workbenchReference?.Id);
                var workbench = elevatedOrgService
                    .Retrieve(DgtWorkbench.EntityLogicalName, workbenchReference.Id, new ColumnSet(true))
                    .ToEntity<DgtWorkbench>();

                solutionId = Guid.Parse(workbench.DgtSolutionid);
            }

            Delegate.TracingService.Trace("running checks for solution {0}", solutionId);
            var checksPassed = Check(carrier, solutionId, workbenchHistoryReference);

            Delegate.TracingService.Trace($"setting success status to {checksPassed}");
            SetOutputParameter(DgtRunCarrierConstraintsCheckResponse.OutParameters.CarrierConstraintsSuccessStatus, checksPassed);

            Delegate.TracingService.Trace("finished {0}", nameof(RunCarrierConstraintsCheck));
            return ExecutionResult.Ok;
        }

        internal bool Check(DgtCarrier carrier, Guid solutionId, EntityReference workbenchHistoryReference)
        {
            var workbenchHistoryLogger = new WorkbenchHistoryLogger(OrganizationService(), new DgtWorkbenchHistory { Id = workbenchHistoryReference.Id });
            workbenchHistoryLogger.LogDebug("start checking constraints");

            Delegate.TracingService.Trace("preparing constraints query");
            var customApiQuery = new QueryExpression(CustomAPI.EntityLogicalName)
            {
                Distinct = true,
                ColumnSet = new ColumnSet(CustomAPI.LogicalNames.UniqueName),
            };

            var constraintLink = customApiQuery.AddLink(DgtCarrierConstraint.EntityLogicalName, CustomAPI.LogicalNames.CustomAPIId, DgtCarrierConstraint.LogicalNames.DgtCustomapiId, JoinOperator.Inner);
            constraintLink.EntityAlias = "constraint";

            var carrierConstraintLink = constraintLink.AddLink(DgtCarrierConstraintDgtCarrier.EntityLogicalName, DgtCarrierConstraint.LogicalNames.DgtCarrierConstraintId, DgtCarrierConstraintDgtCarrier.LogicalNames.DgtCarrierConstraintid, JoinOperator.LeftOuter);
            carrierConstraintLink.EntityAlias = "carrierconstraint";

            var carrierLink = carrierConstraintLink.AddLink(DgtCarrier.EntityLogicalName, DgtCarrierConstraintDgtCarrier.LogicalNames.DgtCarrierid, DgtCarrier.LogicalNames.DgtCarrierId, JoinOperator.LeftOuter);
            carrierLink.EntityAlias = "carrier";

            customApiQuery.Criteria.AddCondition("constraint", DgtCarrierConstraint.LogicalNames.Statecode, ConditionOperator.Equal, DgtCarrierConstraint.Options.Statecode.Active);
            customApiQuery.Criteria.AddFilter(new FilterExpression
            {
                FilterOperator = LogicalOperator.Or,
                Conditions =
                {
                    new ConditionExpression("constraint", DgtCarrierConstraint.LogicalNames.DgtDefaultBit, ConditionOperator.Equal, DgtCarrierConstraint.Options.DgtDefaultBit.Yes),
                    new ConditionExpression("carrier", DgtCarrier.LogicalNames.DgtCarrierId, ConditionOperator.Equal, carrier.Id),
                },
            });

            Delegate.TracingService.Trace("retrieving constraints");
            var customApiResponse = ElevatedOrganizationService.RetrieveMultiple(customApiQuery);

            if (!customApiResponse.Entities.Any())
            {
                Delegate.TracingService.Trace("no constraints defined");
                workbenchHistoryLogger.LogDebug("finished constraints - no constraints defined");
                return true;
            }
            else
            {
                var passed = true;
                Delegate.TracingService.Trace("running constraints");
                foreach (var customApi in customApiResponse.Entities.Cast<CustomAPI>())
                {
                    var constraintRequestName = customApi.UniqueName;

                    Delegate.TracingService.Trace("running constraint check: {0}", constraintRequestName);
                    var constraintCheckResponse = ElevatedOrganizationService.Execute(new OrganizationRequest(constraintRequestName)
                    {
                        Parameters = new ParameterCollection
                        {
                            { "Target", new EntityReference(Solution.EntityLogicalName, solutionId) },
                            { "WorkbenchHistory", workbenchHistoryReference },
                        },
                    });

                    constraintCheckResponse.Results.TryGetValue("Success", out bool success);
                    Delegate.TracingService.Trace($"setting output success to {success}");
                    passed = passed && success;
                }
                return passed;
            }
        }
    }
}
