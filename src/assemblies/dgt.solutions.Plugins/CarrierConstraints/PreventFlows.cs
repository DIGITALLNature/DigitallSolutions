using System;
using System.Collections.Generic;
using System.Linq;
using D365.Extension.Model;
using D365.Extension.Registration;
using dgt.solutions.Plugins.Contract;
using Microsoft.Xrm.Sdk.Query;

namespace dgt.solutions.Plugins.CarrierConstraints
{
    [CustomApiRegistration(SdkMessageNames.DgtPreventFlows)]
    public sealed class PreventFlows : ConstraintBase
    {
        protected override string ConstraintType => "Prevent Flows";

        protected override ConstraintCheckLogEntry RunCheck(Guid solutionId)
        {
            var components = GetSolutionFlowComponents(solutionId);

            if (components.Any())
            {
                Delegate.TracingService.Trace("constraint violated");
                components.ForEach(c => WorkbenchHistoryLogger?.LogConstraintViolation(ConstraintType, c.ObjectId));
                return new ConstraintCheckLogEntry
                {
                    ConstraintType = "Prevent Flows",
                    Succeded = false,
                    ErrorComponents = components.Select(c => new ComponentInfo
                        { ComponentId = c.ObjectId.GetValueOrDefault(), ComponentType = "Flow" }).ToList()
                };
            }
            else
            {
                Delegate.TracingService.Trace("constraint passed");
            }

            WorkbenchHistoryLogger?.LogConstraintSuccess(ConstraintType);
            return new ConstraintCheckLogEntry
            {
                ConstraintType = "Prevent Flows",
                Succeded = true
            };
        }

        private List<SolutionComponent> GetSolutionFlowComponents(Guid solutionId)
        {
            var components = new List<SolutionComponent>();
            var qe = new QueryExpression(SolutionComponent.EntityLogicalName)
            {
                NoLock = true,
                ColumnSet = new ColumnSet(true)
            };
            qe.Criteria.Conditions.AddRange(new List<ConditionExpression>
            {
                new ConditionExpression(SolutionComponent.LogicalNames.SolutionId, ConditionOperator.Equal, solutionId),
                new ConditionExpression(SolutionComponent.LogicalNames.ComponentType, ConditionOperator.Equal,
                    SolutionComponent.Options.ComponentType.Workflow)
            });

            var workflowLink = qe.AddLink(Workflow.EntityLogicalName, SolutionComponent.LogicalNames.ObjectId,
                Workflow.LogicalNames.WorkflowId);
            workflowLink.LinkCriteria.FilterOperator = LogicalOperator.Or;
            workflowLink.LinkCriteria.AddCondition(Workflow.LogicalNames.Category, ConditionOperator.Equal,
                Workflow.Options.Category.DesktopFlow);
            workflowLink.LinkCriteria.AddCondition(Workflow.LogicalNames.Category, ConditionOperator.Equal,
                Workflow.Options.Category.ModernFlow);

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
                Delegate.TracingService.Trace("retrieving solution components (page {0})", qe.PageInfo.PageNumber);
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
    }
}