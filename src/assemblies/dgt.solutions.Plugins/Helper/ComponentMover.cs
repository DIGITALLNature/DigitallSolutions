using System;
using System.Collections.Generic;
using System.Linq;
using D365.Extension.Core;
using D365.Extension.Model;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace dgt.solutions.Plugins.Helper
{
    internal class ComponentMover
    {
        private readonly Executor _executor;
        private readonly WorkbenchHistoryLogger _workbenchHistoryLogger;

        public ComponentMover(Executor executor, WorkbenchHistoryLogger workbenchHistoryLogger = default)
        {
            _executor = executor;
            _workbenchHistoryLogger = workbenchHistoryLogger;
        }

        internal void MoveComponents(Guid originId, string destinationName)
        {
            MoveComponents(GetSolutionComponents(new List<ConditionExpression>{
                new ConditionExpression(SolutionComponent.LogicalNames.SolutionId, ConditionOperator.Equal, originId)
            }), destinationName);
        }

        internal void MoveAssemblyComponents(Guid originId, string destinationName)
        {
            var componentsTypes = new List<int>
            {
                SolutionComponent.Options.ComponentType.PluginAssembly,
                SolutionComponent.Options.ComponentType.SDKMessageProcessingStep
            };
            MoveComponents(GetSolutionComponents(new List<ConditionExpression>{
                new ConditionExpression(SolutionComponent.LogicalNames.SolutionId, ConditionOperator.Equal, originId),
                new ConditionExpression(SolutionComponent.LogicalNames.ComponentType, ConditionOperator.In, componentsTypes.ToArray())
            }), destinationName);
        }

        private IEnumerable<SolutionComponent> GetSolutionComponents(IEnumerable<ConditionExpression> conditions)
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
                var results = _executor.ElevatedOrganizationService.RetrieveMultiple(qe);
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

        private void MoveComponents(IEnumerable<SolutionComponent> components, string destinationName)
        {
            foreach (var component in components)
            {
                _executor.ElevatedOrganizationService.Execute(new AddSolutionComponentRequest
                {
                    AddRequiredComponents = false,
                    ComponentId = component.ObjectId.GetValueOrDefault(),
                    ComponentType = component.ComponentType.Value,
                    SolutionUniqueName = destinationName,
                    DoNotIncludeSubcomponents =
                        component.RootComponentBehavior?.Value == SolutionComponent.Options.RootComponentBehavior.DoNotIncludeSubcomponents ||
                        component.RootComponentBehavior?.Value == SolutionComponent.Options.RootComponentBehavior.IncludeAsShellOnly
                });
                _workbenchHistoryLogger?.LogComponent(component);
            }
        }
    }
}
