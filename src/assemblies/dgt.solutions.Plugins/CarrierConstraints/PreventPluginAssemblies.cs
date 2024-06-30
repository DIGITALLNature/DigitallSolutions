using System;
using System.Collections.Generic;
using System.Linq;
using D365.Extension.Core;
using D365.Extension.Model;
using D365.Extension.Registration;
using dgt.solutions.Plugins.Contract;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace dgt.solutions.Plugins.CarrierConstraints
{
    [CustomApiRegistration(SdkMessageNames.DgtPreventPluginAssemblies)]
    public class PreventPluginAssemblies : Executor
    {
        private PicklistAttributeMetadata _componentTypes;

        protected override ExecutionResult Execute()
        {
            GetInputParameter("Target", out EntityReference workbenchReference);

            var elevatedOrgService = ElevatedOrganizationService;

            var workbench = elevatedOrgService
                .Retrieve(DgtWorkbench.EntityLogicalName, workbenchReference.Id, new ColumnSet(true))
                .ToEntity<DgtWorkbench>();

            if (Guid.TryParse(workbench.DgtSolutionid, out var solutionId))
            {

                _componentTypes = ((RetrieveAttributeResponse)ElevatedOrganizationService.Execute(
                    new RetrieveAttributeRequest
                    {
                        EntityLogicalName = SolutionComponent.EntityLogicalName,
                        LogicalName = SolutionComponent.LogicalNames.ComponentType
                    })).AttributeMetadata as PicklistAttributeMetadata;
                var constraintCheckLog = CheckForAssemblysAndAssemblysSteps(solutionId);

                var constraintCheckLogJson = new SerializerService().JsonSerialize<ConstraintCheckLogEntry>(constraintCheckLog);

                SetOutputParameter(DgtPreventFlowsResponse.OutParameters.ConstraintLog_PreventFlows, constraintCheckLogJson);
            }
            else
            {
                throw new InvalidPluginExecutionException("Invalid Solution Id");
            }

            return ExecutionResult.Ok;
        }

        internal ConstraintCheckLogEntry CheckForAssemblysAndAssemblysSteps(Guid originId)
        {
            var componentsTypes = new List<int>
            {
                SolutionComponent.Options.ComponentType.PluginAssembly,
                SolutionComponent.Options.ComponentType.SDKMessageProcessingStep
            };
            var components = GetSolutionComponents(new List<ConditionExpression>
            {
                new ConditionExpression(SolutionComponent.LogicalNames.SolutionId, ConditionOperator.Equal, originId),
                new ConditionExpression(SolutionComponent.LogicalNames.ComponentType, ConditionOperator.In,
                    componentsTypes.ToArray())
            });

            if (components.Any())
                return new ConstraintCheckLogEntry
                {
                    ConstraintType = "Prevent PreventPluginAssemblys",
                    Succeded = false,
                    ErrorComponents = components.Select(c => new ComponentInfo
                    {
                        ComponentId = c.ObjectId.GetValueOrDefault(),
                        ComponentType = GetComponentTypeSetLabel(c.ComponentType.Value)
                    }).ToList()
                };
            return new ConstraintCheckLogEntry
            {
                ConstraintType = "Prevent PreventPluginAssemblys"
            };
        }

        private List<SolutionComponent> GetSolutionComponents(IEnumerable<ConditionExpression> conditions)
        {
            var components = new List<SolutionComponent>();
            var qe = new QueryExpression(SolutionComponent.EntityLogicalName)
            {
                NoLock = true,
                ColumnSet = new ColumnSet(true)
            };
            qe.Criteria.Conditions.AddRange(conditions);
            qe.Orders.Add(new OrderExpression
            {
                AttributeName = SolutionComponent.LogicalNames.SolutionComponentId,
                OrderType = OrderType.Ascending
            });
            qe.PageInfo = new PagingInfo
            {
                Count = 5000,
                PageNumber = 1,
                PagingCookie = null
            };
            while (true)
            {
                // Retrieve the page.
                var results = ElevatedOrganizationService.RetrieveMultiple(qe);
                components.AddRange(results.Entities.Select(e => e.ToEntity<SolutionComponent>()));
                if (results.MoreRecords)
                {
                    qe.PageInfo.PageNumber++;
                    qe.PageInfo.PagingCookie = results.PagingCookie;
                }
                else
                {
                    break;
                }
            }

            return components;
        }

        private string GetComponentTypeSetLabel(int value)
        {
            return _componentTypes.OptionSet.Options.Single(o => o.Value == value).Label.UserLocalizedLabel.Label;
        }

    }
}